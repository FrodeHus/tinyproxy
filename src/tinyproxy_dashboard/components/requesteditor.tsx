import { Box, Grid, Typography } from '@mui/material';
import { FunctionComponent, useEffect } from 'react';
import { useTinyContext } from '../context/tinycontext';
import { ContentDetails } from './contentdetails';
import { Inspector } from './inspector';

export const RequestEditor: FunctionComponent = () => {
  const { currentRequest, hubConnection } = useTinyContext();

  return (
    <Grid container spacing={2}>
      <Grid item xs={4}>
        <Typography component="h5" color="primary">
          Request
        </Typography>
        <ContentDetails
          contentId={currentRequest.requestContentId!}
          contentLength={currentRequest.requestContentLength!}
        />
      </Grid>
      <Grid item xs={4}>
        <Typography component="h5" color="primary">
          Response
        </Typography>
        <ContentDetails
          contentId={currentRequest.responseContentId!}
          contentLength={currentRequest.responseContentLength!}
        />
      </Grid>
      <Grid item xs={4}>
        <Inspector />
      </Grid>
    </Grid>
  );
};
