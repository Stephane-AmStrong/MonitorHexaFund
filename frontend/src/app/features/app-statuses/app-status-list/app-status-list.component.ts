import { ChangeDetectionStrategy, Component, computed, effect, inject, model } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog } from '@angular/material/dialog';
import { ActivatedRoute, Router } from '@angular/router';
import { ToolbarComponent } from '../../../shared/components/toolbar/toolbar.component';

import { toSignal } from '@angular/core/rxjs-interop';
import { MatCardModule } from '@angular/material/card';
import { MatTableModule } from '@angular/material/table';
import { AppStatusQueryParameters } from '../models/app-status-query-parameters';
import { AppStatusResponse } from '../models/app-status-response';
import { AppStatusStore } from '../services/app-status.store';
import { AppStatusTableComponent } from '../app-status-table/app-status-table.component';
import { AppStatus } from '../models/app-status-enum';
import { MatPaginatorModule } from '@angular/material/paginator';

@Component({
  selector: 'app-status-list',
  imports: [MatCardModule, MatPaginatorModule, MatTableModule, MatButtonModule, ToolbarComponent, AppStatusTableComponent],
  templateUrl: './app-status-list.component.html',
  styleUrl: './app-status-list.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class AppStatusListComponent {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly appStatusStore = inject(AppStatusStore);

  private dialog = inject(MatDialog);

  constructor() {
    effect(() => this.appStatusStore.queryParameters.set(this.queryParams()));
  }

  private readonly routeResource = toSignal(this.route.queryParams);

  readonly queryParams = computed<AppStatusQueryParameters>(() => ({
    withAppId: this.routeResource()?.['withAppId'],
    ofStatus: this.routeResource()?.['ofStatus'] as AppStatus | undefined,
    recordedBefore: this.routeResource()?.['recordedBefore'],
    recordedAfter: this.routeResource()?.['recordedAfter'],
    searchTerm: this.routeResource()?.['searchTerm'],
    orderBy: this.routeResource()?.['orderBy'],
    page: +this.routeResource()?.['page'] || 1,
    pageSize: +this.routeResource()?.['pageSize'] || 50,
  }));

  readonly appStatuses = this.appStatusStore.pagedList;
  readonly pagedListIsLoading = this.appStatusStore.pagedListIsLoading;
  readonly pagingData = this.appStatusStore.pagingData;

  onAppStatusClicked(appstatus: AppStatusResponse) {
    const appId = appstatus.appId;
    if (appId) {
      this.router.navigate(['/apps', appId, 'appstatuses']);
    }
  }

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
