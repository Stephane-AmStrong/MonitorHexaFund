import { BaseModel } from "../../../core/models/base-model";
import { ServerStatusResponse } from "../../server-statuses/models/server-status-response";

export interface ServerResponse extends BaseModel {
  hostName: string;
  appName: string;
  port: string;

  type: string;
  latestStatus: ServerStatusResponse;

  cronStartTime: string;
  cronStopTime: string;

  version: string;
  tags: string[];
}
