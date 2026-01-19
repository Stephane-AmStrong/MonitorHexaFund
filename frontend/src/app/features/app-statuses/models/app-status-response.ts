import { BaseModel } from '../../../core/models/base-model';
import { AppStatus } from './app-status-enum';

export interface AppStatusResponse extends BaseModel {
  appId: string;
  status: AppStatus;
  recordedAt: string;
}
