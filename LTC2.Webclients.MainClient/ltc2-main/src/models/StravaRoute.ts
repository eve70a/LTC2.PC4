import { emptyString } from './Constants'

export class StravaRoute {
    public id: number = -1;

    public routeId: string = emptyString;

    public name: string = emptyString;

    public timestamp?: number = undefined;

    public distance: number = -1.0;
}