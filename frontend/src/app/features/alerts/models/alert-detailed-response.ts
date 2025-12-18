import { AlertResponse } from "./alert-response";
import { ServerResponse } from "../../servers/models/server-response";

export interface AlertDetailedResponse extends AlertResponse {
  server: ServerResponse;
}
