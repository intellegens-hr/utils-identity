// Authentication service, provides methods for API authentication
// ----------------------------------------------------------------------------

// Import dependencies
import { Subject } from 'rxjs';
import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Router } from '@angular/router';

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
  private get _accessTokenCTime(): number | null {
    const value = localStorage.getItem('ngz-utils-identity:accessTokenCTime');
    return value ? parseInt(value, 10) : null;
  }
  private set _accessTokenCTime(value: number | null) {
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
  private get _accessTokenTTL(): number | null {
    const value = localStorage.getItem('ngz-utils-identity:accessTokenTTL');
    return value ? parseInt(value, 10) : null;
  }
  private set _accessTokenTTL(value: number | null) {
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
  private get _refreshToken(): string | null {
    return localStorage.getItem('ngz-utils-identity:RefreshToken');
  }
  private set _refreshToken(value: string | null) {
    if (value) {
      localStorage.setItem('ngz-utils-identity:RefreshToken', value);
    } else {
      localStorage.removeItem('ngz-utils-identity:RefreshToken');
    }
  }

  /**
   * Holds reference to token refresh timeout
   */
  private _refreshTokenTimeout: any;

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
   * Authenticated user's claims
   */
  private _claims: { type: string; value: string }[] = [];
  /**
   * Gets authenticated user's claims
   */
  public get claims(): { type: string; value: string }[] {
    return this._claims;
  }
  /**
   * Authenticated user's roles
   */
  private _roles: string[] = [];
  /**
   * Gets info for the authenticated user or false if user not authenticated
   */
  public get roles(): string[] {
    return this._roles;
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
  private _isAuthenticatedObservable = new Subject<AuthenticationService>();
  /**
   * Gets authentication state observable
   */
  public get isAuthenticatedObservable(): Subject<any> {
    return this._isAuthenticatedObservable;
  }

  constructor(private _http: HttpClient, private _router: Router) {}

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
    this._isAuthenticatedObservable.subscribe((_) => {
      this._router.navigate([], {
        skipLocationChange: true,
        queryParamsHandling: 'merge',
      });
      // TODO: (Re)evaluate route guards without reload
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
      this._accessTokenCTime = Date.now();
      this._accessTokenTTL = res?.data?.[0]?.lifetime;
      this._refreshToken = res?.data?.[0]?.refreshToken;

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
   * TODO: Cancel the current token instead of just doi8ng a local logout
   */
  public async logout(): Promise<void> {
    // Remove tokens
    this.accessToken = null;
    this._accessTokenCTime = null;
    this._accessTokenTTL = null;
    this._refreshToken = null;

    // Remove info
    this._info = false;
    this._isAuthenticatedObservable.next(this);

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

      // (Re)Start token refresh process in background
      this._startTokenRefreshScheduler();

      // Store user info
      this._info = res?.data?.[0];
      this._claims = this._info.claims ? [...this._info.claims] : [];
      this._roles = this._claims
        .filter(
          (claim) =>
            claim.type ===
            'http://schemas.microsoft.com/ws/2008/06/identity/claims/role'
        )
        .map((claim) => claim.value);
      delete this._info.claims;
      this._isAuthenticatedObservable.next(this);
      return this._info;
    } catch (err) {
      // Handle not authorized (401) response
      if (
        err instanceof HttpErrorResponse &&
        (err as HttpErrorResponse).status === 401
      ) {
        // Store user info
        this._info = false;
        this._isAuthenticatedObservable.next(this);
        return false;
      }
      // Handle other errors
      else {
        throw err;
      }
    }
  }

  /**
   * Refreshes user auth token
   */
  private async _refreshAuth(): Promise<any | false> {
    try {
      // Attempt refresh-token API request
      const res = (await this._http
        .post(`${this._path}/token`, {
          grantType: 'refresh_token',
          refreshToken: this._refreshToken,
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
      this._accessTokenCTime = Date.now();
      this._accessTokenTTL = res?.data?.[0]?.lifetime;
      this._refreshToken = res?.data?.[0]?.refreshToken;

      // (Re)Start token refresh process in background
      this._startTokenRefreshScheduler();
    } catch (err) {
      // Return failed login
      return false;
    }
  }

  /**
   * Starts auth token refresh scheduler in background
   */
  private _startTokenRefreshScheduler(): void {
    // Stop any previously scheduled token refresh timeouts
    this._stopTokenRefreshScheduler();
    // (Re)Schedule a refresh token timeout
    if (this._accessTokenCTime && this._accessTokenTTL) {
      const refreshInterval =
        this._accessTokenCTime + this._accessTokenTTL * 1000 - Date.now();
      this._refreshTokenTimeout = setTimeout(() => {
        try {
          this._refreshAuth();
        } catch (err) {}
      }, refreshInterval);
    }
  }
  /**
   * Stops auth token refresh scheduler
   */
  private _stopTokenRefreshScheduler(): void {
    // Clear refresh-token timeout
    clearTimeout(this._refreshTokenTimeout);
  }
}
