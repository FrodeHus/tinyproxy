import { Component, useEffect, useState } from 'react/cjs/react.production.min';

export default class TrafficEntry extends Component {
  constructor(props) {
    super(props)
  }
  render() {
    let statusClass = "ok";
    if (this.props.statusCode < 200 || this.props.statusCode >= 400) {
      statusClass = "error";
    }
    return (
      <div className={"traffic-block " + this.props.handler.verb.method.toLowerCase()}>
        <div className="traffic-summary">
          <span className="upstream-server">{this.props.handler.remoteServer}</span>
          <span className="request-path-summary">{this.props.path}</span>
          <span className={"request-status " + statusClass}>{this.props.statusCode}</span>
        </div>
      </div>
    );
  }
}