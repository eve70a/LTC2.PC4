import { Route } from './Route'
import { emptyString } from './Constants'
import { LimitsInfo } from './LimitsInfo';

export class Routes {

    public isStravaRoute: boolean = false;
    public stravaRouteId: string = emptyString;
    public limitInfo : LimitsInfo | undefined = undefined;

    public routeCollection: Route[] = [];

}