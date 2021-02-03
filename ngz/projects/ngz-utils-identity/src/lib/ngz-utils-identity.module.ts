// Main ngz-utils-identity library module
// ----------------------------------------------------------------------------

// Import modules
import { NgModule } from '@angular/core';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';
const modules = [HttpClientModule, RouterModule];

// Import and (re)export injectables
export * from './services';
import {
  AuthenticationService,
  AuthenticationHttpInterceptopProvider,
  WhenAuthenticated,
  WhenNotAuthenticated,
} from './services';
const injectables = [
  AuthenticationService,
  AuthenticationHttpInterceptopProvider,
  WhenAuthenticated,
  WhenNotAuthenticated,
];

/**
 * Main ngx-showcase library module
 */
@NgModule({
  declarations: [],
  providers: [...injectables],
  imports: [...modules],
  exports: [...modules],
})
export class NgzUtilsIdentityModule {}
