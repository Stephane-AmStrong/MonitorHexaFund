import { Component, effect, inject, input, model, signal, viewChild } from '@angular/core';
import { AppStatusResponse } from '../models/app-status-response';
import { Router } from '@angular/router';
import { MatTable, MatTableModule } from '@angular/material/table';
import { AppStatusIndicatorComponent } from "../../apps/app-status-indicator/app-status-indicator.component";
import { AppStatusStore } from '../services/app-status.store';
import { DatePipe } from '@angular/common';
import { MatSort, MatSortModule, Sort } from '@angular/material/sort';

@Component({
  selector: 'app-status-table',
  imports: [MatTableModule, AppStatusIndicatorComponent, DatePipe, MatSortModule],
  templateUrl: './app-status-table.component.html',
  styleUrl: './app-status-table.component.scss'
})
export class AppStatusTableComponent {
  private readonly router = inject(Router);
  private readonly appStatusStore = inject(AppStatusStore);
  appStatuses = model.required<AppStatusResponse[]>();
  highlightedItemId = signal<string | null>(null);
  deletedItemId = signal<string | null>(null);

  readonly table = viewChild.required<MatTable<AppStatusResponse>>(MatTable);
  readonly sort = viewChild.required<MatSort>(MatSort);

  constructor() {
    effect(() => {
      const createdAppStatus = this.appStatusStore.created();
      if (!createdAppStatus) return;

      this.appStatuses.update(appStatuses => [createdAppStatus, ...appStatuses]);
      this.highlightedItemId.set(createdAppStatus.id);
      setTimeout(() => this.highlightedItemId.set(null), 800);
      this.table().renderRows();
    });

    effect(() => {
      const updatedAppStatus = this.appStatusStore.updated();
      if (!updatedAppStatus) return;

      this.appStatuses.update(appStatuses => {
        const filtered = appStatuses.filter(appStatus => appStatus.id !== updatedAppStatus.id);
        return [updatedAppStatus, ...filtered];
      });
      this.highlightedItemId.set(updatedAppStatus.id);
      setTimeout(() => this.highlightedItemId.set(null), 800);
      this.table().renderRows();
    });

    effect(() => {
      const deletedAppStatus = this.appStatusStore.deleted();
      if (!deletedAppStatus) return;

      this.appStatuses.update(appStatuses => [deletedAppStatus, ...appStatuses.filter(appStatus => appStatus.id !== deletedAppStatus.id)]);
      this.deletedItemId.set(deletedAppStatus.id);
      this.table().renderRows();

      setTimeout(() => {
        this.appStatuses.update(appStatuses => appStatuses.filter(appStatus => appStatus.id !== deletedAppStatus.id));
        this.deletedItemId.set(null);
        this.table().renderRows();
      }, 3000);
    });
  }

  readonly appStatusColumns = input<string[]>(['appId', 'recordedAt']);

  onAppStatusClicked(appStatus: AppStatusResponse) {
    const splitedAppId = appStatus?.appId?.split(';');

    const hostName = splitedAppId[0];
    const appName = splitedAppId[1];

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
