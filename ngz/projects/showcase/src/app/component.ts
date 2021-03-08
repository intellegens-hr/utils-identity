// Showcase application main component
// ----------------------------------------------------------------------------

// Import dependencies
import { Component } from '@angular/core';
import { AuthenticationService } from '@intellegens/ngz-utils-identity';
import { environment } from '../environments/environment';

/**
 * Dummy component
 */
@Component({
  template: `
    <ng-container *ngIf="!_auth.isAuthenticated">
      <button (click)="_login()">Login</button>
      <hr />
      ... as "{{ _getApi().username }}"/"{{ _getApi().password }}"
    </ng-container>
    <ng-container *ngIf="_auth.isAuthenticated">
      <button (click)="_logout()">Logout</button>
      <hr />
      ... logged in as: <br />
      <textarea style="width: 100%;">{{ _toJson(_auth.info) }}</textarea>
      ... claims: <br />
      <textarea style="width: 100%;">{{ _toJson(_auth.claims) }}</textarea>
      ... roles: <br />
      <textarea style="width: 100%;">{{ _toJson(_auth.roles) }}</textarea>
    </ng-container>
  `,
  styleUrls: [],
})
export class SomeComponent {
  constructor(public _auth: AuthenticationService) {}

  public _getApi() {
    return environment.api;
  }
  public _toJson(data: any) {
    return JSON.stringify(data, null, 2);
  }

  public async _login(): Promise<void> {
    await this._auth.login(environment.api.username, environment.api.password);
  }
  public async _logout(): Promise<void> {
    await this._auth.logout();
  }
}
