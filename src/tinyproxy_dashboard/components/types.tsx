export type Request = {
  id: number;
  traceIdentifier?: string;
  path: string;
  method: string;
  requestHeaders: {};
  responseHeaders: {};
  requestContentId?: string;
  requestContentLength?: number;
  responseContentId?: string;
  responseContentLength?: number;
  handler: RouteHandler;
  statusCode: number;
};

export type RouteHandler = {
  method: string;
  serverName: string;
  serverUrl: string;
  prefix: string;
  swaggerEndpoint?: string;
  preferred: boolean;
  routes?: StaticRoute[];
};

export type StaticRoute = {
  relativePath: string;
  httpMethods: string[];
};
