// Main library module
// ----------------------------------------------------------------------------

// Import and (re)export modules
import { NgModule } from '@angular/core';
import { AuthenticationModule } from './authentication';
export * from './authentication';
const imports = [
  AuthenticationModule
];

/**
 * Authentication module
 */
@NgModule({
  imports,
  exports: [ ...imports ]
})
export class IdentityServerUtilsModule { }
