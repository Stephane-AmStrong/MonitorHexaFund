import { BaseModel } from "../../../core/models/base-model";

export interface ClientResponse extends BaseModel {
  login: string;
  gaia: string;
}
