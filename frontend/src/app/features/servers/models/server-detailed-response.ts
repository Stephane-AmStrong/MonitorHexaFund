import { ConnectionResponse } from "../../connections/models/connection-response";
import { ServerResponse } from "./server-response";
import { ServerStatusResponse } from "../../server-statuses/models/server-status-response";

export interface ServerDetailedResponse extends ServerResponse {
  connections: ConnectionResponse[];
  statuses: ServerStatusResponse[];
}
