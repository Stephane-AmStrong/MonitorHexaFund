import { Component, effect, inject, model, viewChild } from '@angular/core';
import { AlertResponse } from '../models/alert-response';
import { Router } from '@angular/router';
import { MatTable, MatTableModule } from '@angular/material/table';
import { AlertStore } from '../services/alert.store';

@Component({
  selector: 'alert-table',
  imports: [MatTableModule],
  templateUrl: './alert-table.component.html',
  styleUrl: './alert-table.component.scss'
})
export class AlertTableComponent {
  private readonly router = inject(Router);
  private readonly alertStore = inject(AlertStore);
  alerts = model.required<AlertResponse[]>();

  table = viewChild.required<MatTable<AlertResponse>>(MatTable);

  constructor() {
    effect(() => {
      const createdAlert = this.alertStore.created();
      if (!createdAlert) return;

      this.alerts.update(alerts => [...alerts, createdAlert]);
      this.table().renderRows();
    });

    effect(() => {
      const updatedAlert = this.alertStore.updated();
      if (!updatedAlert) return;

      this.alerts.update(alerts => alerts.map(alert => alert.id === updatedAlert.id ? updatedAlert : alert));
      this.table().renderRows();
    });

    effect(() => {
      const deletedAlert = this.alertStore.deleted();
      if (!deletedAlert) return;

      this.alerts.update(alerts => alerts.filter(alert => alert.id !== deletedAlert.id));
      this.table().renderRows();
    });
  }

  readonly alertColumns: readonly string[] = ['message', 'severity', 'occurrence', 'status', 'occurredAt'] as const;

  onAlertClicked(alert: AlertResponse) {
    const alertId = alert.id;
    if (alertId) {
      this.router.navigate(['/alerts', alertId]);
    }
  }
}
