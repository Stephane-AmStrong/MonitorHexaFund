import { QueryParameters } from "../../../core/services/rest-api/paging/query-parameters";

export interface ClientQueryParameters extends QueryParameters {
  withGaia?: string;
  withLogin?: string;
}