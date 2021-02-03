// Authentication service, provides methods for API authentication
// ----------------------------------------------------------------------------

// Import dependencies
import { Observable } from 'rxjs';
import { Injectable } from '@angular/core';
import {
  HTTP_INTERCEPTORS,
  HttpClient,
  HttpRequest,
  HttpErrorResponse,
  HttpInterceptor,
  HttpEvent,
  HttpHandler,
} from '@angular/common/http';

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
   * Gets info for the authenticated user or false if user not authenticated
   */
  public get info(): any | false {
    return false;
  }

  constructor(private _http: HttpClient) {}

  /**
   * Initializes the authentication service, checking if current user is authenticated
   * @param path Authentication API root path (defaults to '/auth')
   */
  public async initialize(path: string = 'auth'): Promise<any | false> {
    // Set configuration
    this._path = path.endsWith('/') ? path.substr(0, path.length - 1) : path;

    // Check if user authenticated and if so get authenticated user's info
    await this._getAuthInfo();

    // Update initialized status
    this._isInitialized = true;

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

      // Get authenticated user info
      return await this._getAuthInfo();
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
    // TODO: ...

    // Stop scheduled token refreshing
    this._stopTokenRefreshScheduler();
  }

  /**
   * Checks if user currently authenticated and if so fetches user info
   */
  private async _getAuthInfo(): Promise<any | false> {
    try {
      // Check if user authorized
      const res = await this._http.get(`${this._path}/init`).toPromise();
      // Store user info
      // TODO: ...
      return null;
    } catch (err) {
      // Handle not authorized (401) response
      if (
        err instanceof HttpErrorResponse &&
        (err as HttpErrorResponse).status === 401
      ) {
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
  private _startTokenRefreshScheduler() {
    // TODO: ...
  }
  /**
   * Stops auth token refresh scheduler
   */
  private _stopTokenRefreshScheduler() {
    // TODO: ...
  }
}

/**
 * Authentication service HTTP interceptor
 */
@Injectable()
class AuthenticationServiceInterceptor implements HttpInterceptor {
  constructor(private _auth: AuthenticationService) {}

  public intercept(
    req: HttpRequest<any>,
    next: HttpHandler
  ): Observable<HttpEvent<any>> {
    return next.handle(
      // Check if authenticated
      this._auth.accessToken
        ? // Append authorization header to the request
          req.clone({
            setHeaders: { Authorization: `Bearer ${this._auth.accessToken}` },
          })
        : // Do not modify the request
          req
    );
  }
}

/**
 * Provider for authentication service HTTP interceptor
 */
// tslint:disable-next-line: variable-name
export const AuthenticationServiceInterceptopProvider = {
  provide: HTTP_INTERCEPTORS,
  useClass: AuthenticationServiceInterceptor,
  multi: true,
};
