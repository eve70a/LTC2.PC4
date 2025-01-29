using LTC2.Shared.Models.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LTC2.Shared.Repositories.Interfaces
{
    public interface IScoresRepository
    {
        public void Open();

        public Task StoreScores(bool isRefresh, CalculationResult calculationResult, bool multi);

        public Task<Visit> GetMostRecentVisit(long athleteId, bool multi);

        public Task<CalculationResult> GetMostRecentResult(long athleteId, bool multi);

        public Task<Dictionary<string, Track>> GetTracks(long athleteId, bool multi);

        public Task<Track> GetAlltimeTrackForPlace(long athleteId, string placeId, bool detailed, bool multi);

        public Task<List<Track>> GetAlltimeTracksForAllPlaces(long athleteId, bool multi);

    }
}
