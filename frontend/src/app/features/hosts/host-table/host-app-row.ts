import { HostResponse } from "../models/host-response";
import { AppResponse } from "../../apps/models/app-response";

export interface HostAppRow {
  app: AppResponse;
  hostRowSpan: number;
  isFirstApp: boolean;
}
