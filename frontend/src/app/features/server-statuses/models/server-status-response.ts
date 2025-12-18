import { BaseModel } from '../../../core/models/base-model';
import { ServerStatus } from './server-status-enum';

export interface ServerStatusResponse extends BaseModel {
  serverId: string;
  status: ServerStatus;
  recordedAt: string;
}
