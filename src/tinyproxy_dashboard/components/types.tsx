export type RequestData = {
  headers: {};
  content: string;
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

export type ProxyData = {
  requestId: number;
  handler: RouteHandler;
  path: string;
  statusCode: number;
  request: RequestData;
  response: RequestData;
};
