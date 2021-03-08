// Routing module
// Defines application routes and registers them with the app
// ----------------------------------------------------------------------------

// Import modules
import { NgModule, Injectable } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router, Routes } from '@angular/router';

// Import components
import { SomeComponent } from './component';

// Import route guards
import { AuthenticationService, AuthenticationRouteGuardFactory } from '@intellegens/ngz-utils-identity';

/**
 * When authenticated router guard, redirects to /public if not authenticated
 */
@Injectable()
class WhenAuthenticated extends AuthenticationRouteGuardFactory((auth: AuthenticationService) => auth.isAuthenticated, 'public') {
  constructor(public _router: Router, public _auth: AuthenticationService) {
    super(_router, _auth);
  }
}

/**
 * When not-authenticated router guard, redirects to /private if not authenticated
 */
@Injectable()
class WhenNotAuthenticated extends AuthenticationRouteGuardFactory((auth: AuthenticationService) => !auth.isAuthenticated, 'private') {
  constructor(public _router: Router, public _auth: AuthenticationService) {
    super(_router, _auth);
  }
}

// Routes definition
const routes: Routes = [
  {
    path: 'private',
    canActivate: [WhenAuthenticated],
    component: SomeComponent,
  },
  {
    path: 'public',
    canActivate: [WhenNotAuthenticated],
    component: SomeComponent,
  },
  { path: '**', redirectTo: 'public' },
];

/**
 * Routing module
 * Defines application routes and registers them with the app
 */
@NgModule({
  declarations: [SomeComponent],
  providers: [AuthenticationService, WhenAuthenticated, WhenNotAuthenticated],
  imports: [RouterModule.forRoot(routes), CommonModule],
  exports: [RouterModule, CommonModule],
})
export class AppRoutingModule {}
