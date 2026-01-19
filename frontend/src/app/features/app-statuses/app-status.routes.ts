import { Routes } from '@angular/router';
import { APP_ROUTES } from '../../core/constants/app-routes';
import { AppStatusListComponent } from './app-status-list/app-status-list.component';

export const appStatusRoutes: Routes = [
  {
    path: APP_ROUTES.APPS_STATUS.LIST,
    component: AppStatusListComponent,
  },
];
