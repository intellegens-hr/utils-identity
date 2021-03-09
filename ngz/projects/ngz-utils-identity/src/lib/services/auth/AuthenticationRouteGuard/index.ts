// Authentication route guards
// ----------------------------------------------------------------------------

// Import dependencies
import { Observable, of } from 'rxjs';
import { map } from 'rxjs/operators';
import { CanActivate, Router, ActivatedRouteSnapshot, RouterStateSnapshot, UrlTree } from '@angular/router';
import { AuthenticationService } from '../Authentication';

/**
 * Route guard factory, allows routes based on authentication status
 * @param authenticationValidationCallbackFn Callback determining if route guard should allow or prevent the route from being loaded
 * @param defaultPath Default path to redirect to if route guard prevents route from being loaded
 * @returns RouteGuard prototype to extend
 */
export function AuthenticationRouteGuardFactory(
  authenticationValidationCallbackFn: (auth: AuthenticationService) => boolean,
  defaultPath = '/',
): new (_router: Router, _auth: AuthenticationService) => CanActivate {
  /**
   *  Route guard prototype to be extended
   */
  return class AuthenticationGuard implements CanActivate {
    constructor(public _router: Router, public _auth: AuthenticationService) {}

    /**
     * Checks if route can be activated
     */
    public canActivate(
      _route: ActivatedRouteSnapshot,
      _state: RouterStateSnapshot,
    ): Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree;
    public canActivate(
      _route: ActivatedRouteSnapshot,
      _state: RouterStateSnapshot,
      forcedReevaluation: boolean = false,
    ): Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree {
      // If authentication initialized, resolve
      if (this._auth.isInitialized) {
        return authenticationValidationCallbackFn(this._auth) ? of(true) : of(composeDefaultPath(this._router, defaultPath, forcedReevaluation));
      }
      // If not initialized, wait for initialization
      else {
        return this._auth.isAuthenticatedObservable.pipe(
          map((auth: AuthenticationService) =>
            authenticationValidationCallbackFn(auth) ? true : composeDefaultPath(this._router, defaultPath, forcedReevaluation),
          ),
        );
      }
    }
  };
}

/**
 * Composes a path based on the default path given for a route guard, and in case of forced reevaluation of the route guard, navigates to the default path
 * @param router Router instance
 * @param defaultPath Default path provided to the route guard
 * @param forcedReevaluation If reevaluation of the route guard is being forced by changes to the authentication service
 * @returns Default path URL
 */
function composeDefaultPath(router: Router, defaultPath: string, forcedReevaluation: boolean = false): UrlTree {
  const url = router.parseUrl(defaultPath);
  if (forcedReevaluation) {
    router.navigateByUrl(url);
  }
  return url;
}
