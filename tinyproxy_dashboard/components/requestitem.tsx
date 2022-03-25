import React, { FunctionComponent } from 'react';

type RequestItemProps = {
  handler: any;
  path: string;
  statusCode: number;
};
const getStatusClass = (statusCode: number) => {
  if (statusCode < 200 || statusCode >= 400) {
    return 'error';
  }

  return 'ok';
};

export const RequestItem: FunctionComponent<RequestItemProps> = ({
  handler,
  path,
  statusCode
}) => {
  const statusClass = getStatusClass(statusCode);
  return (
    <div className={'traffic-block ' + handler.verb.method.toLowerCase()}>
      <div className="traffic-summary">
        <span className="upstream-server">{handler.remoteServer}</span>
        <span className="request-path-summary">{path}</span>
        <span className={'request-status ' + statusClass}>{statusCode}</span>
      </div>
    </div>
  );
};
