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
import { selectedGridRowsSelector } from '@mui/x-data-grid';
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

  return (
    <List>
      {requestData.map((req) => {
        return (
          <ListItem key={req.id}>
            <ListItemButton
              selected={selectedIndex === req.id}
              onClick={(event) => handleRequestSelect(event, req)}
            >
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
