import Head from 'next/head';
import {
  Accordion,
  AccordionDetails,
  AccordionSummary,
  AppBar,
  Box,
  Toolbar,
  Typography
} from '@mui/material';
import { Inspector } from '../components/inspector';
import { RequestView } from '../components/requestlist';

export default function Home() {
  return (
    <div>
      <Head>
        <title>TinyProxy Dashboard</title>
        <link rel="icon" href="/favicon.ico" />
      </Head>

      <main>
        <div>
          <Box sx={{ flexGrow: 1 }}>
            <AppBar position="static">
              <Toolbar>
                <Typography variant="h6" color="inherit" component="div">
                  TinyProxy Dashboard
                </Typography>
              </Toolbar>
            </AppBar>
          </Box>
          <Box sx={{ padding: 2 }}>
            <Inspector />
            <RequestView />
          </Box>
        </div>
      </main>
    </div>
  );
}
