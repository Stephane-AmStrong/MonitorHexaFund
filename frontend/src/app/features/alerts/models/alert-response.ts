import { BaseModel } from "../../../core/models/base-model";
import { AlertSeverity } from "./alert-severity-enum";
import { AlertStatus } from "./alert-status-enum";
import { AlertType } from "./alert-type-enum";


export interface AlertResponse extends BaseModel {
  serverId?: string;
  type?: AlertType;
  message?: string;
  severity?: AlertSeverity;
  occurrence: number;
  status: AlertStatus;
  occurredAt: string;
}
