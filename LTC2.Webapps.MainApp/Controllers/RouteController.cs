using LTC2.Shared.ActivityFormats.Gpx.Utils;
using LTC2.Shared.Models.Domain;
using LTC2.Shared.Repositories.Interfaces;
using LTC2.Webapps.MainApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LTC2.Webapps.MainApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RouteController : ControllerBase
    {
        private readonly AppSettings _appSettings;
        private readonly ILogger<RouteController> _logger;
        private readonly IMapRepository _mapRepository;

        private bool _isMapRepositoryOpen = false;
        private object _mapRepositoryLock = new object();

        public RouteController(
            ILogger<RouteController> logger,
            IMapRepository mapRepository,
            AppSettings appSettings)
        {
            _appSettings = appSettings;
            _mapRepository = mapRepository;
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
