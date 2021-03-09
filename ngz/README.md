# NgzIdentityUtils

## Usage

1. Import and initialize `NgzUtilsIdentityModule` in your main application module:

```ts
import {
  NgzUtilsIdentityModule,
  AuthenticationService,
} from "@intellegens/ngz-utils-identity";

@NgModule({
  imports: [AppRoutingModule, NgzUtilsIdentityModule],
})
export class AppModule {
  constructor(public _auth: AuthenticationService) {
    this._auth.initialize(`/auth`, {
      // If application should di a full page reload when logged in
      refreshOnLogin: true,
      // If application should di a full page reload when logged out
      refreshOnLogout: true,
    });
  }
}
```

2. Set up routing with route guards in your routing module:

```ts
import {
  AuthenticationService,
  AuthenticationRouteGuardFactory,
} from "@intellegens/ngz-utils-identity";

/**
 * When authenticated router guard, redirects to /public if not authenticated
 */
@Injectable()
class WhenAuthenticated extends AuthenticationRouteGuardFactory(
  // Route-guard check callback function
  (auth: AuthenticationService) => auth.isAuthenticated,
  // Path to default to if route-guard check returns false
  "public"
) {
  constructor(public _router: Router, public _auth: AuthenticationService) {
    super(_router, _auth);
  }
}

// Routes definition
const routes: Routes = [
  // Private route, protected by the WhenAuthenticated route-guard
  {
    path: "private",
    canActivate: [WhenAuthenticated],
    component: SomePrivatePageComponent,
  },
  // Public route, available to anyone
  {
    path: "public",
    component: SomePageComponent,
  },
  // Redirect
  { path: "**", redirectTo: "public" },
];

/**
 * Routing module
 */
@NgModule({
  providers: [AuthenticationService, WhenAuthenticated, WhenNotAuthenticated],
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}
```

3. Authentication

```ts
import { AuthenticationService } from "@intellegens/ngz-utils-identity";

/**
 * Some component using authentication service
 */
class SomePageComponent {
  constructor(public _auth: AuthenticationService) {}

  /**
   * Log in
   */
  public async login(username: string, password: string) {
    await this._auth.login(username, password);
  }
  /**
   * Log out
   */
  public async logout() {
    await this._auth.logout();
  }
  /**
   * Get authenticated user information
   */
  public get authenticatedUserInfo(): any {
    return this._auth.info;
  }
  /**
   * Get authenticated user claims
   */
  public get authenticatedUserClaims(): { type: string; value: string }[] {
    return this._auth.claims;
  }
  /**
   * Get authenticated user roles
   */
  public authenticatedUserRoles(): string[] {
    return this._auth.roles;
  }
}
```

## Contributing

This project was generated with [Angular CLI](https://github.com/angular/angular-cli) version 11.1.1.

### Build

Run `ng build` to build the project. The build artifacts will be stored in the `dist/` directory. Use the `--prod` flag for a production build.

### Development server

Run `ng serve` for a dev server. Navigate to `http://localhost:4200/`. The app will automatically reload if you change any of the source files.
