import { StravaRoute } from './StravaRoute'
import { AbstractLimitsStravaResponse } from './AbstractLimitsStravaResponse'

export class GetRoutesResponse extends AbstractLimitsStravaResponse {
    public routes?: StravaRoute[] = undefined;
}
