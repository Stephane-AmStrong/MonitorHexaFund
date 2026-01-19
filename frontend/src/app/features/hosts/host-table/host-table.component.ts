import { Component, computed, effect, inject, input, model, viewChild } from '@angular/core';
import { Router } from '@angular/router';
import { HostDetailedResponse } from '../models/host-detailed-response';
import { MatTable, MatTableModule } from '@angular/material/table';
import { AppStatusIndicatorComponent } from '../../apps/app-status-indicator/app-status-indicator.component';
import { PrefixPictogramComponent } from "../../../shared/components/prefix-pictogram/prefix-pictogram.component";
import { HostAppRow } from './host-app-row';
import { CronDescriptionPipe } from '../../../shared/pipes/cron-description.pipe';
import { HashColorPipe } from "../../../shared/pipes/hash-color.pipe";
import { HostStore } from '../services/host.store';
import { AppStatusStore } from '../../app-statuses/services/app-status.store';

@Component({
  selector: 'host-table',
  imports: [MatTableModule, AppStatusIndicatorComponent, PrefixPictogramComponent, CronDescriptionPipe, HashColorPipe],
  templateUrl: './host-table.component.html',
  styleUrl: './host-table.component.scss',
})
export class HostTableComponent {
  private readonly router = inject(Router);
  private readonly hostStore = inject(HostStore);
  private readonly appStatusStore = inject(AppStatusStore);
  hosts = model.required<HostDetailedResponse[]>();
  readonly dataSource = computed<HostAppRow[]>(() => this.flattenHosts(this.hosts()));
  table = viewChild.required<MatTable<HostAppRow>>(MatTable);


  readonly appColumns: readonly string[] = ['hostName', 'appName', 'version', 'port', 'type', 'cronStartTime', 'cronStopTime'] as const;

  constructor() {
    effect(() => {
      const createdApp = this.hostStore.created();
      if (!createdApp) return;

      this.hosts.update(hosts => [...hosts, createdApp]);
      this.table().renderRows();
    });

    effect(() => {
      const updatedApp = this.hostStore.updated();
      if (!updatedApp) return;

      this.hosts.update(hosts => hosts.map(app => app.id === updatedApp.id ? updatedApp : app));
      this.table().renderRows();
    });

    effect(() => {
      const deletedApp = this.hostStore.deleted();
      if (!deletedApp) return;

      this.hosts.update(hosts => hosts.filter(app => app.id !== deletedApp.id));
      this.table().renderRows();
    });

    effect(() => {
      const createdAppStatus = this.appStatusStore.created();
      if (!createdAppStatus) return;

      this.hosts.update(hosts => hosts.map(host => ({...host, apps: host.apps.map(app => app.id === createdAppStatus.appId ? { ...app, latestStatus: createdAppStatus } : app) })));
      this.table().renderRows();
    });
  }

  onHostClicked(row: HostAppRow, event: MouseEvent): void {
    event.stopPropagation();
    const hostName = row.host.name;
    if (hostName) {
      this.router.navigate(['/hosts', hostName]);
    }
  }

  onAppClicked(row: HostAppRow): void {
    const hostName = row.host.name;
    const appName = row.app.appName;
    if (hostName && appName) {
      this.router.navigate(['/hosts', hostName, 'apps', appName]);
    }
  }

  private flattenHosts(hosts: HostDetailedResponse[]): HostAppRow[] {
    return hosts.flatMap((host) =>
      host.apps.map((app, index) => ({
        host,
        app,
        hostRowSpan: host.apps.length,
        isFirstApp: index === 0,
      }))
    );
  }
}