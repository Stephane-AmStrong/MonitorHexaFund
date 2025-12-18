import { QueryParameters } from "../../../core/models/paging/query-parameters";

export interface ClientQueryParameters extends QueryParameters {
  withGaia?: string;
  withLogin?: string;
}