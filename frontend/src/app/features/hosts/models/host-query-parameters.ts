import { QueryParameters } from "../../../core/models/paging/query-parameters";

export interface HostQueryParameters extends QueryParameters {
  withName?: string;
}
