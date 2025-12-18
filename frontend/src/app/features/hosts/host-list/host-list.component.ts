import { ChangeDetectionStrategy, Component, computed, effect, inject } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatGridListModule } from '@angular/material/grid-list';
import { MatTableModule } from '@angular/material/table';
import { ActivatedRoute } from '@angular/router';
import { toSignal } from '@angular/core/rxjs-interop';

import { ToolbarComponent } from '../../../shared/components/toolbar/toolbar.component';
import { HostQueryParameters } from '../models/host-query-parameters';
import { HostTableComponent } from '../host-table/host-table.component';
import { HostStore } from '../services/host.store';
@Component({
  selector: 'host-list',
  imports: [MatCardModule, MatTableModule, MatGridListModule, MatButtonModule, ToolbarComponent, HostTableComponent],
  templateUrl: './host-list.component.html',
  styleUrl: './host-list.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class HostListComponent {
  private readonly route = inject(ActivatedRoute);
  private readonly hostStore = inject(HostStore);

  constructor() {
    effect(() => this.hostStore.queryParameters.set(this.queryParams()));
  }

  private readonly routeResource = toSignal(this.route.queryParams);

  readonly queryParams = computed<HostQueryParameters>(() => ({
    withName: this.routeResource()?.['withName'],
    searchTerm: this.routeResource()?.['searchTerm'],
    orderBy: this.routeResource()?.['orderBy'],
    page: +this.routeResource()?.['page'] || 1,
    pageSize: +this.routeResource()?.['pageSize'] || 10,
  }));

  readonly hosts = this.hostStore.pagedList;
  readonly isLoading = this.hostStore.isLoading;
}
