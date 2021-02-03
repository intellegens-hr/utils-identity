// Authentication service, provides methods for API authentication
// ----------------------------------------------------------------------------

// Import dependencies
import { Subject } from 'rxjs';
import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';

/**
 * Authentication service, provides methods for API authentication
 */
@Injectable()
export class AuthenticationService {
  /**
   * Authentication API root path
   */
  private _path = 'auth';
  /**
   * If login should trigger a full page refresh (for safety, to clear any left-over data)
   */
  private _refreshOnLogin = true;
  /**
   * If logout should trigger a full page refresh (for safety, to clear any left-over data)
   */
  private _refreshOnLogout = true;

  /**
   * Auth service's initialized status (will be true once service has finished initializing)
   */
  private _isInitialized = false;
  /**
   * Gets auth service's initialized status (will return true once service has finished initializing)
   */
  public get isInitialized(): boolean {
    return this._isInitialized;
  }

  /**
   * API auth access token
   */
  public get accessToken(): string | null {
    return localStorage.getItem('ngz-utils-identity:AccessToken');
  }
  public set accessToken(value: string | null) {
    if (value) {
      localStorage.setItem('ngz-utils-identity:AccessToken', value);
    } else {
      localStorage.removeItem('ngz-utils-identity:AccessToken');
    }
  }
  /**
   * API auth access token creation time
   */
  public get accessTokenCTime(): number | null {
    const value = localStorage.getItem('ngz-utils-identity:accessTokenCTime');
    return value ? parseInt(value, 10) : null;
  }
  public set accessTokenCTime(value: number | null) {
    if (value) {
      localStorage.setItem(
        'ngz-utils-identity:accessTokenCTime',
        value.toString()
      );
    } else {
      localStorage.removeItem('ngz-utils-identity:accessTokenCTime');
    }
  }
  /**
   * API auth access token time-to-live
   */
  public get accessTokenTTL(): number | null {
    const value = localStorage.getItem('ngz-utils-identity:accessTokenTTL');
    return value ? parseInt(value, 10) : null;
  }
  public set accessTokenTTL(value: number | null) {
    if (value) {
      localStorage.setItem(
        'ngz-utils-identity:accessTokenTTL',
        value.toString()
      );
    } else {
      localStorage.removeItem('ngz-utils-identity:accessTokenTTL');
    }
  }
  /**
   * API auth refresh token if onw is known
   */
  public get refreshToken(): string | null {
    return localStorage.getItem('ngz-utils-identity:RefreshToken');
  }
  public set refreshToken(value: string | null) {
    if (value) {
      localStorage.setItem('ngz-utils-identity:RefreshToken', value);
    } else {
      localStorage.removeItem('ngz-utils-identity:RefreshToken');
    }
  }

  /**
   * Authenticated user info
   */
  private _info?: any;
  /**
   * Gets info for the authenticated user or false if user not authenticated
   */
  public get info(): any | false {
    return this._info || false;
  }
  /**
   * Gets authentication state
   */
  public get isAuthenticated(): boolean {
    return !!this._info;
  }

  /**
   * Authentication state observable
   */
  private _isAuthenticatedObservable = new Subject<any>();
  /**
   * Gets authentication state observable
   */
  public get isAuthenticatedObservable(): Subject<any> {
    return this._isAuthenticatedObservable;
  }

  constructor(private _http: HttpClient) {}

  /**
   * Initializes the authentication service, checking if current user is authenticated
   * @param path Authentication API root path (defaults to '/auth')
   * @param refreshOnLogin If login should trigger a full page refresh (for safety, to clear any left-over data)
   * @param refreshOnLogout If logout should trigger a full page refresh (for safety, to clear any left-over data)
   */
  public async initialize(
    path: string = 'auth',
    { refreshOnLogin = true, refreshOnLogout = true } = {}
  ): Promise<any | false> {
    // Set configuration
    this._path = path.endsWith('/') ? path.substr(0, path.length - 1) : path;
    this._refreshOnLogin = refreshOnLogin;
    this._refreshOnLogout = refreshOnLogout;

    // Check if user authenticated and if so get authenticated user's info
    await this._getAuthInfo();

    // Update initialized status
    this._isInitialized = true;

    // Refresh routing on authentication change
    this._isAuthenticatedObservable.subscribe((isAuthenticated) => {
      // TODO: Refresh without reloading ...
      window.location.reload();
    });

    // Return authenticated info
    return this.info;
  }

  /**
   * Performs user log-in
   * @param username Username
   * @param password Password
   */
  public async login(username: string, password: string): Promise<any | false> {
    try {
      // Attempt login API request
      const res = (await this._http
        .post(`${this._path}/token`, {
          grantType: 'password',
          username,
          password,
        })
        .toPromise()) as any;

      // Handle failed login
      if (
        !res.success ||
        !res?.data?.[0]?.accessToken ||
        !res?.data?.[0]?.refreshToken ||
        !res?.data?.[0]?.lifetime
      ) {
        return false;
      }

      // Set tokens
      this.accessToken = res?.data?.[0]?.accessToken;
      this.accessTokenCTime = Date.now();
      this.accessTokenTTL = res?.data?.[0]?.lifetime;
      this.refreshToken = res?.data?.[0]?.refreshToken;

      // Start token refresh process in background
      this._startTokenRefreshScheduler();

      // Refresh on login (for safety, to clear any left-over data)
      if (this._refreshOnLogin) {
        window.location.reload();
      }
      // If no refresh, get authenticated user info
      else {
        return await this._getAuthInfo();
      }
    } catch (err) {
      // Return failed login
      return false;
    }
  }

  /**
   * Performs user log-out
   * TODO: ...
   */
  public async logout(): Promise<void> {
    // Remove tokens
    this.accessToken = null;
    this.accessTokenCTime = null;
    this.accessTokenTTL = null;
    this.refreshToken = null;

    // Remove info
    this._info = false;
    this._isAuthenticatedObservable.next(this._info);

    // Stop scheduled token refreshing
    this._stopTokenRefreshScheduler();

    // Refresh on logout (for safety, to clear any left-over data)
    if (this._refreshOnLogout) {
      window.location.reload();
    }
  }

  /**
   * Checks if user currently authenticated and if so fetches user info
   */
  private async _getAuthInfo(): Promise<any | false> {
    try {
      // Check if user authorized
      const res = (await this._http
        .get(`${this._path}/init`)
        .toPromise()) as any;
      // Store user info
      this._info = res?.data?.[0];
      this._isAuthenticatedObservable.next(this._info);
      return this._info;
    } catch (err) {
      // Handle not authorized (401) response
      if (
        err instanceof HttpErrorResponse &&
        (err as HttpErrorResponse).status === 401
      ) {
        // Store user info
        this._info = false;
        this._isAuthenticatedObservable.next(this._info);
        return false;
      }
      // Handle other errors
      else {
        throw err;
      }
    }
  }

  /**
   * Starts auth token refresh scheduler in background
   */
  private _startTokenRefreshScheduler(): void {
    // TODO: ...
  }
  /**
   * Stops auth token refresh scheduler
   */
  private _stopTokenRefreshScheduler(): void {
    // TODO: ...
  }
}
