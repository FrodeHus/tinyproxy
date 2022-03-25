import { FunctionComponent } from "react";
import {RouteHandler} from './types'
type ProxyConfigProps = {
    handlers: RouteHandler[];
}

export const ProxyConfigView: FunctionComponent<ProxyConfigProps> = ({handlers}) => {
    return (
        <div></div>
    )
}