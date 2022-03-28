import { HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import { Chip } from '@mui/material';
import { DataGrid, GridColDef, MuiEvent } from '@mui/x-data-grid';
import { FunctionComponent, useEffect, useState } from 'react';
import { useTinyContext } from '../context/tinycontext';
import { ProxyData } from './types';

type RequestRow = {
  id: number;
  path: string;
  prefix: string;
  method: string;
  statusCode: number;
  upstream: string;
  preferred: boolean;
};

export const RequestView: FunctionComponent = () => {
  const [requestRows, setRequestRows] = useState<RequestRow[]>([]);
  const [proxyData, setProxyData] = useState<ProxyData[]>([]);
    const { setSelectedRequest } = useTinyContext();
    const addRequestRow = (data: ProxyData) => {
        setProxyData((state) => [...state, data]);
        setRequestRows((state) => [
            ...state,
            {
                id: data.requestId,
                method: data.handler.method,
                prefix: data.handler.prefix,
                path: data.path,
                statusCode: data.statusCode,
                upstream: data.handler.serverName,
                preferred: data.handler.preferred
            }
        ])
    };
  useEffect(() => {
    const connection = new HubConnectionBuilder()
      .withUrl('http://localhost:5000/tinyproxy/hub')
      .configureLogging(LogLevel.Information)
      .build();

    async function start() {
      try {
        await connection.start();
        console.log('SignalR Connected.');
      } catch (err) {
        console.log(err);
        setTimeout(start, 5000);
      }
    }

    connection.onclose(async () => {
      await start();
    });

    connection.on(
      'ReceiveProxyData',
      function (
        requestId,
        path,
        statusCode,
        handler,
        requestData,
        responseData
      ) {
        if (!path.hasValue || path.value === '') {
          path = '/';
        } else {
          path = path.value.toString();
        }
        const item = {
          requestId: requestId,
          path: path,
          statusCode: statusCode,
          handler: {
            method: handler.verb.method.toLowerCase(),
            serverName: handler.remoteServer,
            serverUrl: handler.remoteServerBaseUrl,
            prefix: handler.prefix,
            preferred: handler.preffered,
            swaggerEndpoint: handler.swaggerEndpoint,
            routes: handler.routes
          },
          request: {
            headers: requestData.headers,
            content: requestData.content
          },
          response: {
            headers: responseData.headers,
            content: responseData.content
          }
        };
        addRequestRow(item);
      }
    );

    // Start the connection.
    start();
  }, []);

  const columns: GridColDef[] = [
    { field: 'id', headerName: 'ID', flex: 1 },
    {
      field: 'upstream',
      headerName: 'Upstream',
      flex: 5,
      cellClassName: 'upstream-server'
    },
    { field: 'prefix', headerName: 'Prefix', flex: 5 },
    { field: 'path', headerName: 'Path', flex: 30 },
    {
      field: 'method',
      headerName: 'Method',
      flex: 2,
      renderCell: (cellValues) => {
        return (
          <Chip variant="outlined" label={cellValues['value'].toUpperCase()} />
        );
      }
    },
    {
      field: 'statusCode',
      headerName: 'Status',
      flex: 2,
      renderCell: (cellValues) => {
        const statusCode = cellValues['value'];
        const statusClass =
          statusCode < 200 || statusCode >= 400 ? 'error' : 'success';
        return (
          <Chip color={statusClass} label={cellValues['formattedValue']} />
        );
      }
    },
    { field: 'preferred', headerName: 'Preferred', flex: 1 }
  ];
  return (
    <DataGrid
      columns={columns}
      rows={requestRows}
      autoHeight={true}
      onSelectionModelChange={(ids) => {
        const selectedIDs = new Set(ids);

        const selectedRowData = proxyData.filter((row) =>
          selectedIDs.has(row.requestId)
        );
        if (setSelectedRequest && selectedRowData) {
          setSelectedRequest(selectedRowData[0]);
        }
      }}
    />
  );
};
