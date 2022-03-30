import {
  Grid,
  Typography,
  Box,
  Accordion,
  AccordionSummary,
  AccordionDetails,
  Tabs,
  Tab
} from '@mui/material';
import { FunctionComponent, ReactNode, SyntheticEvent, useState } from 'react';
import { useTinyContext } from '../context/tinycontext';
import { ContentDetails } from './contentdetails';
import { HeaderDetails } from './headerdetails';
import { UpstreamHandlerEditor } from './upstreamhandlereditor';

type InspectorProps = {};

interface TabPanelProps {
  children?: ReactNode;
  dir?: string;
  index: number;
  value: number;
}

function TabPanel(props: TabPanelProps) {
  const { children, value, index, ...other } = props;
  return (
    <div
      role="tabpanel"
      hidden={value !== index}
      id={`full-width-tabpanel-${index}`}
      {...other}
    >
      {value === index && <Box sx={{ p: 3 }}>{children}</Box>}
    </div>
  );
}

export const Inspector: FunctionComponent<InspectorProps> = () => {
  const { currentRequest } = useTinyContext();
  const [value, setValue] = useState(0);
  const handleChange = (event: SyntheticEvent, newValue: number) =>
    setValue(newValue);

  return (
    <Box>
      <Tabs
        indicatorColor="secondary"
        textColor="inherit"
        variant="fullWidth"
        onChange={handleChange}
        value={value}
      >
        <Tab label="Quick view" />
        <Tab label="Handler" />
        <Tab label="Request Headers" />
        <Tab label="Response Headers" />
      </Tabs>
      <TabPanel value={value} index={0}>
        <Typography>Not implemented</Typography>
      </TabPanel>
      <TabPanel value={value} index={1}>
        <UpstreamHandlerEditor handler={currentRequest.handler} />
      </TabPanel>
      <TabPanel value={value} index={2}>
        <HeaderDetails headers={currentRequest.request.headers} />
      </TabPanel>
      <TabPanel value={value} index={3}>
        <HeaderDetails headers={currentRequest.response.headers} />
      </TabPanel>
    </Box>
  );
};
