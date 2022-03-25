export type RequestData = {
  headers: {};
  content: string;
};

export type RouteHandler = {
  method: string;
  serverName: string;
  serverUrl: string;
  prefix: string;
};

export type ProxyData = {
  handler: RouteHandler;
  path: string;
  statusCode: number;
  request: RequestData;
};
