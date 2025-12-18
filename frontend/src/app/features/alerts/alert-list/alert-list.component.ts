import { ChangeDetectionStrategy, Component, computed, effect, inject } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog } from '@angular/material/dialog';
import { MatGridListModule } from '@angular/material/grid-list';
import { ActivatedRoute } from '@angular/router';
import { ToolbarComponent } from '../../../shared/components/toolbar/toolbar.component';

import { toSignal } from '@angular/core/rxjs-interop';
import { AlertQueryParameters } from '../models/alert-query-parameters';
import { MatCardModule } from '@angular/material/card';
import { MatTableModule } from '@angular/material/table';
import { AlertTableComponent } from '../alert-table/alert-table.component';
import { AlertStore } from '../services/alert.store';

@Component({
  selector: 'alert-list',
  imports: [MatCardModule, MatGridListModule, MatTableModule, MatButtonModule, ToolbarComponent, AlertTableComponent],
  templateUrl: './alert-list.component.html',
  styleUrl: './alert-list.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class AlertListComponent {
  private readonly route = inject(ActivatedRoute);
  private readonly alertStore = inject(AlertStore);

  private dialog = inject(MatDialog);

  constructor() {
    effect(() => this.alertStore.queryParameters.set(this.queryParams()));
  }

  private readonly routeResource = toSignal(this.route.queryParams);

  readonly queryParams = computed<AlertQueryParameters>(() => ({
    withName: this.routeResource()?.['withName'],
    withAppName: this.routeResource()?.['withAppName'],
    withVersion: this.routeResource()?.['withVersion'],
    searchTerm: this.routeResource()?.['searchTerm'],
    orderBy: this.routeResource()?.['orderBy'],
    page: +this.routeResource()?.['page'] || 1,
    pageSize: +this.routeResource()?.['pageSize'] || 10,
  }));

  readonly alerts = this.alertStore.pagedList;
  readonly isLoading = this.alertStore.isLoading;
}
