import { ServerUpdateRequest } from '../../server-statuses/models/server-update-request';

export interface ServerCreateRequest extends ServerUpdateRequest {
  id: string;
}
