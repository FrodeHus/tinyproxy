import { HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import {
  Badge,
  List,
  ListItem,
  ListItemButton,
  ListItemText
} from '@mui/material';
import React, { FunctionComponent, useEffect, useState } from 'react';
import { useTinyContext } from '../context/tinycontext';
import { Request } from './types';

export const RequestView: FunctionComponent = () => {
  const [requestData, setRequestData] = useState<Request[]>([]);
  const [selectedIndex, setSelectedIndex] = useState(0);
  const { setSelectedRequest, hubConnection } = useTinyContext();
  const addRequestRow = (data: Request) => {
    setRequestData((state) => [...state, data]);
  };

  const handleRequestSelect = (
    event: React.MouseEvent<HTMLDivElement, MouseEvent>,
    req: Request
  ) => {
    setSelectedIndex(req.id);
    if (setSelectedRequest) setSelectedRequest(req);
  };

  useEffect(() => {
    if (!hubConnection) return;
    hubConnection.on('ReceiveRequest', function (request: Request) {
      addRequestRow(request);
    });
  }, [hubConnection]);

  const statusCodeColor = (req: Request) => {
    return req.statusCode < 400 ? 'success' : 'error';
  };

  return (
    <List>
      {requestData.map((req) => {
        return (
          <ListItem key={req.id}>
            <ListItemButton
              selected={selectedIndex === req.id}
              onClick={(event) => handleRequestSelect(event, req)}
            >
              <Badge
                badgeContent={req.statusCode}
                max={1000}
                color={statusCodeColor(req)}
                anchorOrigin={{
                  horizontal: 'left',
                  vertical: 'top'
                }}
              >
                <ListItemText
                  primary={req.path}
                  secondary={
                    req.method +
                    ' ' +
                    (req.handler
                      ? req.handler.remoteServerBaseUrl
                      : 'no upstream')
                  }
                />
              </Badge>
            </ListItemButton>
          </ListItem>
        );
      })}
    </List>
  );
};
