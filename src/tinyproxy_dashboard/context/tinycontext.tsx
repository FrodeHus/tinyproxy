import { createContext, FunctionComponent, useContext, useState } from 'react';
import { ProxyData } from '../components/types';

interface IProxyState {
  currentRequest: ProxyData;
  setSelectedRequest?: (request: ProxyData) => void;
}
const defaultState: IProxyState = {
  currentRequest: {
    requestId: -1,
    path: '',
    statusCode: 0,
    handler: {
      method: '',
      serverName: '',
      serverUrl: '',
      prefix: '',
      preferred: false,
      swaggerEndpoint: '',
      routes: []
    },
    request: {
      headers: {},
      content: ''
    },
    response: {
      headers: [],
      content: ''
    }
  }
};

export const TinyContext = createContext<IProxyState>(defaultState);
export const useTinyContext = () => useContext(TinyContext);
export const TinyContextProvider: FunctionComponent = ({ children }) => {
  const [currentRequest, setCurrentRequest] = useState(defaultState.currentRequest)
  const setSelectedRequest = (request: ProxyData) => {
    setCurrentRequest(request);
  }
  return (
    <TinyContext.Provider value={{ currentRequest, setSelectedRequest }}>
      {children}
    </TinyContext.Provider>
  );
};
