import Head from 'next/head';
import { HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import { useEffect, useState } from 'react';
import { RequestItem } from '../components';
import AppBar from '@mui/material/AppBar';
type RequestData = {
  headers: { name: string; value: string }[];
  content: string;
};
type RouteHandler = {
  method: string;
  serverName: string;
  serverUrl: string;
  prefix: string;
};

type ProxyData = {
  handler: RouteHandler;
  path: string;
  statusCode: number;
  request: RequestData;
};
export const Home: React.FC = ({}) => {
  const [trafficData, setTrafficData] = useState<ProxyData[]>([]);
  const addTrafficData = (data: ProxyData) =>
    setTrafficData((state) => [...state, data]);
  useEffect(() => {
    const connection = new HubConnectionBuilder()
      .withUrl('/proxyHub')
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
      'GetTrafficSummary',
      function (path, statusCode, handler, request) {
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
            serverUrl: handler.remoteUrl,
            prefix: handler.prefix
          },
          request: {
            headers: request.headers.map(
              (kvp: { key: string; value: string }) => {
                return { name: kvp.key, value: kvp.value };
              }
            ),
            content: ''
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
        <div id="tinyproxy-ui">
          <section className="tinyproxy-ui">
            <div className="topbar">
              <h4 className="title">TinyProxy Dashboard</h4>
            </div>
            <div className="wrapper">
              <section className="block">
                <div id="routed-traffic" className="traffic-section">
                  {trafficData.map(function (d, idx) {
                    return (
                      <RequestItem
                        key={idx}
                        handler={d.handler}
                        path={d.path}
                        statusCode={d.statusCode}
                      />
                    );
                  })}
                </div>
              </section>
            </div>
          </section>
        </div>
      </main>
    </div>
  );
};
