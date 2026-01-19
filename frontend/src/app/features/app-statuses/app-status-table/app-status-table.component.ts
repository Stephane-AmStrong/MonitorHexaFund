import { Component, effect, inject, input, model, viewChild } from '@angular/core';
import { AppStatusResponse } from '../models/app-status-response';
import { Router } from '@angular/router';
import { MatTable, MatTableModule } from '@angular/material/table';
import { AppStatusIndicatorComponent } from "../../apps/app-status-indicator/app-status-indicator.component";
import { AppStatusStore } from '../services/app-status.store';

@Component({
  selector: 'app-status-table',
  imports: [MatTableModule, AppStatusIndicatorComponent],
  templateUrl: './app-status-table.component.html',
  styleUrl: './app-status-table.component.scss'
})
export class AppStatusTableComponent {
  private readonly router = inject(Router);
  private readonly appStatusStore = inject(AppStatusStore);
  appStatuses = model.required<AppStatusResponse[]>();

  table = viewChild.required<MatTable<AppStatusResponse>>(MatTable);

  constructor() {
    effect(() => {
      const createdAppStatus = this.appStatusStore.created();
      if (!createdAppStatus) return;

      this.appStatuses.update(appStatuses => [...appStatuses, createdAppStatus]);
      this.table().renderRows();
    });

    effect(() => {
      const updatedAppStatus = this.appStatusStore.updated();
      if (!updatedAppStatus) return;

      this.appStatuses.update(appStatuses => appStatuses.map(appStatus => appStatus.id === updatedAppStatus.id ? updatedAppStatus : appStatus));
      this.table().renderRows();
    });

    effect(() => {
      const deletedAppStatus = this.appStatusStore.deleted();
      if (!deletedAppStatus) return;

      this.appStatuses.update(appStatuses => appStatuses.filter(appStatus => appStatus.id !== deletedAppStatus.id));
      this.table().renderRows();
    });
  }

  readonly appStatusColumns = input<string[]>(['app', 'recordedAt']);

  onAppStatusClicked(appStatus: AppStatusResponse) {
    const splitedAppId = appStatus?.appId?.split(';');

    const hostName = splitedAppId[0];
    const appName = splitedAppId[1];

    if (hostName && appName) {
      this.router.navigate(['/hosts', hostName, 'apps', appName]);
    }
  }
}
