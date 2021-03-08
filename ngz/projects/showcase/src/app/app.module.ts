// Main ngx-showcase library module
// ----------------------------------------------------------------------------

// Import modules
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BrowserModule } from '@angular/platform-browser';
import { AppRoutingModule } from './app-routing.module';
import { NgzUtilsIdentityModule, AuthenticationService } from '@intellegens/ngz-utils-identity';
import { environment } from '../environments/environment';

// Import components
import { AppComponent } from './app.component';

/**
 * Main showcase app module
 */
@NgModule({
  declarations: [AppComponent],
  imports: [BrowserModule, CommonModule, AppRoutingModule, NgzUtilsIdentityModule],
  providers: [],
  bootstrap: [AppComponent],
})
export class AppModule {
  constructor(public _auth: AuthenticationService) {
    this._auth.initialize(`${environment.api.url}/auth`, {
      refreshOnLogin: false,
      refreshOnLogout: false,
    });
  }
}
