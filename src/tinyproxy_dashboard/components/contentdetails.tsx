import {
  Box,
  Button,
  FormControlLabel,
  Switch,
  TextField,
  Typography
} from '@mui/material';
import { Download } from '@mui/icons-material';
import React, { useEffect, useState } from 'react';
import { useTinyContext } from '../context/tinycontext';

type ContentDetailsProps = {
  contentId: string;
  contentLength: number;
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

export const ContentDetails: React.FC<ContentDetailsProps> = ({
  contentId,
  contentLength
}) => {
  const [encoded, setEncoded] = useState(true);
  const [content, setContent] = useState('');
  const [downloadLargeContent, setDownloadLargeContent] = useState(false);
  const { hubConnection } = useTinyContext();
  const handleChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    setEncoded(event.target.checked);
  };
  const maxAutoDownloadSize = 16 * 1024;
  const manuallyFetch = () => {
    setDownloadLargeContent(true);
    if (!hubConnection || hubConnection?.state !== 'Connected') return;
    hubConnection
        .invoke('GetContent', contentId)
        .then((loadedContent: string) => {
          setContent(loadedContent);
        })
        .catch((reason: any) => {
          console.log(reason);
        });
  }
  useEffect(() => {
    if (contentId && hubConnection?.state === 'Connected' && (contentLength < maxAutoDownloadSize || downloadLargeContent)) {
      hubConnection
        .invoke('GetContent', contentId)
        .then((loadedContent: string) => {
          setContent(loadedContent);
        })
        .catch((reason: any) => {
          console.log(reason);
        });
    }
  }, [contentId]);

  if (contentLength > maxAutoDownloadSize && !downloadLargeContent) {
    return (
      <Box>
        <Typography>Content is {contentLength / 1024}kB</Typography>
        <Button variant="contained" endIcon={<Download />} onClick={manuallyFetch}>
          Fetch content
        </Button>
      </Box>
    );
  }

  return (
    <Box sx={{ width: '95%' }}>
      <FormControlLabel
        control={<Switch checked={encoded} onChange={handleChange} />}
        label="Encoded"
      />
      <Box sx={{ overflowWrap: 'anywhere' }}>
        <TextField
          multiline
          rows={20}
          fullWidth
          value={getContent(content, encoded)}
        />
      </Box>
    </Box>
  );
};
