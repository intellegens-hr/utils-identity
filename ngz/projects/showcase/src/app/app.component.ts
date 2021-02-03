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
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
})
export class AppComponent {
  constructor(private _auth: AuthenticationService) {
    // Initialize authentication
    this._auth.initialize('https://localhost:5010/auth');
  }

  public async _login() {
    const info = await this._auth.login('alice', 'Pass123$');
    console.log(info);
  }
}
