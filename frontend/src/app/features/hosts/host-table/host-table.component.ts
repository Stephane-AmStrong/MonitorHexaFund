import { Component, computed, effect, inject, input, model, signal, viewChild } from '@angular/core';
import { Router } from '@angular/router';
import { AppResponse } from '../../apps/models/app-response';
import { MatTable, MatTableModule } from '@angular/material/table';
import { AppStatusIndicatorComponent } from '../../apps/app-status-indicator/app-status-indicator.component';
import { PrefixPictogramComponent } from "../../../shared/components/prefix-pictogram/prefix-pictogram.component";
import { HostAppRow } from './host-app-row';
import { CronDescriptionPipe } from '../../../shared/pipes/cron-description.pipe';
import { HashColorPipe } from "../../../shared/pipes/hash-color.pipe";
import { AppStore } from '../../apps/services/app.store';
import { AppStatusStore } from '../../app-statuses/services/app-status.store';
import { MatSort, MatSortModule, Sort } from '@angular/material/sort';

@Component({
  selector: 'host-table',
  imports: [MatTableModule, AppStatusIndicatorComponent, PrefixPictogramComponent, CronDescriptionPipe, HashColorPipe, MatSortModule],
  templateUrl: './host-table.component.html',
  styleUrl: './host-table.component.scss',
})
export class HostTableComponent {
  private readonly router = inject(Router);
  private readonly appStore = inject(AppStore);
  private readonly appStatusStore = inject(AppStatusStore);
  apps = model.required<AppResponse[]>();
  highlightedItemId = signal<string | null>(null);
  deletedItemId = signal<string | null>(null);
  readonly dataSource = computed<HostAppRow[]>(() => this.flattenHosts(this.apps()));
  readonly table = viewChild.required<MatTable<HostAppRow>>(MatTable);
  readonly sort = viewChild.required<MatSort>(MatSort);

  readonly appColumns: readonly string[] = ['hostName', 'appName', 'version', 'port', 'type', 'cronStartTime', 'cronStopTime'] as const;

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

  onHostClicked(row: HostAppRow, event: MouseEvent): void {
    event.stopPropagation();
    const hostName = row.app.hostName;
    if (hostName) {
      this.router.navigate(['/hosts', hostName]);
    }
  }

  onAppClicked(row: HostAppRow): void {
    const hostName = row.app.hostName;
    const appName = row.app.appName;
    if (hostName && appName) {
      this.router.navigate(['/hosts', hostName, 'apps', appName]);
    }
  }

  private flattenHosts(apps: AppResponse[]): HostAppRow[] {
    const grouped = new Map<string, AppResponse[]>();

    apps.forEach(app => {
      if (!grouped.has(app.hostName)) grouped.set(app.hostName, []);
      grouped.get(app.hostName)!.push(app);
    });

    return Array.from(grouped.values()).flatMap(hostApps =>
      hostApps.map((app, index) => ({
        app,
        hostRowSpan: hostApps.length,
        isFirstApp: index === 0,
      }))
    );
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
