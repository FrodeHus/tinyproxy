import { Typography } from '@mui/material';
import { FunctionComponent } from 'react';
import { useTinyContext } from '../context/tinycontext';

type InspectorProps = {};

export const Inspector: FunctionComponent<InspectorProps> = () => {
  const { currentRequest } = useTinyContext();
  if (!currentRequest) {
    return (<Typography>Select a request to inspect</Typography>);
  }
  return <div>{currentRequest.path}</div>;
};
