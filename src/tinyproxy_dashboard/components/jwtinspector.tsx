import { Box, FormControlLabel, Switch, Typography } from '@mui/material';
import { FunctionComponent, useState } from 'react';
type JwtInspectorProps = {
  jwtToken: string;
};

function decodeJwtToken(jwtToken: string) {
  const parts = jwtToken.split(' ')[1].split('.');
  const header = Buffer.from(parts[0], 'base64').toString('binary');
  const payload = Buffer.from(parts[1], 'base64').toString('binary');
  const headerObj = JSON.parse(header);
  const payloadObj = JSON.parse(payload);

  return (
    JSON.stringify(headerObj, null, 2) +
    '.' +
    JSON.stringify(payloadObj, null, 2) +
    '.[Signature]'
  );
}

export const JwtInspector: FunctionComponent<JwtInspectorProps> = ({
  jwtToken
}) => {
  const [showDecoded, setShowDecoded] = useState(false);
  const handleChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    setShowDecoded(event.target.checked);
  };
  return (
    <Box>
      <FormControlLabel
        control={<Switch checked={showDecoded} onChange={handleChange} />}
        label="Decode"
      />
      {!showDecoded && <Typography className="content">{jwtToken}</Typography>}
      {showDecoded && <pre>{decodeJwtToken(jwtToken)}</pre>}
    </Box>
  );
};
