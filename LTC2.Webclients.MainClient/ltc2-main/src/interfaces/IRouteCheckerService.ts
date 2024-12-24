import { PresentationRoutes } from '../models/PresentationRoutes';
import { Routes } from '../models/Routes'

export interface IRouteCheckerService {
    checkGpx(file: File):  Promise<Routes | undefined>;

    listRoutes(): Promise<PresentationRoutes | undefined>;
    
}