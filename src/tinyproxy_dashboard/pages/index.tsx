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
import { RequestView } from '../components/requestlist';
import { RequestEditor } from '../components/requesteditor';

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
            <RequestEditor />
            <RequestView />
          </Box>
        </div>
      </main>
    </div>
  );
}
