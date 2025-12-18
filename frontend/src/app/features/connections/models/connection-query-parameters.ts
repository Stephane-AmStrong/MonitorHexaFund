import { QueryParameters } from "../../../core/models/paging/query-parameters";

export interface ConnectionQueryParameters extends QueryParameters {
  withClientId?: string;
  withServerId?: string;
  withApplication?: string;
}