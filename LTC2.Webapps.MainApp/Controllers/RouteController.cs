using Elasticsearch.Net;
using LTC2.Shared.ActivityFormats.Gpx.Utils;
using LTC2.Shared.Models.Domain;
using LTC2.Shared.Repositories.Interfaces;
using LTC2.Shared.StravaConnector.Exceptions;
using LTC2.Shared.StravaConnector.Interfaces;
using LTC2.Shared.StravaConnector.Models.Requests;
using LTC2.Webapps.MainApp.Models;
using LTC2.Webapps.MainApp.Models.Requests;
using LTC2.Webapps.MainApp.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LTC2.Webapps.MainApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RouteController : ControllerBase
    {
        private readonly AppSettings _appSettings;
        private readonly ILogger<RouteController> _logger;
        private readonly IMapRepository _mapRepository;
        private readonly TokenUtils _tokenUtils;
        private readonly IStravaConnector _stravaConnector;

        private bool _isMapRepositoryOpen = false;
        private object _mapRepositoryLock = new object();

        public RouteController(
            TokenUtils tokenUtils,
            ILogger<RouteController> logger,
            IMapRepository mapRepository,
            IStravaConnector stravaConnector,
            IStravaHttpProxy stravaHttpProxy,
            AppSettings appSettings)
        {
            _appSettings = appSettings;
            _mapRepository = mapRepository;
            _tokenUtils = tokenUtils;
            _stravaConnector = stravaConnector;
            _logger = logger;
        }

        [HttpPost]
        [Authorize]
        [Route("checkgpx")]
        public IActionResult CheckGpx(IFormFile file)
        {
            var fileName = ReadGpxFile(file);

            var response = CheckGpxFile(fileName);

            return Ok(response);
        }

        [HttpGet]
        [Authorize]
        [Route("checkstravaroute")]
        public async Task<IActionResult> CheckStravaRoute([FromQuery] CheckStravaRouteRequest request)
        {
            var authHeader = _tokenUtils.GetAuthenticationHeader(HttpContext.Request);
            var token = authHeader?.Parameter;

            if (token != null)
            {
                if (_tokenUtils.ValidateToken(token))
                {
                    var athleteId = _tokenUtils.GetProfileFormToken(token).AthleteId;

                    var gpxRequest = new GetRouteDetailsAsGpxRequest();
                    gpxRequest.AthleteId = Convert.ToInt64(athleteId);
                    gpxRequest.RouteId = request.RouteId;

                    try
                    {
                        var gpx = await _stravaConnector.GetRouteDetailsAsGpx(gpxRequest);

                        if (gpx != null && gpx.Gpx != null)
                        {
                            var gpxFile = WriteGpxFile(gpx.Gpx);
                            var response = CheckGpxFile(gpxFile);

                            response.IsStravaRoute = true;
                            response.StravaRouteId = request.RouteId;

                            return Ok(response);
                        }
                    } 
                    catch (StravaTooManyDailyRequestsException ex)
                    {
                        var response = new Routes();
                        response.LimitInfo = new LimitInfo();

                        response.LimitInfo.LimitExceeded = true;

                        if (ex.Limits.HasLimits)
                        {
                            response.LimitInfo.QuarterRateLimit = ex.Limits.QuarterRateLimit;
                            response.LimitInfo.QuarterRateUsage = ex.Limits.QuarterRateUsage;
                            response.LimitInfo.DayRateLimit = ex.Limits.DayRateLimit;
                            response.LimitInfo.DayRateUsage = ex.Limits.DayRateUsage;
                        }

                        return Ok(response);
                    }

                    return NotFound();
                }
            }

            return Unauthorized();
        }

        [HttpGet]
        [Authorize]
        [Route("list")]
        public async Task<IActionResult> GetRoutes()
        {
            var authHeader = _tokenUtils.GetAuthenticationHeader(HttpContext.Request);
            var token = authHeader?.Parameter;

            if (token != null)
            {
                if (_tokenUtils.ValidateToken(token))
                {
                    var athleteId = _tokenUtils.GetProfileFormToken(token).AthleteId;

                    var request = new GetRoutesRequest();
                    request.AthleteId = Convert.ToInt64(athleteId);

                    var routes = await _stravaConnector.GetRoutes(request);

                    return Ok(routes);
                }
            }

            return Unauthorized();
        }

        private string WriteGpxFile(string gpx)
        {
            EnsureTempFolderExists();


            try
            {
                var uniqueId = Guid.NewGuid().ToString();
                var gpxName = Path.Combine(_appSettings.TempRoutesFolder, $"{uniqueId}.gpx");

                System.IO.File.WriteAllText(gpxName, gpx);

                return gpxName;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error writing gpx file");

                throw;
            }
        }


        private string ReadGpxFile(IFormFile file)
        {
            EnsureTempFolderExists();

            CleanOldGpxFiles();

            try
            {
                var uniqueId = Guid.NewGuid().ToString();
                var gpxName = Path.Combine(_appSettings.TempRoutesFolder, $"{uniqueId}.gpx");

                using (FileStream fs = System.IO.File.Create(gpxName))
                {
                    file.CopyTo(fs);

                    fs.Flush();
                }

                return gpxName;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error reading gpx file");

                throw;
            }
        }

        private Routes CheckGpxFile(string file)
        {
            try
            {
                EnsureMapRepository();

                var tracks = GpxCoordinateUtils.CreateLinestringForGpxTrack(file);
                var response = new Routes();

                foreach (var track in tracks)
                {
                    // check each track and consolidate results
                    var coordinates = new List<List<double>>();

                    foreach (var trackCoordinate in track.Coordinates)
                    {
                        var coordinate = new List<double>
                        {
                            trackCoordinate.Y,
                            trackCoordinate.X
                        };

                        coordinates.Add(coordinate);
                    }

                    var places = _mapRepository.CheckTrack(coordinates);

                    var route = new Route
                    {
                        Coordinates = coordinates,
                        Places = places.Select(p => p.Id).ToList()
                    };

                    response.RouteCollection.Add(route);
                }

                return response;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error checking gpx file");

                throw;
            }
        }

        private void EnsureTempFolderExists()
        {
            try
            {
                if (!Directory.Exists(_appSettings.TempRoutesFolder))
                {
                    Directory.CreateDirectory(_appSettings.TempRoutesFolder);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error ensuring temp folder");

                throw;
            }
        }

        private void CleanOldGpxFiles()
        {
            try
            {
                var files = Directory.GetFiles(_appSettings.TempRoutesFolder, "*.gpx");

                foreach (var file in files)
                {
                    var expired = System.IO.File.GetCreationTime(file).ToUniversalTime().AddHours(6);
                    var now = DateTime.UtcNow;

                    if (now > expired)
                    {
                        System.IO.File.Delete(file);
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error cleaning old gpx files");
             
                throw;
            }
        }

        private void EnsureMapRepository()
        {
            lock (_mapRepositoryLock)
            {
                if (!_isMapRepositoryOpen)
                {
                    _mapRepository.Open();

                    _isMapRepositoryOpen = true;

                    _logger.LogInformation("Map repository opened");
                }
            }
        }
    }
}
