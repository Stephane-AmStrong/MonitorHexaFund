import { HostResponse } from "../models/host-response";
import { AppResponse } from "../../apps/models/app-response";

export interface HostAppRow {
  host: HostResponse;
  app: AppResponse;
  hostRowSpan: number;
  isFirstApp: boolean;
}
