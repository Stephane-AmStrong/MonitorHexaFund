import { ServerStatus } from './server-status-enum';
import { QueryParameters } from "../../../core/models/paging/query-parameters";

export interface ServerStatusQueryParameters extends QueryParameters {
  withServerId?: string;
  ofStatus?: ServerStatus;
  recordedBefore?: string;
  recordedAfter?: string;
}