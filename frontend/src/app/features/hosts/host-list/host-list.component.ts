import { ChangeDetectionStrategy, Component, computed, effect, inject } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatTableModule } from '@angular/material/table';
import { ActivatedRoute, Router } from '@angular/router';
import { toSignal } from '@angular/core/rxjs-interop';

import { ToolbarComponent } from '../../../shared/components/toolbar/toolbar.component';
import { HostQueryParameters } from '../models/host-query-parameters';
import { HostTableComponent } from '../host-table/host-table.component';
import { HostStore } from '../services/host.store';
import { MatPaginatorModule } from '@angular/material/paginator';
@Component({
  selector: 'host-list',
  imports: [MatCardModule, MatTableModule, MatPaginatorModule, MatButtonModule, ToolbarComponent, HostTableComponent],
  templateUrl: './host-list.component.html',
  styleUrl: './host-list.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class HostListComponent {
  private readonly route = inject(ActivatedRoute);
  private readonly hostStore = inject(HostStore);
  private readonly router = inject(Router);
  constructor() {
    effect(() => this.hostStore.queryParameters.set(this.queryParams()));
  }

  private readonly routeResource = toSignal(this.route.queryParams);

  readonly queryParams = computed<HostQueryParameters>(() => ({
    withName: this.routeResource()?.['withName'],
    searchTerm: this.routeResource()?.['searchTerm'],
    orderBy: this.routeResource()?.['orderBy'],
    page: +this.routeResource()?.['page'] || 1,
    pageSize: +this.routeResource()?.['pageSize'] || 50,
  }));

  readonly hosts = this.hostStore.pagedList;
  readonly pagedListIsLoading = this.hostStore.pagedListIsLoading;
  readonly pagingData = this.hostStore.pagingData;

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
