import { FunctionComponent } from 'react';
import { useTinyContext } from '../context/tinycontext';

type InspectorProps = {};

export const Inspector: FunctionComponent<InspectorProps> = () => {
  const { currentRequest } = useTinyContext();
  return <div></div>;
};
