import { ChangeDetectionStrategy, Component, computed, effect, inject, model } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog } from '@angular/material/dialog';
import { ActivatedRoute, Router} from '@angular/router';
import { ToolbarComponent } from '../../../shared/components/toolbar/toolbar.component';

import { toSignal } from '@angular/core/rxjs-interop';
import { ConnectionQueryParameters } from '../models/connection-query-parameters';
import { MatCardModule } from '@angular/material/card';
import { MatTableModule } from '@angular/material/table';
import { ConnectionTableComponent } from '../connection-table/connection-table.component';
import { ConnectionStore } from '../services/connection.store';
import { MatPaginatorModule } from '@angular/material/paginator';

@Component({
  selector: 'connection-list',
  imports: [MatCardModule, MatPaginatorModule, MatTableModule, MatButtonModule, ToolbarComponent, ConnectionTableComponent],
  templateUrl: './connection-list.component.html',
  styleUrl: './connection-list.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ConnectionListComponent {
  private readonly route = inject(ActivatedRoute);
  private readonly connectionStore = inject(ConnectionStore);
  private readonly router = inject(Router);
  private dialog = inject(MatDialog);

  constructor() {
    effect(() => this.connectionStore.queryParameters.set(this.queryParams()));
  }

  private readonly routeResource = toSignal(this.route.queryParams);

    readonly queryParams = computed<ConnectionQueryParameters>(() => ({
    withClientGaia: this.routeResource()?.['withClientGaia'],
    withAppId: this.routeResource()?.['withAppId'],
    establishedBefore: this.routeResource()?.['establishedBefore'],
    establishedAt: this.routeResource()?.['establishedAt'],
    establishedAfter: this.routeResource()?.['establishedAfter'],
    terminatedBefore: this.routeResource()?.['terminatedBefore'],
    terminatedAt: this.routeResource()?.['terminatedAt'],
    terminatedAfter: this.routeResource()?.['terminatedAfter'],
    searchTerm: this.routeResource()?.['searchTerm'],
    orderBy: this.routeResource()?.['orderBy'],
    page: +this.routeResource()?.['page'] || 1,
    pageSize: +this.routeResource()?.['pageSize'] || 50,
  }));

  readonly connections = this.connectionStore.pagedList;
  readonly pagedListIsLoading = this.connectionStore.pagedListIsLoading;
  readonly pagingData = this.connectionStore.pagingData;

  onPageEvent(event: { pageIndex: number; pageSize: number }) {
    this.router.navigate([], {
      queryParams: {
        page: event.pageIndex + 1,
        pageSize: event.pageSize,
       },
      queryParamsHandling: 'merge',
    });
  }
}
