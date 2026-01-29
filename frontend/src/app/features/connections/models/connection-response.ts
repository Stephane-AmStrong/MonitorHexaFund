import { BaseModel } from '../../../core/models/base-model';

export interface ConnectionResponse extends BaseModel {
  clientGaia: string;
  appId: string;
  processId: string;
  apiVersion: string;
  machine: string;
  establishedAt: string;
  terminatedAt?: string;
}
