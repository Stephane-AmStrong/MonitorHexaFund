import { QueryParameters } from "../../../core/services/rest-api/paging/query-parameters";

export interface ConnectionQueryParameters extends QueryParameters {
  withClientGaia?: string;
  withAppId?: string;
}
