// Routing module
// Defines application routes and registers them with the app
// ----------------------------------------------------------------------------

// Import modules
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';

// Import components
import { SomeComponent } from './component';

// Import route guards
import {
  WhenAuthenticated,
  WhenNotAuthenticated,
} from '@intellegens/ngz-utils-identity';

// Routes definition
const routes: Routes = [
  {
    path: 'private',
    canActivate: [WhenAuthenticated],
    component: SomeComponent,
  },
  {
    path: 'public',
    canActivate: [WhenNotAuthenticated],
    component: SomeComponent,
  },
  { path: '**', redirectTo: 'public' },
];
WhenAuthenticated.onFailRedirectTo('public');
WhenNotAuthenticated.onFailRedirectTo('private');

/**
 * Routing module
 * Defines application routes and registers them with the app
 */
@NgModule({
  declarations: [SomeComponent],
  imports: [RouterModule.forRoot(routes), CommonModule],
  exports: [RouterModule, CommonModule, SomeComponent],
})
export class AppRoutingModule {}
