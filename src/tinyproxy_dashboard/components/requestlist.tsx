import { HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import { Chip } from '@mui/material';
import { DataGrid, GridColDef } from '@mui/x-data-grid';
import { FunctionComponent, useEffect, useState } from 'react';
import { useTinyContext } from '../context/tinycontext';
import { Request } from './types';

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
  const [requestData, setRequestData] = useState<Request[]>([]);
  const { setSelectedRequest, hubConnection } = useTinyContext();
  const addRequestRow = (data: Request) => {
    setRequestData((state) => [...state, data]);
    setRequestRows((state) => [
      ...state,
      {
        id: data.id,
        method: data.method,
        prefix: data.handler?.prefix,
        path: data.path,
        statusCode: data.statusCode,
        upstream: data.handler?.remoteServer,
        preferred: data.handler?.preferred
      }
    ]);
  };

  useEffect(() => {
    if (!hubConnection) return;
    hubConnection.on('ReceiveRequest', function (request: Request) {
      addRequestRow(request);
    });
  }, [hubConnection]);

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
      flex: 3,
      renderCell: (cellValues) => {
        return <Chip variant="outlined" label={cellValues['value']} />;
      }
    },
    {
      field: 'statusCode',
      headerName: 'Status',
      flex: 3,
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

        const selectedRowData = requestData.filter((row) =>
          selectedIDs.has(row.id)
        );
        if (setSelectedRequest && selectedRowData) {
          setSelectedRequest(selectedRowData[0]);
        }
      }}
    />
  );
};
