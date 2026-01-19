import { AppStatus } from './app-status-enum';
import { QueryParameters } from "../../../core/services/rest-api/paging/query-parameters";

export interface AppStatusQueryParameters extends QueryParameters {
  withAppId?: string;
  ofStatus?: AppStatus;
  recordedBefore?: string;
  recordedAfter?: string;
}
