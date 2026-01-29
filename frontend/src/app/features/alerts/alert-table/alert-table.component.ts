import { Component, effect, inject, model, signal, viewChild } from '@angular/core';
import { AlertResponse } from '../models/alert-response';
import { Router } from '@angular/router';
import { MatTable, MatTableModule } from '@angular/material/table';
import { AlertStore } from '../services/alert.store';
import { DatePipe } from '@angular/common';
import { MatSort, MatSortModule, Sort } from '@angular/material/sort';

@Component({
  selector: 'alert-table',
  imports: [MatTableModule, DatePipe, MatSortModule],
  templateUrl: './alert-table.component.html',
  styleUrl: './alert-table.component.scss'
})
export class AlertTableComponent {
  private readonly router = inject(Router);
  private readonly alertStore = inject(AlertStore);
  alerts = model.required<AlertResponse[]>();
  highlightedItemId = signal<string | null>(null);
  deletedItemId = signal<string | null>(null);

  readonly table = viewChild.required<MatTable<AlertResponse>>(MatTable);
  readonly sort = viewChild.required<MatSort>(MatSort);

  constructor() {
    effect(() => {
      const createdAlert = this.alertStore.created();
      if (!createdAlert) return;

      this.alerts.update(alerts => [createdAlert, ...alerts]);
      this.highlightedItemId.set(createdAlert.id);
      setTimeout(() => this.highlightedItemId.set(null), 800);
      this.table().renderRows();
    });

    effect(() => {
      const updatedAlert = this.alertStore.updated();
      if (!updatedAlert) return;

      this.alerts.update(alerts => {
        const filtered = alerts.filter(alert => alert.id !== updatedAlert.id);
        return [updatedAlert, ...filtered];
      });
      this.highlightedItemId.set(updatedAlert.id);
      setTimeout(() => this.highlightedItemId.set(null), 800);
      this.table().renderRows();
    });

    effect(() => {
      const deletedAlert = this.alertStore.deleted();
      if (!deletedAlert) return;

      this.alerts.update(alerts => [deletedAlert, ...alerts.filter(alert => alert.id !== deletedAlert.id)]);
      this.deletedItemId.set(deletedAlert.id);
      this.table().renderRows();

      setTimeout(() => {
        this.alerts.update(alerts => alerts.filter(alert => alert.id !== deletedAlert.id));
        this.deletedItemId.set(null);
        this.table().renderRows();
      }, 3000);
    });
  }

  readonly alertColumns: readonly string[] = ['message', 'severity', 'occurrence', 'status', 'occurredAt'] as const;

  onAlertClicked(alert: AlertResponse) {
    const alertId = alert.id;
    if (alertId) {
      this.router.navigate(['/alerts', alertId]);
    }
  }

  onSortChange($event: Sort) {
     if ($event.active && $event.direction) {
      this.router.navigate([], {
        queryParams: {
          orderBy: `${$event.active} ${$event.direction}`,
        },
        queryParamsHandling: 'merge',
      });
    }
  }
}
