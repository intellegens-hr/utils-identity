// Main ngx-showcase library module
// ----------------------------------------------------------------------------

// Import modules
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { AppRoutingModule } from './app-routing.module';
import { NgzUtilsIdentityModule } from '@intellegens/ngz-utils-identity';

// Import components
import { AppComponent } from './app.component';

/**
 * Main showcase app module
 */
@NgModule({
  declarations: [AppComponent],
  imports: [BrowserModule, AppRoutingModule, NgzUtilsIdentityModule],
  providers: [],
  bootstrap: [AppComponent],
})
export class AppModule {}
