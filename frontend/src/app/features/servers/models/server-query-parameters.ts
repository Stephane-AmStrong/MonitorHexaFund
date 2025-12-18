import { QueryParameters } from "../../../core/models/paging/query-parameters";

export interface ServerQueryParameters extends QueryParameters {
  withHostName?: string;
  withAppName?: string;
  withVersion?: string;
}