import React, { FunctionComponent } from 'react';
import { RouteHandler, ProxyData, RequestData } from '../components/types';
import Accordion from '@mui/material/Accordion';
import AccordionSummary from '@mui/material/AccordionSummary';
import AccordionDetails from '@mui/material/AccordionDetails';
type RequestItemProps = {
  handler: RouteHandler;
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
    <Accordion>
      <AccordionSummary>
        <div className={'traffic-block ' + handler.method}>
          <div className="traffic-summary">
            <span className="upstream-server">{handler.serverName}</span>
            <span className="request-path-summary">{path}</span>
            <span className={'request-status ' + statusClass}>
              {statusCode}
            </span>
          </div>
        </div>
      </AccordionSummary>
      <AccordionDetails>Testing</AccordionDetails>
    </Accordion>
  );
};
