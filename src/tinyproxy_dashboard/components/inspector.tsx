import { Grid, Typography } from '@mui/material';
import { FunctionComponent, useState } from 'react';
import { useTinyContext } from '../context/tinycontext';
import { ContentDetails } from './contentdetails';
import { HeaderDetails } from './headerdetails';

type InspectorProps = {};

export const Inspector: FunctionComponent<InspectorProps> = () => {
  const { currentRequest } = useTinyContext();

  if (!currentRequest) {
    return <Typography>Select a request to inspect</Typography>;
  }
  return (
    <Grid container spacing={2}>
      <Grid item xs={6}>
        <Typography component="h5" color="primary">
          Request
        </Typography>
        <HeaderDetails headers={currentRequest.request.headers} />
        <ContentDetails
          content={currentRequest.request.content}
          contentType={''}
        />
      </Grid>
      <Grid item xs={6}>
        <Typography component="h5" color="primary">
          Response
        </Typography>
        <HeaderDetails headers={currentRequest.response.headers} />
        <ContentDetails
          content={currentRequest.response.content}
          contentType={''}
        />
      </Grid>
    </Grid>
  );
};
