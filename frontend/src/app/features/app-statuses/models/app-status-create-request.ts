import { AppStatus } from "./app-status-enum";

export interface AppStatusCreateRequest {
  appId: string;
  status: AppStatus;
}
