import { AlertSeverity } from "./alert-severity-enum";
import { QueryParameters } from "../../../core/services/rest-api/paging/query-parameters";

export interface AlertQueryParameters extends QueryParameters {
  withAppId?: string;
  ofSeverity?: AlertSeverity;
  occurredBefore?: string;
  occurredAfter?: string;
}
