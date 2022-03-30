import { createContext, FunctionComponent, useContext, useState } from 'react';
import { Request } from '../components/types';

interface IProxyState {
  currentRequest: Request;
  setSelectedRequest?: (request: Request) => void;
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
      method: '',
      serverName: '',
      serverUrl: '',
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
  const setSelectedRequest = (request: Request) => {
    setCurrentRequest(request);
  };
  return (
    <TinyContext.Provider value={{ currentRequest, setSelectedRequest }}>
      {children}
    </TinyContext.Provider>
  );
};
