// Showcase application main component
// ----------------------------------------------------------------------------

// Import dependencies
import { Component } from '@angular/core';
import { AuthenticationService } from '@intellegens/ngz-utils-identity';

/**
 * Showcase application main component
 */
@Component({
  selector: 'app-root',
  template: `
    <h3>Hello world!</h3>
    <a routerLink="/">Home</a> | <a routerLink="/public">Public</a> |
    <a routerLink="/private">Private</a>
    <hr />
    <div *ngIf="!_auth.isInitialized">... checking auth ...</div>
    <div *ngIf="true || _auth.isInitialized">
      <router-outlet></router-outlet>
    </div>
  `,
  styleUrls: [],
})
export class AppComponent {
  constructor(public _auth: AuthenticationService) {
    this._auth.initialize('https://localhost:5010/auth', {
      refreshOnLogin: false,
      refreshOnLogout: false,
    });
  }
}
