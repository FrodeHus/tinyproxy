import {
  FormControl,
  FormControlLabel,
  FormGroup,
  FormLabel,
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
    <Stack spacing={2}>
      <TextField
        label="Name"
        value={handler.remoteServer}
        disabled={!isEditing}
      />
      <TextField
        label="Upstream URL"
        value={handler.remoteServerBaseUrl}
        disabled={!isEditing}
      />
      {handler.swaggerEndpoint && (
        <TextField
          label="Swagger Endpoint"
          value={handler.swaggerEndpoint}
          disabled={!isEditing}
        />
      )}
      <TextField
        hidden={!handler.relativePath}
        label="Path"
        value={handler.relativePath}
        disabled={!isEditing}
      />
      <TextField label="Prefix" value={handler.prefix} disabled={!isEditing} />
      <FormControl fullWidth>
        <InputLabel id='handler-http-method'>Method</InputLabel>
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
    </Stack>
  );
};
