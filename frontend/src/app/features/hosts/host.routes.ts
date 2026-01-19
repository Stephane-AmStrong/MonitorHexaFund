import { Routes } from '@angular/router';
import { HostListComponent } from './host-list/host-list.component';
import { APP_ROUTES } from '../../core/constants/app-routes';
import { AppDetailComponent } from '../apps/app-detail/app-detail.component';
import { HostDetailComponent } from './host-detail/host-detail.component';

export const hostRoutes: Routes = [
  {
    path: APP_ROUTES.HOSTS.LIST,
    component: HostListComponent,
  },
  {
    path: APP_ROUTES.HOSTS.DETAIL,
    component: HostDetailComponent,
  },
  {
    path: APP_ROUTES.HOSTS.APPS_REDIRECT,
    redirectTo: APP_ROUTES.HOSTS.DETAIL,
    pathMatch: 'full'
  },
  {
    path: APP_ROUTES.HOSTS.APP_BY_HOST,
    component: AppDetailComponent,
  }
];
