import {
  HubConnection,
  HubConnectionBuilder,
  LogLevel
} from '@microsoft/signalr';
import {
  createContext,
  FunctionComponent,
  useContext,
  useEffect,
  useState
} from 'react';
import { Request } from '../components/types';

interface IProxyState {
  currentRequest: Request;
  setSelectedRequest?: (request: Request) => void;
  hubConnection?: HubConnection;
}
const defaultState: IProxyState = {
  currentRequest: {
    id: -1,
    path: '',
    method: 'GET',
    requestHeaders: {},
    responseHeaders: {},
    statusCode: 0,
    handler: {
      verb: '',
      remoteServer: '',
      remoteServerBaseUrl: '',
      prefix: '',
      preferred: false,
      swaggerEndpoint: '',
      routes: []
    }
  }
};

export const TinyContext = createContext<IProxyState>(defaultState);
export const useTinyContext = () => useContext(TinyContext);
export const TinyContextProvider: FunctionComponent = ({ children }) => {
  const [currentRequest, setCurrentRequest] = useState(
    defaultState.currentRequest
  );
  const [hubConnection, setHubConnection] = useState<HubConnection>();
  const setSelectedRequest = (request: Request) => {
    setCurrentRequest(request);
  };

  useEffect(() => {
    if (hubConnection) return;
    const connection = new HubConnectionBuilder()
      .withUrl('http://localhost:5000/tinyproxy/hub')
      .configureLogging(LogLevel.Information)
      .withAutomaticReconnect()
      .build();
    setHubConnection(connection);
  }, [hubConnection]);

  useEffect(() => {
    if (hubConnection) {
      hubConnection.start();
    }
  }, [hubConnection]);

  return (
    <TinyContext.Provider
      value={{ currentRequest, setSelectedRequest, hubConnection }}
    >
      {children}
    </TinyContext.Provider>
  );
};
