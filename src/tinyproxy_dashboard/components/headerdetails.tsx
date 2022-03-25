import { FunctionComponent } from 'react';
import {
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Paper
} from '@mui/material';

type HeaderDetailsProps = {
  headers: { [key: string]: string };
};

export const HeaderDetails: FunctionComponent<HeaderDetailsProps> = ({
  headers
}) => {
  const headerRows = Object.keys(headers).map((k) => ({
    name: k,
    value: headers[k]
  }));
  return (
    <TableContainer component={Paper}>
      <Table size="small">
        <TableHead>
          <TableRow>
            <TableCell>Name</TableCell>
            <TableCell>Value</TableCell>
          </TableRow>
        </TableHead>
        <TableBody>
          {headerRows.map((h) => (
            <TableRow key={h.name}>
              <TableCell align="left">{h.name}</TableCell>
              <TableCell align="left">{h.value}</TableCell>
            </TableRow>
          ))}
        </TableBody>
      </Table>
    </TableContainer>
  );
};
