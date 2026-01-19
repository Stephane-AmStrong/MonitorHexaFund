import { HostResponse } from "./host-response";
import { AppResponse } from "../../apps/models/app-response";

export interface HostDetailedResponse extends HostResponse {
    apps : AppResponse[];
}
