import { Typography } from "@mui/material";
import { FunctionComponent } from "react";
import { Request } from './types';
type InspectorQuickViewProps = {
    request: Request;
}

function getPropertyByPath(request: Request, path: string) {
    if (!path) {
        return request;
    }

    var prop, props = path.split('.');
}

export const InspectorQuickView: FunctionComponent<InspectorQuickViewProps> = ({ request }) => {

    return (
        <Typography>{request['path']}</Typography>
    )
}