import { HostResponse } from "../models/host-response";
import { ServerResponse } from "../../servers/models/server-response";

export interface HostServerRow {
  host: HostResponse;
  server: ServerResponse;
  hostRowSpan: number;
  isFirstServer: boolean;
}
