import { ConnectionResponse } from "../../connections/models/connection-response";
import { AppResponse } from "./app-response";
import { AppStatusResponse } from "../../app-statuses/models/app-status-response";

export interface AppDetailedResponse extends AppResponse {
  connections: ConnectionResponse[];
  statuses: AppStatusResponse[];
}
