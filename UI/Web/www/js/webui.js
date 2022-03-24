"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/proxyHub").build();


connection.on("GetTrafficSummary", function (path, statusCode, handler, request) {
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
    appendTrafficDetail(trafficEntry, path, request);
    document.getElementById("routed-traffic").appendChild(trafficEntry);
    updateScroll();
});

function appendTrafficDetail(parent, path, request){
    let details = document.createElement("div");
    details.className = "traffic-details";
    let headerPanel = document.createElement("div");
    headerPanel.className = "panel";
    details.appendChild(headerPanel);
    let title = document.createElement("h3");
    title.className = "panel-title";
    title.textContent ="HTTP Headers";
    headerPanel.appendChild(title);
    let headerList = document.createElement("ul");
    for (const [key, value] of Object.entries(request.headers)) {
        let item = document.createElement("li");
        let headerName = document.createElement("strong");
        headerName.textContent = key + " :";
        let headerValue = document.createElement("span");
        headerValue.textContent = value;
        item.appendChild(headerName);
        item.appendChild(headerValue);
        headerList.appendChild(item);
    }
    details.appendChild(headerList);
    parent.appendChild(details);
}
function updateScroll(){
    var element = document.getElementById("routed-traffic");
    element.scrollTop = element.scrollHeight;
}

connection.start();

