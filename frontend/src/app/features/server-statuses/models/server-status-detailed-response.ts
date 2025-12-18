import { ServerResponse } from "../../servers/models/server-response";
import { ServerStatusResponse } from "./server-status-response";

export interface ServerStatusDetailedResponse extends ServerStatusResponse {
  server: ServerResponse;
}
