import { createContext, FunctionComponent, useContext } from 'react';
import { ProxyData } from '../components/types';

interface IProxyState {
  currentRequest: ProxyData;
  setCurrentRequest?: (request: ProxyData) => void;
}
const defaultState = {
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
};

export const TinyContext = createContext<IProxyState>({
  currentRequest: defaultState
});
export const useTinyContext = () => useContext(TinyContext);
export const TinyContextProvider: FunctionComponent = ({ children }) => {
  return (
    <TinyContext.Provider value={{ currentRequest: defaultState }}>
      {children}
    </TinyContext.Provider>
  );
};
