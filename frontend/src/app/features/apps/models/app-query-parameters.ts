import { QueryParameters } from "../../../core/services/rest-api/paging/query-parameters";

export interface AppQueryParameters extends QueryParameters {
  withHostName?: string;
  withAppName?: string;
  withVersion?: string;
}
