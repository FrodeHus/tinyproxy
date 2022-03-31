import { HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import {
  Badge,
  Chip,
  List,
  ListItem,
  ListItemButton,
  ListItemIcon,
  ListItemText
} from '@mui/material';
import { FunctionComponent, useEffect, useState } from 'react';
import { useTinyContext } from '../context/tinycontext';
import { Request } from './types';

export const RequestView: FunctionComponent = () => {
  const [requestData, setRequestData] = useState<Request[]>([]);
  const { setSelectedRequest, hubConnection } = useTinyContext();
  const addRequestRow = (data: Request) => {
    setRequestData((state) => [...state, data]);
  };

  useEffect(() => {
    if (!hubConnection) return;
    hubConnection.on('ReceiveRequest', function (request: Request) {
      addRequestRow(request);
    });
  }, [hubConnection]);

  return (
    <List>
      {requestData.map((req) => {
        return (
          <ListItem>
            <ListItemIcon>
              <Badge
                badgeContent={req.statusCode}
                max={1000}
                anchorOrigin={{
                  vertical: 'top',
                  horizontal: 'left'
                }}
              >
                <Chip label={req.method} />
              </Badge>
            </ListItemIcon>
            <ListItemButton>
              <ListItemText
                primary={req.path}
                secondary={req.handler?.remoteServer}
              />
            </ListItemButton>
          </ListItem>
        );
      })}
    </List>
  );
};
