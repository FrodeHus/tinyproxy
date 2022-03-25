import React, { FunctionComponent } from 'react';
import { RouteHandler, ProxyData, RequestData } from '../components/types';
import Accordion from '@mui/material/Accordion';
import AccordionSummary from '@mui/material/AccordionSummary';
import AccordionDetails from '@mui/material/AccordionDetails';
import Typography from '@mui/material/Typography';
import { Chip, Badge, Tabs, Tab, Box, Stack } from '@mui/material';
import { HeaderDetails } from './headerdetails';
import { ContentDetails } from './contentdetails';
type RequestItemProps = {
  handler: RouteHandler;
  path: string;
  statusCode: number;
  request: RequestData;
  response: RequestData;
};
const getStatusClass = (statusCode: number) => {
  if (statusCode < 200 || statusCode >= 400) {
    return 'error';
  }

  return 'success';
};

type TabPanelProps = {
  children?: React.ReactNode;
  index: number;
  value: number;
};

const TabPanel: FunctionComponent<TabPanelProps> = ({
  index,
  value,
  children
}) => {
  return (
    <div
      role="tabpanel"
      hidden={value !== index}
      id={`simple-tabpanel-${index}`}
      aria-labelledby={`simple-tab-${index}`}
    >
      {value === index && <Box sx={{ p: 3 }}>{children}</Box>}
    </div>
  );
};

function a11yProps(index: number) {
  return {
    id: `simple-tab-${index}`,
    'aria-controls': `simple-tabpanel-${index}`
  };
}

export const RequestItem: FunctionComponent<RequestItemProps> = ({
  handler,
  path,
  statusCode,
  request,
  response
}) => {
  const [view, setView] = React.useState(0);
  const handleChange = (event: React.SyntheticEvent, newValue: number) => {
    setView(newValue);
  };
  const contentType = request.headers;
  return (
    <Accordion>
      <AccordionSummary>
        <Stack direction="row" spacing={1}>
          <Badge
            badgeContent={statusCode}
            max={1000}
            color={getStatusClass(statusCode)}
            anchorOrigin={{
              vertical: 'top',
              horizontal: 'left'
            }}
          >
            <Chip variant="outlined" label={handler.method.toUpperCase()} />
          </Badge>
          <Chip color="primary" label={handler.serverName} />
          <Chip color="secondary" label={handler.prefix} />
          {handler.preferred && <Chip color="info" label="preferred"></Chip>}
        </Stack>
        <span className="request-path-summary">{path}</span>
      </AccordionSummary>
      <AccordionDetails>
        <Box sx={{ width: '100%' }}>
          <Box sx={{ borderBottom: 1, borderColor: 'divider' }}>
            <Tabs value={view} onChange={handleChange}>
              <Tab label="Request" {...a11yProps(0)} />
              <Tab label="Response" {...a11yProps(1)} />
            </Tabs>
          </Box>
          <TabPanel value={view} index={0}>
            <HeaderDetails headers={request.headers} />
            <ContentDetails content={request.content} contentType={ ""}/>
          </TabPanel>
          <TabPanel value={view} index={1}>
            <HeaderDetails headers={response.headers} />
            <ContentDetails content={response.content}  contentType={ ""}/>
          </TabPanel>
        </Box>{' '}
      </AccordionDetails>
    </Accordion>
  );
};
