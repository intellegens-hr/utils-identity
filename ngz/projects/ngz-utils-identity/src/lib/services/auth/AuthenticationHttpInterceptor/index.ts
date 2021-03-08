// Authentication service's HTTP interceptor, injects authentication onto HTTP requests
// ----------------------------------------------------------------------------

// Import dependencies
import { Observable } from 'rxjs';
import { Injectable } from '@angular/core';
import { HTTP_INTERCEPTORS, HttpRequest, HttpInterceptor, HttpEvent, HttpHandler } from '@angular/common/http';
import { AuthenticationService } from '../Authentication';

/**
 * Authentication service HTTP interceptor
 */
@Injectable()
class AuthenticationHttpInterceptor implements HttpInterceptor {
  constructor(private _auth: AuthenticationService) {}

  public intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    return next.handle(
      // Check if authenticated
      this._auth.accessToken
        ? // Append authorization header to the request
          req.clone({
            setHeaders: { Authorization: `Bearer ${this._auth.accessToken}` },
          })
        : // Do not modify the request
          req,
    );
  }
}

/**
 * Provider for authentication service HTTP interceptor
 */
// tslint:disable-next-line: variable-name
export const AuthenticationHttpInterceptorProvider = {
  provide: HTTP_INTERCEPTORS,
  useClass: AuthenticationHttpInterceptor,
  multi: true,
};
