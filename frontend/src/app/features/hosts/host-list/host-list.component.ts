import { ChangeDetectionStrategy, Component, computed, effect, inject } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatTableModule } from '@angular/material/table';
import { ActivatedRoute, Router } from '@angular/router';
import { toSignal } from '@angular/core/rxjs-interop';

import { ToolbarComponent } from '../../../shared/components/toolbar/toolbar.component';
import { AppQueryParameters } from '../../apps/models/app-query-parameters';
import { HostTableComponent } from '../host-table/host-table.component';
import { AppStore } from '../../apps/services/app.store';
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
  private readonly appStore = inject(AppStore);
  private readonly router = inject(Router);
  constructor() {
    effect(() => this.appStore.queryParameters.set(this.queryParams()));
  }

  private readonly routeResource = toSignal(this.route.queryParams);

  readonly queryParams = computed<AppQueryParameters>(() => {
    const orderBy = this.routeResource()?.['orderBy'];
    let finalOrderBy = 'hostName asc';

    if (orderBy) {
      const hostNameMatch = orderBy.match(/^hostName\s+(asc|desc)/i);
      const direction = hostNameMatch ? hostNameMatch[1].toLowerCase() : 'asc';

      const otherOrderBy = orderBy.replace(/^hostName\s+(asc|desc),?\s*/i, '').trim();
      finalOrderBy = otherOrderBy
        ? `hostName ${direction}, ${otherOrderBy}`
        : `hostName ${direction}`;
    }

    return {
      withHostName: this.routeResource()?.['withHostName'],
      withAppName: this.routeResource()?.['withAppName'],
      withVersion: this.routeResource()?.['withVersion'],
      searchTerm: this.routeResource()?.['searchTerm'],
      orderBy: finalOrderBy,
      page: +this.routeResource()?.['page'] || 1,
      pageSize: +this.routeResource()?.['pageSize'] || 500,
    };
  });

  readonly apps = this.appStore.pagedList;
  readonly pagedListIsLoading = this.appStore.pagedListIsLoading;
  readonly pagingData = this.appStore.pagingData;

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
