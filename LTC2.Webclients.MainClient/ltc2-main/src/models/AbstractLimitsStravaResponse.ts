
import { Limits } from './Limits';

export abstract class AbstractLimitsStravaResponse extends Limits {
    public limitsExceeded: boolean = false;

    public hasLimits: boolean = false;
}
