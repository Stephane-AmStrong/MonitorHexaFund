import { ChangeDetectionStrategy, Component, computed, effect, inject } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog } from '@angular/material/dialog';
import { ActivatedRoute, Router } from '@angular/router';
import { ToolbarComponent } from '../../../shared/components/toolbar/toolbar.component';

import { toSignal } from '@angular/core/rxjs-interop';
import { ClientQueryParameters } from '../models/client-query-parameters';
import { MatCardModule } from '@angular/material/card';
import { ClientStore } from '../services/client.store';
import { ClientTableComponent } from '../client-table/client-table.component';
import { MatPaginatorModule } from '@angular/material/paginator';

@Component({
  selector: 'client-list',
  imports: [MatCardModule, MatPaginatorModule, MatButtonModule, ToolbarComponent, ClientTableComponent],
  templateUrl: './client-list.component.html',
  styleUrl: './client-list.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ClientListComponent {
  private readonly route = inject(ActivatedRoute);
  private readonly clientStore = inject(ClientStore);
  private readonly router = inject(Router);

  private dialog = inject(MatDialog);

  constructor() {
    effect(() => this.clientStore.queryParameters.set(this.queryParams()));
  }

  private readonly routeResource = toSignal(this.route.queryParams);

  readonly queryParams = computed<ClientQueryParameters>(() => ({
    withGaia: this.routeResource()?.['withGaia'],
    withLogin: this.routeResource()?.['withLogin'],
    searchTerm: this.routeResource()?.['searchTerm'],
    orderBy: this.routeResource()?.['orderBy'],
    page: +this.routeResource()?.['page'] || 1,
    pageSize: +this.routeResource()?.['pageSize'] || 50,
  }));

  readonly clients = this.clientStore.pagedList;
  readonly pagedListIsLoading = this.clientStore.pagedListIsLoading;
  readonly pagingData = this.clientStore.pagingData;

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
