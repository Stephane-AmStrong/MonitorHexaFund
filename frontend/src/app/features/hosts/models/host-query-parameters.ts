import { QueryParameters } from "../../../core/services/rest-api/paging/query-parameters";

export interface HostQueryParameters extends QueryParameters {
  withName?: string;
}
