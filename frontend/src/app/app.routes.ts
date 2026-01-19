import { Routes } from '@angular/router';
import { LayoutComponent } from './features/dashboard/layout/layout.component';
import { APP_ROUTES } from './core/constants/app-routes';
import { PageNotFoundComponent } from './features/errors/page-not-found/page-not-found.component';

export const routes: Routes = [
  {
    path: APP_ROUTES.ROOT,
    pathMatch: 'full',
    redirectTo: '/dashboard',
  },
  {
    path: APP_ROUTES.DASHBOARD,
    component: LayoutComponent,
  },
  {
    path: APP_ROUTES.ALERTS.ROOT,
    loadChildren: () =>
      import('./features/alerts/alert.routes').then((r) => r.alertRoutes),
  },
  {
    path: APP_ROUTES.CLIENTS.ROOT,
    loadChildren: () =>
      import('./features/clients/client.routes').then((r) => r.clientRoutes),
  },
  {
    path: APP_ROUTES.CONNECTIONS.ROOT,
    loadChildren: () =>
      import('./features/connections/connection.routes').then((r) => r.connectionRoutes),
  },
  {
    path: APP_ROUTES.ERRORS.ROOT,
    loadChildren: () =>
      import('./features/errors/error.routes').then((r) => r.errorRoutes),
  },
  {
    path: APP_ROUTES.HOSTS.ROOT,
    loadChildren: () =>
      import('./features/hosts/host.routes').then((r) => r.hostRoutes),
  },
  {
    path: APP_ROUTES.APPS.ROOT,
    loadChildren: () =>
      import('./features/apps/app.routes').then((r) => r.appRoutes),
  },
  {
    path: APP_ROUTES.APPS_STATUS.ROOT,
    loadChildren: () =>
      import('./features/app-statuses/app-status.routes').then((r) => r.appStatusRoutes),
  },
  {
    path: APP_ROUTES.NOT_FOUND,
    component: PageNotFoundComponent,
  },
];
