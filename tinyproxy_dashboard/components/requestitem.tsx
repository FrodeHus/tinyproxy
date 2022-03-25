import React, { FunctionComponent } from 'react';
import { RouteHandler, ProxyData, RequestData } from '../components/types';
import Accordion from '@mui/material/Accordion';
import AccordionSummary from '@mui/material/AccordionSummary';
import AccordionDetails from '@mui/material/AccordionDetails';
import Typography from '@mui/material/Typography';
import { Chip, Badge } from '@mui/material';
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
        <Badge
          badgeContent={statusCode}
          max={1000}
          color="primary"
          anchorOrigin={{
            vertical: 'top',
            horizontal: 'left'
          }}
        >
          <Chip label={handler.method}></Chip>
        </Badge>
        <Typography>{path}</Typography>
      </AccordionSummary>
      <AccordionDetails>
        <Typography>Testing</Typography>
      </AccordionDetails>
    </Accordion>
  );
};
