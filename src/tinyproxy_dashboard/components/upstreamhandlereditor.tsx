import {
  FormControl,
  FormControlLabel,
  FormGroup,
  FormLabel,
  Grid,
  InputLabel,
  MenuItem,
  Select,
  Stack,
  Switch,
  TextField,
  Typography
} from '@mui/material';
import { FunctionComponent, useState } from 'react';
import { RouteHandler } from './types';

type UpstreamHandlerEditorProps = {
  handler: RouteHandler;
};

export const UpstreamHandlerEditor: FunctionComponent<
  UpstreamHandlerEditorProps
> = ({ handler }) => {
  const [isEditing, setIsEditing] = useState(false);

  if (!handler) {
    return (
      <Typography color="warning">
        No handler defined for this request
      </Typography>
    );
  }
  return (
    <Grid container spacing={2}>
      <Grid item xs={12} sm={6}>
        <TextField
          label="Name"
          fullWidth
          value={handler.remoteServer}
          disabled={!isEditing}
        />
      </Grid>
      <Grid item xs={12} sm={6}>
        <TextField
          fullWidth
          label="Upstream URL"
          value={handler.remoteServerBaseUrl}
          disabled={!isEditing}
        />
      </Grid>
      <Grid item xs={12} sm={6}>
        <TextField
          label="Swagger Endpoint"
          fullWidth
          value={handler.swaggerEndpoint}
          disabled={!isEditing}
        />
      </Grid>
      <Grid item xs={12} sm={6}>
        <TextField
          label="Path"
          fullWidth
          value={handler.relativePath}
          disabled={!isEditing}
        />
      </Grid>
      <Grid item xs={12} sm={6}>
        <TextField
          label="Prefix"
          fullWidth
          value={handler.prefix}
          disabled={!isEditing}
        />
      </Grid>
      <Grid item xs={12} sm={6}>
        <FormControl fullWidth>
          <InputLabel id="handler-http-method">Method</InputLabel>
          <Select
            labelId="handler-http-method"
            id="handler-http-method-select"
            value={handler.verb}
            label={handler.verb}
            disabled={!isEditing}
          >
            <MenuItem value="GET">GET</MenuItem>
            <MenuItem value="PUT">PUT</MenuItem>
            <MenuItem value="POST">POST</MenuItem>
            <MenuItem value="PATCH">PATCH</MenuItem>
            <MenuItem value="DELETE">DELETE</MenuItem>
            <MenuItem value="OPTIONS">OPTIONS</MenuItem>
          </Select>
        </FormControl>
      </Grid>
    </Grid>
  );
};
