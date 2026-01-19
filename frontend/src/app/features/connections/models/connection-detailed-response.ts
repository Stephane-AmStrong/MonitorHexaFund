import { ClientResponse } from '../../clients/models/client-response';
import { ConnectionResponse } from './connection-response';
import { AppResponse } from '../../apps/models/app-response';

export interface ConnectionDetailedResponse extends ConnectionResponse {
  client: ClientResponse;
  app: AppResponse;
}