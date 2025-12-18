import { BaseModel } from "../../../core/models/base-model";

export interface ConnectionResponse extends BaseModel {
  clientId: string;
  serverId: string;
  application: string;
  apiVersion: string;
  machine: string;
  processId: string;
}
