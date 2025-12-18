import { ServerStatus } from "./server-status-enum";

export interface ServerStatusCreateRequest {
  serverId: string;
  status: ServerStatus;
}
