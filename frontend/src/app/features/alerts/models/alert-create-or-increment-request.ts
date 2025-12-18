import { AlertSeverity } from "./alert-severity-enum";
import { AlertType } from "./alert-type-enum";

export interface AlertCreateOrIncrementRequest {
  serverId?: string;
  type?: AlertType;
  message?: string;
  severity?: AlertSeverity;
}
