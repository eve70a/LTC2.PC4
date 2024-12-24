import type { ISettingsService } from '../interfaces/ISettingsService';
import type { IProfileService } from '../interfaces/IProfileService';
import type { IRouteCheckerService } from '../interfaces/IRouteCheckerService';

import "reflect-metadata";
import { injectable, inject } from "inversify";
import { TYPES } from '../types/TYPES';
import { ClientSettings } from '../models/ClientSettings';
import { NotAuthorizedException } from '../exceptions/NotAuthorizedException';
import { LimitsExceededException } from '../exceptions/LimitsExceededException';

import { Routes } from '../models/Routes';
import { GetRoutesResponse } from '../models/GetRoutesResponse';
import { PresentationRoutes } from '../models/PresentationRoutes';

import axios, { AxiosError } from "axios";

@injectable()
export class RouteCheckerService implements IRouteCheckerService {
    
    @inject(TYPES.ISettingsService) 
    private _settingsService?: ISettingsService;

    @inject(TYPES.IProfileService) 
    private _profileService?: IProfileService;

    @inject(TYPES.ClientSettings) 
    private _clientSetting?: ClientSettings

    async checkGpx(file: File): Promise<Routes | undefined> {
        const token = await this._profileService?.getToken();
        
        if (token) {
            const settings = await this._settingsService?.getSettings();
            const url = settings?.routeServiceBaseUrl;
            const timeout = 3 * (this._clientSetting?.requestTimeout ?? 5000);
            
            try {
                const formData = new FormData();
                formData.append("file", file);

                const route = await axios.postForm<Routes>(url + '/api/Route/checkgpx', formData, {headers: {'Authorization': `Bearer ${token}`}, timeout: timeout});

                return route.data;
            } catch(error) {
                console.log(error);

                if(axios.isAxiosError(error)){
                    const axiosError = error as AxiosError;

                    if (axiosError.response?.status == 401){
                        throw new NotAuthorizedException("Missing or expired token.");
                    }
                }

                throw error;
            }
        } else {
            throw new NotAuthorizedException("Missing or expired token.");
        }
    }

    async listRoutes() : Promise<PresentationRoutes | undefined> {
        const token = await this._profileService?.getToken();
        
        if (token) {
            const settings = await this._settingsService?.getSettings();
            const url = settings?.routeServiceBaseUrl;
            const timeout = 3 * (this._clientSetting?.requestTimeout ?? 5000);

            try {
                const routes = await axios.get<GetRoutesResponse>(url + '/api/Route/list', {headers: {'Authorization': `Bearer ${token}`}, timeout: timeout});
    
                if (routes && routes.data && routes.data.limitsExceeded) {
                    throw new LimitsExceededException("Strava limits exceeded", routes.data);
                }

                return new PresentationRoutes(routes.data);
            } catch(error) {
                console.log(error);

                if(axios.isAxiosError(error)){
                    const axiosError = error as AxiosError;

                    if (axiosError.response?.status == 401){
                        throw new NotAuthorizedException("Missing or expired token.");
                    }
                }

                throw error;
            }
    } else {
            throw new NotAuthorizedException("Missing or expired token.");
        }
    }
}

