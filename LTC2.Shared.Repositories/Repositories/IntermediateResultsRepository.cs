using LTC2.Shared.Models.Domain;
using LTC2.Shared.Models.Settings;
using LTC2.Shared.Repositories.Interfaces;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

namespace LTC2.Shared.Repositories.Repositories
{
    public class IntermediateResultsRepository : IIntermediateResultsRepository
    {
        private readonly GenericSettings _genericSettings;
        private readonly ILogger _logger;

        private bool _isStoring;

        public IntermediateResultsRepository(GenericSettings genericSettings, ILogger<IntermediateResultsRepository> logger)
        {
            _genericSettings = genericSettings;
            _logger = logger;
        }

        public void Close()
        {
        }

        public void Open()
        {
            if (_genericSettings.IntermediateResultsFolder != null && !Directory.Exists(_genericSettings.IntermediateResultsFolder))
            {
                Directory.CreateDirectory(_genericSettings.IntermediateResultsFolder);
            }
        }

        public void StoreIntermedidateResult(CalculationResult calculationResult, bool multi)
        {
            if (_genericSettings.IntermediateResultsFolder != null)
            {
                try
                {
                    if (!_isStoring)
                    {
                        _isStoring = true;

                        var resultAsString = JsonConvert.SerializeObject(calculationResult, Formatting.Indented);
                        var fileName = Path.Combine(_genericSettings.IntermediateResultsFolder, GetFilename(calculationResult.AthleteId, multi));

                        File.WriteAllText(fileName, resultAsString);
                    }
                }
                catch (Exception e)
                {
                    _logger.LogWarning(e, $"Unable to store intermediate result: {e.Message}");
                }
                finally
                {
                    _isStoring = false;
                }
            }
        }

        public void Clear(long athleteId, bool multi = false)
        {
            if (_genericSettings.IntermediateResultsFolder != null)
            {
                var retryCount = 3;

                while (retryCount > 0)
                {
                    try
                    {
                        var fileName = Path.Combine(_genericSettings.IntermediateResultsFolder, GetFilename(athleteId, multi));

                        if (File.Exists(fileName))
                        {
                            File.Delete(fileName);
                        }

                        retryCount = 0;
                    }
                    catch (Exception e)
                    {
                        retryCount--;

                        if (retryCount > 0)
                        {
                            Task.Delay(2000).Wait();
                        }
                        else
                        {
                            _logger.LogWarning(e, $"Unable to store intermediate result: {e.Message}");

                        }
                    }
                }

            }
        }

        public bool HasIntermediateResult(long athleteId, bool multi)
        {
            if (_genericSettings.IntermediateResultsFolder != null)
            {
                var fileName = Path.Combine(_genericSettings.IntermediateResultsFolder, GetFilename(athleteId, multi));

                return File.Exists(fileName);
            }

            return false;
        }

        private string GetFilename(long athleteId, bool multi)
        {
            var multiSuffix = multi ? "-m" : string.Empty;
            return $"i{athleteId}{multiSuffix}.json";
        }

        public CalculationResult GetIntermediateResult(long athleteId, bool multi)
        {

            if (_genericSettings.IntermediateResultsFolder != null)
            {
                var fileName = Path.Combine(_genericSettings.IntermediateResultsFolder, GetFilename(athleteId, multi));

                _logger.LogInformation($"Reading intermediate result from {fileName}");

                if (File.Exists(fileName))
                {
                    try
                    {
                        var content = File.ReadAllText(fileName);

                        return JsonConvert.DeserializeObject<CalculationResult>(content);
                    }
                    catch (Exception e)
                    {
                        _logger.LogWarning(e, $"Unable to read intermediate result: {e.Message}");

                        try
                        {
                            File.Delete(fileName);
                        }
                        catch (Exception e2)
                        {
                            _logger.LogWarning(e2, $"Unable to delete intermediate result after exception: {e2.Message}");
                        }
                    }
                }
            }

            return null;
        }
    }
}
