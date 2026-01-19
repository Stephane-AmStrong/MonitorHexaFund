import { Routes } from '@angular/router';
import { AppListComponent } from './app-list/app-list.component';
import { APP_ROUTES } from '../../core/constants/app-routes';

export const appRoutes: Routes = [
  {
    path: APP_ROUTES.APPS.LIST,
    component: AppListComponent,
  },
];
