import { ChangeDetectionStrategy, Component, computed, effect, inject, model } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog } from '@angular/material/dialog';
import { MatGridListModule } from '@angular/material/grid-list';
import { ConnectionResponse } from '../models/connection-response';
import { ActivatedRoute, Router } from '@angular/router';
import { ToolbarComponent } from '../../../shared/components/toolbar/toolbar.component';

import { toSignal } from '@angular/core/rxjs-interop';
import { ConnectionQueryParameters } from '../models/connection-query-parameters';
import { MatCardModule } from '@angular/material/card';
import { ConnectionService } from '../services/connection.service';
import { MatTableModule } from '@angular/material/table';
import { ConnectionTableComponent } from '../connection-table/connection-table.component';
import { ConnectionStore } from '../services/connection.store';

@Component({
  selector: 'connection-list',
  imports: [MatCardModule, MatGridListModule, MatTableModule, MatButtonModule, ToolbarComponent, ConnectionTableComponent],
  templateUrl: './connection-list.component.html',
  styleUrl: './connection-list.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ConnectionListComponent {
  private readonly route = inject(ActivatedRoute);
  private readonly connectionStore = inject(ConnectionStore);

  private dialog = inject(MatDialog);

  constructor() {
    effect(() => this.connectionStore.queryParameters.set(this.queryParams()));
  }

  private readonly routeResource = toSignal(this.route.queryParams);
  
  readonly queryParams = computed<ConnectionQueryParameters>(() => ({
    withName: this.routeResource()?.['withName'],
    withAppName: this.routeResource()?.['withAppName'],
    withVersion: this.routeResource()?.['withVersion'],
    searchTerm: this.routeResource()?.['searchTerm'],
    orderBy: this.routeResource()?.['orderBy'],
    page: +this.routeResource()?.['page'] || 1,
    pageSize: +this.routeResource()?.['pageSize'] || 10,
  }));

  readonly connections = this.connectionStore.pagedList;
  readonly isLoading = this.connectionStore.isLoading;
}
