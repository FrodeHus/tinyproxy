"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/proxyHub").build();


connection.on("GetTrafficSummary", function (path, statusCode, handler) {
    let trafficEntry = document.createElement("div");
    trafficEntry.className = "traffic-block " + handler.verb.method.toLowerCase();
    let summary = document.createElement("div");
    summary.className = "traffic-summary";
    
    let upstream = document.createElement("span");
    upstream.className = "upstream-server";
    upstream.textContent = handler.remoteServer;
    summary.appendChild(upstream);
    
    let pathSummary = document.createElement("span");
    pathSummary.className = "request-path-summary";
    if(!path.hasValue || path.value === ""){
        path = "/";
    }else{
        path = path.value.toString();
    }
    
    pathSummary.textContent = path;
    summary.appendChild(pathSummary);

    let requestStatus = document.createElement("span");
    let statusClass = "ok";
    if(statusCode < 200 || statusCode >= 400){
        statusClass = "error";
    }
    
    requestStatus.className = "request-status " + statusClass;
    requestStatus.textContent = statusCode;
    summary.appendChild(requestStatus);


    trafficEntry.appendChild(summary);
    document.getElementById("routed-traffic").appendChild(trafficEntry);

    li.textContent = `${user} says ${message}`;
});

connection.start();

