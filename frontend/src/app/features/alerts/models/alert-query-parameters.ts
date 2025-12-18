import { AlertSeverity } from "./alert-severity-enum";
import { AlertType } from "./alert-type-enum";
import { QueryParameters } from "../../../core/models/paging/query-parameters";


export interface AlertQueryParameters extends QueryParameters {
  withServerId?: string;
  ofType?: AlertType;
  ofSeverity?: AlertSeverity;
  occurredBefore?: string;
  occurredAfter?: string;
}
