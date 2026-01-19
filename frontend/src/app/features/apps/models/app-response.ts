import { BaseModel } from "../../../core/models/base-model";
import { AppStatusResponse } from "../../app-statuses/models/app-status-response";

export interface AppResponse extends BaseModel {
  hostName: string;
  appName: string;
  port: string;

  type: string;
  latestStatus: AppStatusResponse;

  cronStartTime: string;
  cronStopTime: string;

  version: string;
  tags: string[];
}
