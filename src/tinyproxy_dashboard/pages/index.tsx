import Head from 'next/head';
import {
  Accordion,
  AccordionDetails,
  AccordionSummary,
  AppBar,
  Box,
  CssBaseline,
  Drawer,
  Toolbar,
  Typography
} from '@mui/material';
import { RequestView } from '../components/requestlist';
import { RequestEditor } from '../components/requesteditor';

export default function Home() {
  const drawerWidth = 300;
  return (
    <div>
      <Head>
        <title>TinyProxy Dashboard</title>
        <link rel="icon" href="/favicon.ico" />
      </Head>

      <main>
        <Box sx={{ display: 'flex' }}>
          <CssBaseline />
          <AppBar
            position="fixed"
            sx={{ zIndex: (theme) => theme.zIndex.drawer + 1 }}
          >
            <Toolbar>
              <Typography variant="h6" color="inherit" component="div">
                TinyProxy Dashboard
              </Typography>
            </Toolbar>
          </AppBar>
          <Drawer
            variant="permanent"
            sx={{
              width: drawerWidth,
              flexShrink: 0,
              [`& .MuiDrawer-paper`]: {
                width: drawerWidth,
                boxSizing: 'border-box'
              }
            }}
          >
            <Box sx={{ overflow: 'auto' }}>
              <RequestView />
            </Box>
          </Drawer>
          <Box sx={{ padding: 2 }}>
            <RequestEditor />
          </Box>
        </Box>
      </main>
    </div>
  );
}
