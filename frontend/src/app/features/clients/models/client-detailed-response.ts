import { ClientResponse } from "./client-response";
import { ConnectionResponse } from "../../connections/models/connection-response";

export interface ClientDetailedResponse extends ClientResponse {
  connections: ConnectionResponse[];
}