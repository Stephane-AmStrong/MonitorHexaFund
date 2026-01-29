import { Component, effect, inject, input, model, viewChild, signal } from '@angular/core';
import { AppResponse } from '../models/app-response';
import { Router } from '@angular/router';
import { MatTable, MatTableModule } from '@angular/material/table';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatSort, MatSortModule, Sort } from '@angular/material/sort';
import { PrefixPictogramComponent } from '../../../shared/components/prefix-pictogram/prefix-pictogram.component';
import { AppStatusIndicatorComponent } from '../app-status-indicator/app-status-indicator.component';
import { CronDescriptionPipe } from '../../../shared/pipes/cron-description.pipe';
import { HashColorPipe } from '../../../shared/pipes/hash-color.pipe';
import { AppStore } from '../services/app.store';
import { AppStatusStore } from '../../app-statuses/services/app-status.store';

@Component({
  selector: 'app-table',
  imports: [
    MatTableModule,
    MatSortModule,
    MatPaginatorModule,
    PrefixPictogramComponent,
    AppStatusIndicatorComponent,
    CronDescriptionPipe,
    HashColorPipe,
  ],
  templateUrl: './app-table.component.html',
  styleUrl: './app-table.component.scss',
})
export class AppTableComponent {
  private readonly router = inject(Router);
  private readonly appStore = inject(AppStore);
  private readonly appStatusStore = inject(AppStatusStore);
  apps = model.required<AppResponse[]>();
  highlightedItemId = signal<string | null>(null);
  deletedItemId = signal<string | null>(null);

  readonly table = viewChild.required<MatTable<AppResponse>>(MatTable);
  readonly sort = viewChild.required<MatSort>(MatSort);

  appColumns = input<string[]>([
    'hostName',
    'appName',
    'version',
    'port',
    'type',
    'cronStartTime',
    'cronStopTime',
  ]);

  constructor() {
    effect(() => {
      const createdApp = this.appStore.created();
      if (!createdApp) return;

      this.apps.update((apps) => [createdApp, ...apps]);
      this.highlightedItemId.set(createdApp.id);
      setTimeout(() => this.highlightedItemId.set(null), 800);
      this.table().renderRows();
    });

    effect(() => {
      const updatedApp = this.appStore.updated();
      if (!updatedApp) return;

      this.apps.update((apps) => [updatedApp, ...apps.filter((app) => app.id !== updatedApp.id)]);
      this.highlightedItemId.set(updatedApp.id);
      setTimeout(() => this.highlightedItemId.set(null), 800);
      this.table().renderRows();
    });

    effect(() => {
      const deletedApp = this.appStore.deleted();
      if (!deletedApp) return;

      this.apps.update((apps) => [deletedApp, ...apps.filter((app) => app.id !== deletedApp.id)]);
      this.deletedItemId.set(deletedApp.id);
      this.table().renderRows();

      setTimeout(() => {
        this.apps.update((apps) => apps.filter((app) => app.id !== deletedApp.id));
        this.deletedItemId.set(null);
        this.table().renderRows();
      }, 3000);
    });

    effect(() => {
      const createdAppStatus = this.appStatusStore.created();
      if (!createdAppStatus) return;

      this.highlightedItemId.set(createdAppStatus.appId);
      setTimeout(() => this.highlightedItemId.set(null), 800);

      this.apps.update((apps) => {
        const app = apps.find((a) => a.id === createdAppStatus.appId);
        if (!app) return apps;

        return [
          { ...app, latestStatus: createdAppStatus },
          ...apps.filter((a) => a.id !== createdAppStatus.appId),
        ];
      });

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
