import Head from 'next/head'
import styles from '../styles/Home.module.css'
import { HubConnectionBuilder, LogLevel } from "@microsoft/signalr"
import { useEffect } from 'react/cjs/react.production.min';



export default function Home() {
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

    });

    // Start the connection.
    start();
  }, [])

  return (
    <div className={styles.container}>
      <Head>
        <title>TinyProxy Dashboard</title>
        <link rel="icon" href="/favicon.ico" />
      </Head>

      <main className={styles.main}>
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

                </div>
              </section>
            </div>
          </section>
        </div>
      </main>
    </div>
  )
}
