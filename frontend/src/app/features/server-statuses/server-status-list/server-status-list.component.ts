import { ChangeDetectionStrategy, Component, computed, effect, inject, model } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog } from '@angular/material/dialog';
import { MatGridListModule } from '@angular/material/grid-list';
import { ActivatedRoute, Router } from '@angular/router';
import { ToolbarComponent } from '../../../shared/components/toolbar/toolbar.component';

import { toSignal } from '@angular/core/rxjs-interop';
import { MatCardModule } from '@angular/material/card';
import { MatTableModule } from '@angular/material/table';
import { ServerStatusQueryParameters } from '../models/server-status-query-parameters';
import { ServerStatusResponse } from '../models/server-status-response';
import { ServerStatusStore } from '../services/server-status.store';
import { ServerStatusTableComponent } from '../server-status-table/server-status-table.component';
import { ServerStatus } from '../models/server-status-enum';

@Component({
  selector: 'server-status-list',
  imports: [MatCardModule, MatGridListModule, MatTableModule, MatButtonModule, ToolbarComponent, ServerStatusTableComponent],
  templateUrl: './server-status-list.component.html',
  styleUrl: './server-status-list.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ServerStatusListComponent {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly serverStatusStore = inject(ServerStatusStore);

  private dialog = inject(MatDialog);

  constructor() {
    effect(() => this.serverStatusStore.queryParameters.set(this.queryParams()));
  }

  private readonly routeResource = toSignal(this.route.queryParams);

  readonly queryParams = computed<ServerStatusQueryParameters>(() => ({
    withServerId: this.routeResource()?.['withServerId'],
    ofStatus: this.routeResource()?.['ofStatus'] as ServerStatus | undefined,
    recordedBefore: this.routeResource()?.['recordedBefore'],
    recordedAfter: this.routeResource()?.['recordedAfter'],
    searchTerm: this.routeResource()?.['searchTerm'],
    orderBy: this.routeResource()?.['orderBy'],
    page: +this.routeResource()?.['page'] || 1,
    pageSize: +this.routeResource()?.['pageSize'] || 10,
  }));

  readonly serverStatuses = this.serverStatusStore.pagedList;
  readonly isLoading = this.serverStatusStore.isLoading;

  readonly serverStatusColumns: readonly string[] = ['hostName', 'appName', 'version', 'port', 'type', 'cronStartTime', 'cronStopTime'] as const;

  onServerStatusClicked(serverstatus: ServerStatusResponse) {
    const serverId = serverstatus.serverId;
    if (serverId) {
      this.router.navigate(['/servers', serverId, 'serverstatuses']);
    }
  }
}
