import { AlertSeverity } from "./alert-severity-enum";

export interface AlertCreateOrIncrementRequest {
  appId?: string;
  message?: string;
  severity?: AlertSeverity;
}
