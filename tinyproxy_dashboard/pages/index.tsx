import Head from 'next/head';
import { HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import { useEffect, useState } from 'react';
import { RequestItem } from '../components';
import { ProxyData } from '../components/types';
import { AppBar, Box, Toolbar, Typography } from '@mui/material';

export default function Home() {
  const [trafficData, setTrafficData] = useState<ProxyData[]>([]);
  const addTrafficData = (data: ProxyData) =>
    setTrafficData((state) => [...state, data]);
  useEffect(() => {
    const connection = new HubConnectionBuilder()
      .withUrl('http://localhost:5000/tinyproxy/hub')
      .configureLogging(LogLevel.Information)
      .build();

    async function start() {
      try {
        await connection.start();
        console.log('SignalR Connected.');
      } catch (err) {
        console.log(err);
        setTimeout(start, 5000);
      }
    }

    connection.onclose(async () => {
      await start();
    });

    connection.on(
      'ReceiveProxyData',
      function (path, statusCode, handler, requestData, responseData) {
        if (!path.hasValue || path.value === '') {
          path = '/';
        } else {
          path = path.value.toString();
        }
        const item = {
          path: path,
          statusCode: statusCode,
          handler: {
            method: handler.verb.method.toLowerCase(),
            serverName: handler.remoteServer,
            serverUrl: handler.remoteServerBaseUrl,
            prefix: handler.prefix,
            preferred: handler.preffered,
            swaggerEndpoint: handler.swaggerEndpoint,
            routes: handler.routes
          },
          request: {
            headers: requestData.headers,
            content: requestData.content
          },
          response: {
            headers: responseData.headers,
            content: responseData.content
          }
        };

        addTrafficData(item);
      }
    );

    // Start the connection.
    start();
  }, []);

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
            <section className="block">
              <div id="routed-traffic" className="traffic-section">
                {trafficData.map(function (d, idx) {
                  return (
                    <RequestItem
                      key={idx}
                      handler={d.handler}
                      path={d.path}
                      statusCode={d.statusCode}
                      request={d.request}
                      response={d.response}
                    />
                  );
                })}
              </div>
            </section>
          </Box>
        </div>
      </main>
    </div>
  );
}
