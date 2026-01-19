import { BaseModel } from "../../models/base-model";

export interface SseEventHandlers<T extends BaseModel> {
  onCreated?: (entity: T) => void;
  onUpdated?: (entity: T) => void;
  onDeleted?: (entity: T) => void;
}