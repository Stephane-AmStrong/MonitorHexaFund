import { ClientResponse } from '../../clients/models/client-response';
import { ConnectionResponse } from './connection-response';
import { ServerResponse } from '../../servers/models/server-response';

export interface ConnectionDetailedResponse extends ConnectionResponse {
  client: ClientResponse;
  server: ServerResponse;
}