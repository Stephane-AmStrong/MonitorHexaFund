import { AppResponse } from "../../apps/models/app-response";
import { AppStatusResponse } from "./app-status-response";

export interface AppStatusDetailedResponse extends AppStatusResponse {
  app: AppResponse;
}
