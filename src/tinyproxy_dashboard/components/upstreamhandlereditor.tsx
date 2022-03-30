import {
  FormControl,
  FormControlLabel,
  FormGroup,
  FormLabel,
  Stack,
  Switch,
  TextField
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
  return (
    <Stack spacing={2}>
      <TextField
        label="Name"
        value={handler.serverName}
        disabled={!isEditing}
      />
      <TextField
        label="Upstream URL"
        value={handler.serverUrl}
        disabled={!isEditing}
      />
      <TextField
        label="Swagger Endpoint"
        value={handler.swaggerEndpoint}
        disabled={!isEditing}
      />
      <TextField label="Prefix" value={handler.prefix} disabled={!isEditing} />
      <FormControl component="fieldset">
        <FormLabel component="legend">HTTP methods</FormLabel>
        <FormGroup>
          <FormControlLabel
            disabled={!isEditing}
            control={<Switch name="get" />}
            label="GET"
          />
          <FormControlLabel
            disabled={!isEditing}
            control={<Switch name="post" />}
            label="POST"
          />
          <FormControlLabel
            disabled={!isEditing}
            control={<Switch name="put" />}
            label="PUT"
          />
          <FormControlLabel
            disabled={!isEditing}
            control={<Switch name="delete" />}
            label="DELETE"
          />
          <FormControlLabel
            disabled={!isEditing}
            control={<Switch name="patch" />}
            label="PATCH"
          />
          <FormControlLabel
            disabled={!isEditing}
            control={<Switch name="options" />}
            label="OPTIONS"
          />
        </FormGroup>
      </FormControl>
    </Stack>
  );
};
