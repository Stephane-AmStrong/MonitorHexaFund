import { ChangeDetectionStrategy, Component, computed, effect, inject } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { ActivatedRoute } from '@angular/router';
import { MatTabsModule } from '@angular/material/tabs';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatGridListModule } from '@angular/material/grid-list';
import { MatDividerModule } from '@angular/material/divider';
import { MatChipsModule } from '@angular/material/chips';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatListModule } from '@angular/material/list';
import { DatePipe } from '@angular/common';

import { ToolbarComponent } from '../../../shared/components/toolbar/toolbar.component';
import { DetailCardComponent } from '../../../shared/components/detail-card/detail-card.component';
import { ServerStatusIndicatorComponent } from '../server-status-indicator/server-status-indicator.component';
import { PrefixPictogramComponent } from '../../../shared/components/prefix-pictogram/prefix-pictogram.component';
import { CronDescriptionPipe } from '../../../shared/pipes/cron-description.pipe';
import { HashColorPipe } from '../../../shared/pipes/hash-color.pipe';

import { AlertTableComponent } from '../../alerts/alert-table/alert-table.component';
import { ConnectionTableComponent } from '../../connections/connection-table/connection-table.component';
import { ServerStatusTableComponent } from '../../server-statuses/server-status-table/server-status-table.component';

import { ServerStore } from '../services/server.store';
import { AlertStore } from '../../alerts/services/alert.store';
import { ConnectionStore } from '../../connections/services/connection.store';

import { AlertQueryParameters } from '../../alerts/models/alert-query-parameters';
import { ConnectionQueryParameters } from '../../connections/models/connection-query-parameters';
import { ServerStatusResponse } from '../../server-statuses/models/server-status-response';

@Component({
  selector: 'server-detail',
  imports: [
    MatTabsModule,
    MatCardModule,
    MatIconModule,
    MatGridListModule,
    MatDividerModule,
    MatChipsModule,
    MatProgressSpinnerModule,
    MatListModule,
    DatePipe,
    ToolbarComponent,
    DetailCardComponent,
    ServerStatusIndicatorComponent,
    PrefixPictogramComponent,
    CronDescriptionPipe,
    HashColorPipe,
    AlertTableComponent,
    ConnectionTableComponent,
    ServerStatusTableComponent,
  ],
  templateUrl: './server-detail.component.html',
  styleUrl: './server-detail.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ServerDetailComponent {
  private readonly route = inject(ActivatedRoute);
  private readonly serverStore = inject(ServerStore);
  private readonly alertStore = inject(AlertStore);
  private readonly connectionStore = inject(ConnectionStore);

  private readonly routeResource = toSignal(this.route.params);

  readonly selectedId = computed<[string, string]>(() => [
    this.routeResource()?.['hostName'],
    this.routeResource()?.['serverName'],
  ]);

  readonly serverDetail = this.serverStore.selected;
  readonly isLoading = this.serverStore.isLoading;

  readonly latestStatus = computed<ServerStatusResponse | null>(() => this.serverDetail()?.statuses[0] || null);
  readonly statuses = computed<ServerStatusResponse[]>(() => this.serverDetail()?.statuses || []);
  readonly tags = computed<string[]>(() => this.serverDetail()?.tags || []);

  readonly alerts = this.alertStore.pagedList;
  readonly connections = this.connectionStore.pagedList;

  private readonly alertQueryParams = computed<AlertQueryParameters>(() => ({
    withServerId: this.serverDetail()?.id,
    orderBy: 'occurredAt desc',
  }));

  private readonly connectionQueryParams = computed<ConnectionQueryParameters>(() => ({ withServerId: this.serverDetail()?.id}));

  readonly serverStatusColumns: string[] = ['status', 'recordedAt'];

  constructor() {
    effect(() => { this.serverStore.selectedId.set(this.selectedId()); });
    effect(() => { this.alertStore.queryParameters.set(this.alertQueryParams()); });
    effect(() => { this.connectionStore.queryParameters.set(this.connectionQueryParams()); });
  }
}
