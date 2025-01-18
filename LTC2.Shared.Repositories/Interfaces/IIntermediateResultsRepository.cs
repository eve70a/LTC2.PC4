using LTC2.Shared.Models.Domain;

namespace LTC2.Shared.Repositories.Interfaces
{
    public interface IIntermediateResultsRepository : IBaseRepository
    {
        public void StoreIntermedidateResult(CalculationResult calculationResult, bool multi);

        public void Clear(long athleteId, bool multi);

        public bool HasIntermediateResult(long athleteId, bool multi);

        public CalculationResult GetIntermediateResult(long athleteId, bool multi);

    }
}
