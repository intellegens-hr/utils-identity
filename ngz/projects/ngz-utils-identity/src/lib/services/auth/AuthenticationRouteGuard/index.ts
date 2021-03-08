// Authentication route guards
// ----------------------------------------------------------------------------

// Import dependencies
import { Observable, of } from 'rxjs';
import { map } from 'rxjs/operators';
import { CanActivate, Router, UrlTree } from '@angular/router';
import { AuthenticationService } from '../Authentication';

/**
 * Route guard factory, allows routes only when authenticated (TODO: ...)
 * @param authenticationValidationCallbackFn
 * @param defaultPath
 * @returns
 */
export function AuthenticationRouterGuardFactory(
  authenticationValidationCallbackFn: (
    isAuthenticated: boolean,
    info: any,
    claims: { type: string; value: string }[],
    roles: string[]
  ) => boolean,
  defaultPath = '/'
): new (_router: Router, _auth: AuthenticationService) => CanActivate {
  /**
   *  (TODO: ...)
   */
  return class WhenAuthenticatedGuard implements CanActivate {
    constructor(public _router: Router, public _auth: AuthenticationService) {}

    /**
     * Checks if route can be activated
     */
    public canActivate(): Observable<boolean | UrlTree> {
      // If authentication initialized, resolve
      if (this._auth.isInitialized) {
        return authenticationValidationCallbackFn(
          this._auth.isAuthenticated,
          this._auth.info,
          this._auth.claims,
          this._auth.roles
        )
          ? of(true)
          : of(this._router.parseUrl(defaultPath));
      }
      // If not initialized, wait for initialization
      else {
        return this._auth.isAuthenticatedObservable.pipe(
          map((auth: AuthenticationService) =>
            authenticationValidationCallbackFn(
              auth.isAuthenticated,
              auth.info,
              auth.claims,
              auth.roles
            )
              ? true
              : this._router.parseUrl(defaultPath)
          )
        );
      }
    }
  };
}
