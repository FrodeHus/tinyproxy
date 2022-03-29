import {
  Grid,
  Typography,
  Box,
  Accordion,
  AccordionSummary,
  AccordionDetails
} from '@mui/material';
import { FunctionComponent, useState } from 'react';
import { useTinyContext } from '../context/tinycontext';
import { ContentDetails } from './contentdetails';
import { HeaderDetails } from './headerdetails';

type InspectorProps = {};

export const Inspector: FunctionComponent<InspectorProps> = () => {
  const { currentRequest } = useTinyContext();

  return (
    <Box>
      <Accordion>
        <AccordionSummary>
          <Typography>Upstream Handler</Typography>
        </AccordionSummary>
      </Accordion>{' '}
      <Accordion>
        <AccordionSummary>
          <Typography>Request Attributes</Typography>
        </AccordionSummary>
      </Accordion>
      <Accordion>
        <AccordionSummary>
          <Typography>Request Query Parameters</Typography>
        </AccordionSummary>
      </Accordion>
      <Accordion>
        <AccordionSummary>
          <Typography>Request Body Parameters</Typography>
        </AccordionSummary>
      </Accordion>
      <Accordion>
        <AccordionSummary>
          <Typography>Request Headers</Typography>
        </AccordionSummary>
        <AccordionDetails>
          <HeaderDetails headers={currentRequest.request.headers} />
        </AccordionDetails>
      </Accordion>
      <Accordion>
        <AccordionSummary>
          <Typography>Response Headers</Typography>
        </AccordionSummary>
        <AccordionDetails>
          <HeaderDetails headers={currentRequest.response.headers} />
        </AccordionDetails>
      </Accordion>
    </Box>
  );
};
