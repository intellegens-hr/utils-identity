// Authentication route guards
// ----------------------------------------------------------------------------

// Import dependencies
import { Observable, of } from 'rxjs';
import { map } from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { CanActivate, Router, UrlTree } from '@angular/router';
import { AuthenticationService } from '../Authentication';

/**
 * Route guard allows routes only when authenticated
 */
@Injectable()
export class WhenAuthenticated implements CanActivate {
  /**
   * Path to redirect to when route condition not met
   */
  private static _defaultPath = '/';
  /**
   * Sets path to redirect to when route condition not met
   * @param path Path to redirect to when route condition not met
   */
  public static onFailRedirectTo(path: string): void {
    WhenAuthenticated._defaultPath = path;
  }

  constructor(private _router: Router, private _auth: AuthenticationService) {}

  /**
   * Checks if route can be activated
   */
  public canActivate(): Observable<boolean | UrlTree> {
    // If authentication initialized, resolve
    if (this._auth.isInitialized) {
      return !!this._auth.info
        ? of(true)
        : of(this._router.parseUrl(WhenAuthenticated._defaultPath));
    }
    // If not initialized, wait for initialization
    else {
      return this._auth.isAuthenticatedObservable.pipe(
        map((info) =>
          !!info ? true : this._router.parseUrl(WhenAuthenticated._defaultPath)
        )
      );
    }
  }
}

/**
 * Route guard allows routes only when not authenticated
 */
@Injectable()
export class WhenNotAuthenticated implements CanActivate {
  /**
   * Path to redirect to when route condition not met
   */
  private static _defaultPath = '/';
  /**
   * Sets path to redirect to when route condition not met
   * @param path Path to redirect to when route condition not met
   */
  public static onFailRedirectTo(path: string): void {
    WhenNotAuthenticated._defaultPath = path;
  }

  constructor(private _router: Router, private _auth: AuthenticationService) {}

  /**
   * Checks if route can be activated
   */
  public canActivate(): Observable<boolean | UrlTree> {
    // If authentication initialized, resolve
    if (this._auth.isInitialized) {
      return !this._auth.info
        ? of(true)
        : of(this._router.parseUrl(WhenNotAuthenticated._defaultPath));
    }
    // If not initialized, wait for initialization
    else {
      return this._auth.isAuthenticatedObservable.pipe(
        map((info) =>
          !info
            ? true
            : this._router.parseUrl(WhenNotAuthenticated._defaultPath)
        )
      );
    }
  }
}
