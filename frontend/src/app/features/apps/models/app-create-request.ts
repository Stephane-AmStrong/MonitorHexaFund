export interface AppCreateRequest {
  hostName: string;
  appName: string;
  port: string;

  type: string;

  version: string;
  tags: Set<string>;
}
