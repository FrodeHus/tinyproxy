export default function TrafficEntry(props) {
    let statusClass = "ok";
    let {handler,path,statusCode} = props;
    if (statusCode < 200 || statusCode >= 400) {
      statusClass = "error";
    }
    return (
      <div className={"traffic-block " + handler.verb.method.toLowerCase()}>
        <div className="traffic-summary">
          <span className="upstream-server">{handler.remoteServer}</span>
          <span className="request-path-summary">{path}</span>
          <span className={"request-status " + statusClass}>{statusCode}</span>
        </div>
      </div>
    );
  }