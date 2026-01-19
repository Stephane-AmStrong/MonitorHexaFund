import { AlertResponse } from "./alert-response";
import { AppResponse } from "../../apps/models/app-response";

export interface AlertDetailedResponse extends AlertResponse {
  app: AppResponse;
}
