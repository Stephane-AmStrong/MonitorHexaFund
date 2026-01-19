import { BaseModel } from "./base-model";
import { SseMessageType } from "./sse-message-enums";

export interface SseEventModel<T extends BaseModel> {
    event: SseMessageType;
    content: T;
}
