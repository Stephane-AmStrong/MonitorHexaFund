import { AppCreateRequest } from '../../apps/models/app-create-request';

export interface AppUpdateRequest extends AppCreateRequest {
  runMode: string;
  cronStartTime: string;
  cronStopTime: string;
}
