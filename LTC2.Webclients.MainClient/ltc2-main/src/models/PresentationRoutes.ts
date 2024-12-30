import { PresentationRoute } from './PresentationRoute';
import { GetRoutesResponse } from './GetRoutesResponse';

export class PresentationRoutes {
    
    constructor(routesResponse: GetRoutesResponse){
        if (routesResponse.routes){
            this.routes = routesResponse.routes.map( (sRoute) => new PresentationRoute(sRoute));
        }
    }

    public routes?: PresentationRoute[] = undefined;
}