// Authentication service, provides methods for API authentication
// ----------------------------------------------------------------------------

// Import dependencies
import { Subject } from 'rxjs';
import { Injectable, Injector } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Router, ActivatedRoute, RouterStateSnapshot } from '@angular/router';

/**
 * Authenticated user claim type
 */
export type TAuthenticatedUserClaim = { type: string; value: string };

/**
 * Authentication service, provides methods for API authentication
 */
@Injectable()
export class AuthenticationService {
  /**
   * Authentication API root path
   */
  private _path = '/auth';
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
  public get accessToken(): string | undefined {
    return localStorage.getItem('ngz-utils-identity:AccessToken') || undefined;
  }
  public set accessToken(value: string | undefined) {
    if (value) {
      localStorage.setItem('ngz-utils-identity:AccessToken', value);
    } else {
      localStorage.removeItem('ngz-utils-identity:AccessToken');
    }
  }
  /**
   * API auth access token creation time
   */
  private get _accessTokenCTime(): number | undefined {
    const value = localStorage.getItem('ngz-utils-identity:accessTokenCTime');
    return value ? parseInt(value, 10) : undefined;
  }
  private set _accessTokenCTime(value: number | undefined) {
    if (value) {
      localStorage.setItem('ngz-utils-identity:accessTokenCTime', value.toString());
    } else {
      localStorage.removeItem('ngz-utils-identity:accessTokenCTime');
    }
  }
  /**
   * API auth access token time-to-live
   */
  private get _accessTokenTTL(): number | undefined {
    const value = localStorage.getItem('ngz-utils-identity:accessTokenTTL');
    return value ? parseInt(value, 10) : undefined;
  }
  private set _accessTokenTTL(value: number | undefined) {
    if (value) {
      localStorage.setItem('ngz-utils-identity:accessTokenTTL', value.toString());
    } else {
      localStorage.removeItem('ngz-utils-identity:accessTokenTTL');
    }
  }
  /**
   * API auth refresh token if onw is known
   */
  private get _refreshToken(): string | undefined {
    return localStorage.getItem('ngz-utils-identity:RefreshToken') || undefined;
  }
  private set _refreshToken(value: string | undefined) {
    if (value) {
      localStorage.setItem('ngz-utils-identity:RefreshToken', value);
    } else {
      localStorage.removeItem('ngz-utils-identity:RefreshToken');
    }
  }

  /**
   * Holds reference to token refresh timeout
   */
  private _refreshTokenTimeout?: any;

  /**
   * Authenticated user info
   */
  private _info?: any;
  /**
   * Gets info for the authenticated user if user not authenticated
   */
  public get info(): any | undefined {
    return this._info;
  }
  /**
   * Authenticated user's claims
   */
  private _claims: TAuthenticatedUserClaim[] = [];
  /**
   * Gets authenticated user's claims
   */
  public get claims(): TAuthenticatedUserClaim[] {
    return this._claims;
  }
  /**
   * Authenticated user's roles
   */
  private _roles: string[] = [];
  /**
   * Gets authenticated user's roles
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

  constructor(private _injector: Injector, private _router: Router, private _route: ActivatedRoute, private _http: HttpClient) {}

  /**
   * Initializes the authentication service, checking if current user is authenticated
   * @param path Authentication API root path (defaults to '/auth')
   * @param refreshOnLogin If login should trigger a full page refresh (for safety, to clear any left-over data)
   * @param refreshOnLogout If logout should trigger a full page refresh (for safety, to clear any left-over data)
   */
  public async initialize(path: string = '/auth', { refreshOnLogin = true, refreshOnLogout = true } = {}): Promise<void> {
    // Set configuration
    this._path = path.endsWith('/') ? path.substr(0, path.length - 1) : path;
    this._refreshOnLogin = refreshOnLogin;
    this._refreshOnLogout = refreshOnLogout;

    // Check if user authenticated and if so get authenticated user's info
    await this._getAuthInfo();

    // Update initialized status
    this._isInitialized = true;

    // Refresh routing on authentication change
    this._isAuthenticatedObservable.subscribe(_ => {
      this._forceRerunAuthGuard();
    });
  }

  /**
   * Performs user log-in
   * @param username Username
   * @param password Password
   */
  public async login(username: string, password: string): Promise<boolean> {
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
      if (!res.success || !res?.data?.[0]?.accessToken || !res?.data?.[0]?.refreshToken || !res?.data?.[0]?.lifetime) {
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
        return true;
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
   * TODO: Revoke the current token instead of just doing a local logout
   */
  public async logout(): Promise<void> {
    // Remove tokens
    this.accessToken = undefined;
    this._accessTokenCTime = undefined;
    this._accessTokenTTL = undefined;
    this._refreshToken = undefined;

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

  public async refreshAuthenticationInfo(): Promise<boolean> {
    return this._getAuthInfo();
  }

  public async refreshAuthenticationToken(): Promise<void> {
    return this._refreshAuth();
  }

  /**
   * Checks if user currently authenticated and if so fetches user info
   */
  private async _getAuthInfo(): Promise<boolean> {
    try {
      // Check if user authorized
      const res = (await this._http.get(`${this._path}/init`).toPromise()) as any;

      // (Re)Start token refresh process in background
      this._startTokenRefreshScheduler();

      // Store user info
      this._info = res?.data?.[0];
      this._claims = this._info.claims ? [...this._info.claims] : [];
      this._roles = this._claims.filter(claim => claim.type === 'http://schemas.microsoft.com/ws/2008/06/identity/claims/role').map(claim => claim.value);
      delete this._info.claims;
      this._isAuthenticatedObservable.next(this);
      return true;
    } catch (err) {
      // Handle not authorized (401) response
      if (err instanceof HttpErrorResponse && (err as HttpErrorResponse).status === 401) {
        // Store user info
        this._info = false;
        this._isAuthenticatedObservable.next(this);
        return false;
      }
      // Handle other errors
      else {
        this._isAuthenticatedObservable.next(this);
        throw err;
      }
    }
  }

  /**
   * Refreshes user auth token
   */
  private async _refreshAuth(): Promise<void> {
    // Attempt refresh-token API request
    const res = (await this._http
      .post(`${this._path}/token`, {
        grantType: 'refresh_token',
        refreshToken: this._refreshToken,
      })
      .toPromise()) as any;

    // Handle failed login
    if (!res.success || !res?.data?.[0]?.accessToken || !res?.data?.[0]?.refreshToken || !res?.data?.[0]?.lifetime) {
      throw new Error('Failed refreshing authentication token!');
    }

    // Set tokens
    this.accessToken = res?.data?.[0]?.accessToken;
    this._accessTokenCTime = Date.now();
    this._accessTokenTTL = res?.data?.[0]?.lifetime;
    this._refreshToken = res?.data?.[0]?.refreshToken;

    // (Re)Start token refresh process in background
    this._startTokenRefreshScheduler();
  }

  /**
   * Starts auth token refresh scheduler in background
   */
  private _startTokenRefreshScheduler(): void {
    // Stop any previously scheduled token refresh timeouts
    this._stopTokenRefreshScheduler();
    // (Re)Schedule a refresh token timeout
    if (this._accessTokenCTime && this._accessTokenTTL) {
      const refreshInterval = this._accessTokenCTime + this._accessTokenTTL * 1000 - Date.now();
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
    if (this._refreshTokenTimeout !== undefined) {
      clearTimeout(this._refreshTokenTimeout);
    }
  }

  /**
   * Dirty hack for angular2 routing recheck
   * (https://stackoverflow.com/questions/45680250/angular2-how-to-reload-page-with-router-recheck-canactivate)
   */
  private _forceRerunAuthGuard(): void {
    if (this._route.root.children.length) {
      // Gets current route
      const currentRoute = this._route.root.children[0];
      // Gets first guard class
      // tslint:disable-next-line: variable-name
      const AuthGuard = currentRoute?.snapshot?.routeConfig?.canActivate?.[0];
      // Injects guard
      // tslint:disable-next-line: deprecation
      const authGuard = this._injector.get(AuthGuard);
      // Makes custom RouterStateSnapshot object
      const routerStateSnapshot: RouterStateSnapshot = Object.assign({}, currentRoute.snapshot, { url: this._router.url });
      // Runs canActivate
      authGuard.canActivate(currentRoute.snapshot, routerStateSnapshot, true);
    }
  }
}
