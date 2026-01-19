import { AppUpdateRequest } from '../../app-statuses/models/app-update-request';

export interface AppCreateRequest extends AppUpdateRequest {
  id: string;
}
