import { HostResponse } from "./host-response";
import { ServerResponse } from "../../servers/models/server-response";

export interface HostDetailedResponse extends HostResponse {
    servers : ServerResponse[];
}
