import { BaseModel } from "../../../core/models/base-model";
import { AlertSeverity } from "./alert-severity-enum";
import { AlertStatus } from "./alert-status-enum";

export interface AlertResponse extends BaseModel {
  appId?: string;
  message?: string;
  severity?: AlertSeverity;
  occurrence: number;
  status: AlertStatus;
  occurredAt: string;
}
