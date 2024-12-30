import { Limits } from '../models/Limits';

export class LimitsExceededException extends Error {
    
    public Limits?: Limits = undefined;

    constructor(message: string, limits: Limits) {
      super(message);

      this.Limits = limits;
  
      Object.setPrototypeOf(this, LimitsExceededException.prototype);
    }
  }