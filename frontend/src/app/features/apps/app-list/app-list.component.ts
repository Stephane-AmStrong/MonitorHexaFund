import { ChangeDetectionStrategy, Component, computed, effect, inject, viewChild } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog } from '@angular/material/dialog';
import { ActivatedRoute, Router } from '@angular/router';
import { ToolbarComponent } from '../../../shared/components/toolbar/toolbar.component';

import { toSignal } from '@angular/core/rxjs-interop';
import { AppQueryParameters } from '../models/app-query-parameters';
import { MatCardModule } from '@angular/material/card';
import { AppStore } from '../services/app.store';
import { AppTableComponent } from '../app-table/app-table.component';
import { MatPaginator, MatPaginatorModule } from '@angular/material/paginator';
import { MatSort, MatSortModule } from '@angular/material/sort';

@Component({
  selector: 'app-list',
  imports: [
    MatCardModule,
    MatSortModule,
    MatPaginatorModule,
    MatButtonModule,
    ToolbarComponent,
    AppTableComponent,
  ],
  templateUrl: './app-list.component.html',
  styleUrl: './app-list.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class AppListComponent {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly appStore = inject(AppStore);
  readonly paginator = viewChild.required<MatPaginator>(MatPaginator);
  readonly sort = viewChild.required<MatSort>(MatSort);

  private dialog = inject(MatDialog);

  constructor() {
    effect(() => this.appStore.queryParameters.set(this.queryParams()));
  }

  private readonly routeResource = toSignal(this.route.queryParams);

  readonly queryParams = computed<AppQueryParameters>(() => ({
    withHostName: this.routeResource()?.['withHostName'],
    withAppName: this.routeResource()?.['withAppName'],
    withVersion: this.routeResource()?.['withVersion'],
    searchTerm: this.routeResource()?.['searchTerm'],
    orderBy: this.routeResource()?.['orderBy'],
    page: +this.routeResource()?.['page'] || 1,
    pageSize: +this.routeResource()?.['pageSize'] || 200,
  }));

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
