import { BaseModel } from "../../../core/models/base-model";

export interface ConnectionResponse extends BaseModel {
  clientGaia: string;
  clientLogin: string;
  appId: string;
  apiVersion: string;
  machine: string;
  processId: string;
  establishedAt: string;
  terminatedAt?: string;
}
