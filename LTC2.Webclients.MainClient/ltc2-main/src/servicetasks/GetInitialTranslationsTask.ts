import type { ISeriveTask } from "../interfaces/IServiceTask";
import type { ITranslationService } from "../interfaces/ITranslationService";

import { inject, injectable } from "inversify";
import "reflect-metadata";
import { TYPES } from '../types/TYPES';

import { runsInMultiSportMode } from '../utils/Utils';


@injectable()
export class GetInitialTranslationsTask implements ISeriveTask {
    
    @inject(TYPES.ITranslationService) 
    private _translationService?: ITranslationService

    async execute(): Promise<void> {
        console.log('GetInitialTranslationsTask');

        await this._translationService?.loadText('nl');

        if (runsInMultiSportMode()) {
            await this._translationService?.mergeText('nl', 'multi');
        }
    }
}