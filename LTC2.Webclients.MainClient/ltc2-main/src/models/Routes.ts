import { Route } from './Route'
import { emptyString } from './Constants'
import { AbstractLimitsStravaResponse } from './AbstractLimitsStravaResponse'

export class Routes extends AbstractLimitsStravaResponse {
    public routeCollection: Route[] = [];

    public IsStravaRoute: boolean = false;
    public StravaRouteId: string = emptyString;
}