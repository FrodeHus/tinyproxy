import { FunctionComponent } from 'react';
import { Request } from './types';
import {
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Paper,
  Box,
  IconButton,
  Typography
} from '@mui/material';
import { Search } from '@mui/icons-material';
import { JwtInspector } from './jwtinspector';

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

  const getVisualizer = (headerName: string, headerValue: string) => {
    switch (headerName.toLowerCase()) {
      case 'authorization':
        return (
          <Box>
            <JwtInspector jwtToken={headerValue} />
            {/* <Typography className="content">{headerValue}</Typography>
            <IconButton>
              <Search />
            </IconButton> */}
          </Box>
        );
        break;

      default:
        return <Typography className="header-value">{headerValue}</Typography>;
    }
  };
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
              <TableCell align="left">{h.name} </TableCell>
              <TableCell align="left" className="header-value">
                {getVisualizer(h.name, h.value)}
              </TableCell>
            </TableRow>
          ))}
        </TableBody>
      </Table>
    </TableContainer>
  );
};
