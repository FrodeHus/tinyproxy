import { Box, Grid, Typography } from '@mui/material';
import { FunctionComponent } from 'react';
import { useTinyContext } from '../context/tinycontext';
import { ContentDetails } from './contentdetails';
import { Inspector } from './inspector';

export const RequestEditor: FunctionComponent = () => {
  const { currentRequest } = useTinyContext();

  return (
    <Grid container spacing={2}>
      <Grid item xs={4}>
        <Typography component="h5" color="primary">
          Request
        </Typography>
        <ContentDetails
          content={currentRequest.request.content}
          contentType={''}
        />
      </Grid>
      <Grid item xs={4}>
        <Typography component="h5" color="primary">
          Response
        </Typography>
        <ContentDetails
          content={currentRequest.response.content}
          contentType={''}
        />
      </Grid>
      <Grid item xs={4}>
        <Inspector />
      </Grid>
    </Grid>
  );
};
