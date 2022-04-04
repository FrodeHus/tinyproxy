import '../styles/globals.css';
import '@fontsource/roboto/300.css';
import '@fontsource/roboto/500.css';
import '@fontsource/roboto/700.css';
import { AppProps } from 'next/app';
import React from 'react';
import { TinyContextProvider } from '../context/tinycontext';

function App({ Component, pageProps }: AppProps) {
  return (
    <TinyContextProvider>
      <Component {...pageProps} />
    </TinyContextProvider>
  );
}

export default App;
