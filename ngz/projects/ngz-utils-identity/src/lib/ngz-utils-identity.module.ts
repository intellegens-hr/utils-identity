// Main ngz-utils-identity library module
// ----------------------------------------------------------------------------

// Import modules
import { NgModule } from '@angular/core';
import { HttpClientModule } from '@angular/common/http';
const modules = [HttpClientModule];

// Import and (re)export injectables
export * from './services';
import {
  AuthenticationService,
  AuthenticationServiceInterceptopProvider,
} from './services';
const injectables = [
  AuthenticationService,
  AuthenticationServiceInterceptopProvider,
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
