import { ChangeDetectionStrategy, Component, computed, effect, inject } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog } from '@angular/material/dialog';
import { MatGridListModule } from '@angular/material/grid-list';
import { ActivatedRoute } from '@angular/router';
import { ToolbarComponent } from '../../../shared/components/toolbar/toolbar.component';

import { toSignal } from '@angular/core/rxjs-interop';
import { ServerQueryParameters } from '../models/server-query-parameters';
import { MatCardModule } from '@angular/material/card';
import { ServerStore } from '../services/server.store';
import { ServerTableComponent } from '../server-table/server-table.component';

@Component({
  selector: 'server-list',
  imports: [
    MatCardModule,
    MatGridListModule,
    MatButtonModule,
    ToolbarComponent,
    ServerTableComponent,
  ],
  templateUrl: './server-list.component.html',
  styleUrl: './server-list.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ServerListComponent {
  private readonly route = inject(ActivatedRoute);
  private readonly serverStore = inject(ServerStore);

  private dialog = inject(MatDialog);

  constructor() {
    effect(() => this.serverStore.queryParameters.set(this.queryParams()));
  }

  private readonly routeResource = toSignal(this.route.queryParams);

  readonly queryParams = computed<ServerQueryParameters>(() => ({
    withHostName: this.routeResource()?.['withHostName'],
    withAppName: this.routeResource()?.['withAppName'],
    withVersion: this.routeResource()?.['withVersion'],
    searchTerm: this.routeResource()?.['searchTerm'],
    orderBy: this.routeResource()?.['orderBy'],
    page: +(this.routeResource()?.['page'] ?? 1),
    pageSize: +(this.routeResource()?.['pageSize'] ?? 10),
  }));

  readonly servers = this.serverStore.pagedList;
  readonly isLoading = this.serverStore.isLoading;
}
