import {
  Box,
  FormControlLabel,
  Paper,
  Switch,
  Typography
} from '@mui/material';
import React from 'react';

type ContentDetailsProps = {
  content: string;
  contentType: string;
};

const decode = (str: string): string =>
  Buffer.from(str, 'base64').toString('binary');

const getContent = (str: string, encoded: boolean): string => {
  if (!str) {
    return '';
  }
  if (encoded) {
    return str;
  }
  return decode(str);
};


export const ContentDetails: React.FC<ContentDetailsProps> = ({ content }) => {
  const [encoded, setEncoded] = React.useState(true);
  const handleChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    setEncoded(event.target.checked);
  };
  return (
    <Box sx={{ width: '95%' }}>
      <Typography component="h2" variant="h6" color="primary" gutterBottom>
        Content
      </Typography>
      <FormControlLabel
        control={<Switch checked={encoded} onChange={handleChange} />}
        label="Encoded"
      />
      <Box sx={{ overflowWrap: 'anywhere' }}>
        <pre>{getContent(content, encoded)}</pre>
      </Box>
    </Box>
  );
};
