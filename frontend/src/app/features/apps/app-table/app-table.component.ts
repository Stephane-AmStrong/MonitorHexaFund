import { Component, effect, inject, input, model, viewChild } from '@angular/core';
import { AppResponse } from '../models/app-response';
import { Router } from '@angular/router';
import { MatTable, MatTableModule } from '@angular/material/table';
import {MatPaginatorModule} from '@angular/material/paginator';
import {MatSort, MatSortModule, Sort} from '@angular/material/sort';
import { PrefixPictogramComponent } from '../../../shared/components/prefix-pictogram/prefix-pictogram.component';
import { AppStatusIndicatorComponent } from '../app-status-indicator/app-status-indicator.component';
import { CronDescriptionPipe } from "../../../shared/pipes/cron-description.pipe";
import { HashColorPipe } from "../../../shared/pipes/hash-color.pipe";
import { AppStore } from '../services/app.store';
import { AppStatusStore } from '../../app-statuses/services/app-status.store';

@Component({
  selector: 'app-table',
  imports: [MatTableModule, MatSortModule, MatPaginatorModule, PrefixPictogramComponent, AppStatusIndicatorComponent, CronDescriptionPipe, HashColorPipe],
  templateUrl: './app-table.component.html',
  styleUrl: './app-table.component.scss',
})
export class AppTableComponent {
  private readonly router = inject(Router);
  private readonly appStore = inject(AppStore);
  private readonly appStatusStore = inject(AppStatusStore);
  apps = model.required<AppResponse[]>();

  readonly table = viewChild.required<MatTable<AppResponse>>(MatTable);
  readonly sort = viewChild.required<MatSort>(MatSort);

  appColumns = input<string[]>(['hostName', 'appName', 'version', 'port', 'type', 'cronStartTime', 'cronStopTime'])

  constructor() {
    effect(() => {
      const createdApp = this.appStore.created();
      if (!createdApp) return;

      this.apps.update(apps => [createdApp, ...apps]);
      this.table().renderRows();
    });

    effect(() => {
      const updatedApp = this.appStore.updated();
      if (!updatedApp) return;

      this.apps.update(apps => apps.map(app => app.id === updatedApp.id ? updatedApp : app));
      this.table().renderRows();
    });

    effect(() => {
      const deletedApp = this.appStore.deleted();
      if (!deletedApp) return;

      this.apps.update(apps => apps.filter(app => app.id !== deletedApp.id));
      this.table().renderRows();
    });

    effect(() => {
      const createdAppStatus = this.appStatusStore.created();
      if (!createdAppStatus) return;

      this.apps.update(apps => apps.map(app => app.id === createdAppStatus.appId ? { ...app, latestStatus: createdAppStatus } : app));
      this.table().renderRows();
    });
  }

  onAppClicked(app: AppResponse) {
    const hostName = app.hostName;
    const appName = app.appName;
    if (hostName && appName) {
      this.router.navigate(['/hosts', hostName, 'apps', appName]);
    }
  }

  onSortChange($event: Sort) {
     if ($event.active && $event.direction) {
      this.router.navigate([], {
        queryParams: {
          orderBy: `${$event.active} ${$event.direction}`,
        },
        queryParamsHandling: 'merge',
      });
    }
  }
}
