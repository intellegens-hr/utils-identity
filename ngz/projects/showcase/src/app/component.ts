// Showcase application main component
// ----------------------------------------------------------------------------

// Import dependencies
import { Component } from '@angular/core';
import { AuthenticationService } from '@intellegens/ngz-utils-identity';

/**
 * Dummy component
 */
@Component({
  template: `
    <button *ngIf="!_auth.isAuthenticated" (click)="_login()">Login</button>
    <button *ngIf="_auth.isAuthenticated" (click)="_logout()">Logout</button>
  `,
  styleUrls: [],
})
export class SomeComponent {
  constructor(public _auth: AuthenticationService) {}

  public async _login(): Promise<void> {
    await this._auth.login('alice', 'Pass123$');
  }
  public async _logout(): Promise<void> {
    await this._auth.logout();
  }
}
