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
import { AppStatusIndicatorComponent } from '../app-status-indicator/app-status-indicator.component';
import { PrefixPictogramComponent } from '../../../shared/components/prefix-pictogram/prefix-pictogram.component';
import { CronDescriptionPipe } from '../../../shared/pipes/cron-description.pipe';
import { HashColorPipe } from '../../../shared/pipes/hash-color.pipe';

import { AlertTableComponent } from '../../alerts/alert-table/alert-table.component';
import { ConnectionTableComponent } from '../../connections/connection-table/connection-table.component';
import { AppStatusTableComponent } from '../../app-statuses/app-status-table/app-status-table.component';

import { AppStore } from '../services/app.store';
import { AlertStore } from '../../alerts/services/alert.store';
import { ConnectionStore } from '../../connections/services/connection.store';

import { AlertQueryParameters } from '../../alerts/models/alert-query-parameters';
import { ConnectionQueryParameters } from '../../connections/models/connection-query-parameters';
import { AppStatusStore } from '../../app-statuses/services/app-status.store';
import { AppStatusQueryParameters } from '../../app-statuses/models/app-status-query-parameters';

@Component({
  selector: 'app-detail',
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
    AppStatusIndicatorComponent,
    PrefixPictogramComponent,
    CronDescriptionPipe,
    HashColorPipe,
    AlertTableComponent,
    ConnectionTableComponent,
    AppStatusTableComponent,
  ],
  templateUrl: './app-detail.component.html',
  styleUrl: './app-detail.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class AppDetailComponent {
  private readonly route = inject(ActivatedRoute);
  private readonly alertStore = inject(AlertStore);
  private readonly connectionStore = inject(ConnectionStore);
  private readonly appStore = inject(AppStore);
  private readonly appStatusStore = inject(AppStatusStore);

  private readonly routeResource = toSignal(this.route.params);

  readonly selectedId = computed<[string, string]>(() => [
    this.routeResource()?.['hostName'],
    this.routeResource()?.['appName'],
  ]);

  readonly appDetail = this.appStore.selected;
  readonly isLoading = this.appStore.isLoading;

  readonly tags = computed<string[]>(() => this.appDetail()?.tags || []);

  readonly alerts = this.alertStore.pagedList;
  readonly connections = this.connectionStore.pagedList;
  readonly appStatuses = this.appStatusStore.pagedList;

  private readonly alertQueryParams = computed<AlertQueryParameters>(() => ({
    withAppId: this.appDetail()?.id,
    orderBy: 'occurredAt desc',
  }));

  private readonly appStatusQueryParams = computed<AppStatusQueryParameters>(() => ({
    withAppId: this.appDetail()?.id,
    orderBy: 'recordedAt desc',
  }));

  private readonly connectionQueryParams = computed<ConnectionQueryParameters>(() => ({ withAppId: this.appDetail()?.id}));

  readonly appStatusColumns: string[] = ['status', 'recordedAt'];

  constructor() {
    effect(() => { this.appStore.selectedId.set(this.selectedId()); });
    effect(() => { this.alertStore.queryParameters.set(this.alertQueryParams()); });
    effect(() => { this.connectionStore.queryParameters.set(this.connectionQueryParams()); });
    effect(() => { this.appStatusStore.queryParameters.set(this.appStatusQueryParams()); });
  }
}
