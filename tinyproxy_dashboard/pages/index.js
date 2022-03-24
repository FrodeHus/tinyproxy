import Head from 'next/head'
import styles from '../styles/Home.module.css'
import { HubConnectionBuilder, LogLevel } from "@microsoft/signalr"
import { useEffect, useState } from 'react/cjs/react.production.min';



export default function Home() {
  const [trafficData, setTrafficData] = useState([]);
  const addTrafficData = (data) => setTrafficData(state => [...state, data])
  useEffect(() => {
    const connection = new HubConnectionBuilder()
      .withUrl("/proxyHub")
      .configureLogging(LogLevel.Information)
      .build();

    async function start() {
      try {
        await connection.start();
        console.log("SignalR Connected.");
      } catch (err) {
        console.log(err);
        setTimeout(start, 5000);
      }
    };

    connection.onclose(async () => {
      await start();
    });

    connection.on("GetTrafficSummary", function (path, statusCode, handler, request) {
      if (!path.hasValue || path.value === "") {
        path = "/"
      } else {
        path = path.value.toString()
      }
      
      let item = {
        path: path,
        statusCode: statusCode,
        routeHandler: handler,
        request: request
      }

      console.log(item)
      addTrafficData(item)
    
    });

    // Start the connection.
    start();
  }, [])

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
              <h4 className="title">
                TinyProxy Dashboard
              </h4>
            </div>
            <div className="wrapper">
              <section className="block">
                <div id="routed-traffic" className="traffic-section">
                    {trafficData.map(function (d, idx) {
                      return (<div key={idx} className={"traffic-block " + d.routeHandler.verb.method.toLowerCase()}>
                        <div className="traffic-summary">
                          <span className="upstream-server">{d.routeHandler.remoteServer}</span>
                          <span className="request-path-summary">{d.path}</span>
                        </div>
                      </div>)
                    })}
                </div>
              </section>
            </div>
          </section>
        </div>
      </main>
    </div>
  )
}
