import { QueryParameters } from "../../../core/services/rest-api/paging/query-parameters";

export interface ConnectionQueryParameters extends QueryParameters {
  withClientGaia?: string;
  withAppId?: string;
  establishedBefore?: string;
  establishedAfter?: string;
  terminatedBefore?: string;
  terminatedAfter?: string;
}
