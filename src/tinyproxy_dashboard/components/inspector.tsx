import { Box, Tab, Tabs, Typography } from '@mui/material';
import { request } from 'http';
import { FunctionComponent, useState } from 'react';
import { useTinyContext } from '../context/tinycontext';
import { ContentDetails } from './contentdetails';
import { HeaderDetails } from './headerdetails';

type InspectorProps = {};
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

export const Inspector: FunctionComponent<InspectorProps> = () => {
  const { currentRequest } = useTinyContext();
  const [view, setView] = useState(0);
  const handleChange = (event: React.SyntheticEvent, newValue: number) => {
    setView(newValue);
  };

  if (!currentRequest) {
    return (<Typography>Select a request to inspect</Typography>);
  }
  return (
    <Box sx={{ width: '100%' }}>
    <Box sx={{ borderBottom: 1, borderColor: 'divider' }}>
      <Tabs value={view} onChange={handleChange}>
        <Tab label="Request" {...a11yProps(0)} />
        <Tab label="Response" {...a11yProps(1)} />
      </Tabs>
    </Box>
    <TabPanel value={view} index={0}>
      <HeaderDetails headers={currentRequest.request.headers} />
      <ContentDetails content={currentRequest.request.content} contentType={''} />
    </TabPanel>
    <TabPanel value={view} index={1}>
      <HeaderDetails headers={currentRequest.response.headers} />
      <ContentDetails content={currentRequest.response.content} contentType={''} />
    </TabPanel>
  </Box>
  );
};
