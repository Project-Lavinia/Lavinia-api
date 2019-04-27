using System;
using System.Collections.Generic;
using System.Linq;
using LaviniaApi.Data;
using LaviniaApi.Models;
using LaviniaApi.Utilities;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LaviniaApi.Controllers.v1
{
    /// <summary>
    ///     Controller that serves data regarding political elections in various levels, for the Mandater project at the
    ///     University of Oslo, department of political science
    /// </summary>
    [EnableCors("CorsPolicy")]
    [Produces("application/json")]
    [Route("api/v1.0.0/")]
    public class ElectionController : Controller
    {
        private readonly ElectionContext _context;
        private readonly ILogger _logger;

        /// <summary>
        ///     Constructor for the ElectionController, enables database access and logging
        /// </summary>
        /// <param name="context">ElectionContext object that allows access to the database</param>
        /// <param name="logger">Logger that gives information about the context of a log message</param>
        public ElectionController(ElectionContext context, ILogger<ElectionController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        ///     Default path method that returns a list of shallow Country objects, showing which countries the API has data on.
        ///     If deep is specified it returns all data.
        /// </summary>
        /// <param name="deep">Optional boolean parameter, the method returns a deep list if true</param>
        /// <returns>List of countries</returns>
        [ProducesResponseType(typeof(IEnumerable<Country>), 200)]
        [ProducesResponseType(500)]
        [HttpGet]
        public IActionResult GetCountries(bool? deep)
        {
            _logger.LogInformation("GetCountries called with parameter deep = " + deep);
            try
            {
                if (deep.HasValue && deep.Value)
                {
                    return Ok(
                        _context.Countries
                            .Include(c => c.ElectionTypes)
                            .ThenInclude(c => c.Elections)
                            .ThenInclude(c => c.Counties)
                            .ThenInclude(c => c.Results)
                    );
                }

                return Ok(_context.Countries);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Something went terribly wrong in GetCountries");
                return new StatusCodeResult(500);
            }
        }

        /// <summary>
        ///     Returns either a shallow or a deep Country object, where a deep object contains the entire hierarchy of data from
        ///     the country down.
        /// </summary>
        /// <param name="deep">Optional boolean parameter, the method returns a deep list if true</param>
        /// <param name="countryCode">Two character country code, ISO 3166-1 alpha-2</param>
        /// <returns>Country</returns>
        [ProducesResponseType(typeof(Country), 200)]
        [ProducesResponseType(500)]
        [HttpGet("{countryCode}")]
        public IActionResult GetCountry(string countryCode, bool? deep)
        {
            _logger.LogInformation(
                "GetCountry called with parameters countryCode = " + countryCode + ", deep = " + deep);
            try
            {
                if (deep.HasValue && deep.Value)
                {
                    return Ok(
                        _context.Countries
                            .Include(c => c.ElectionTypes)
                            .ThenInclude(c => c.Elections)
                            .ThenInclude(c => c.Counties)
                            .ThenInclude(c => c.Results)
                            .First(c => c.CountryCode == countryCode.ToUpperInvariant())
                    );
                }

                return Ok(
                    _context.Countries
                        .Include(c => c.ElectionTypes)
                        .First(c => c.CountryCode == countryCode.ToUpperInvariant())
                );
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Something has gone terribly wrong in GetCountry");
                return new StatusCodeResult(500);
            }
        }

        /// <summary>
        ///     Returns either a shallow or a deep ElectionType object, where a deep object contains the entire hierarchy of data
        ///     from the ElectionType down.
        /// </summary>
        /// <param name="deep">Optional boolean parameter, the method returns a deep list if true</param>
        /// <param name="electionCode">Two character election type code</param>
        /// <param name="countryCode">Two character country code, ISO 3166-1 alpha-2</param>
        /// <returns>ElectionType of a given country</returns>
        [ProducesResponseType(typeof(ElectionType), 200)]
        [ProducesResponseType(500)]
        [HttpGet("{countryCode}/{electionCode}")]
        public IActionResult GetElectionType(string countryCode, string electionCode, bool? deep)
        {
            _logger.LogInformation("GetElectionType called with parameters countryCode = " + countryCode +
                                   ", electionCode = " + electionCode + ", deep = " + deep);
            try
            {
                if (deep.HasValue && deep.Value)
                {
                    return Ok(
                        _context.Countries
                            .Include(c => c.ElectionTypes)
                            .ThenInclude(c => c.Elections)
                            .ThenInclude(c => c.Counties)
                            .ThenInclude(c => c.Results)
                            .First(c => c.CountryCode == countryCode.ToUpperInvariant())
                            .ElectionTypes
                            .First(c => c.InternationalName == ETNameUtilities.CodeToName(electionCode))
                    );
                }

                return Ok(
                    _context.Countries
                        .Include(c => c.ElectionTypes)
                        .ThenInclude(c => c.Elections)
                        .First(c => c.CountryCode == countryCode.ToUpperInvariant())
                        .ElectionTypes
                        .First(c => c.InternationalName == ETNameUtilities.CodeToName(electionCode))
                );
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Something has gone terribly wrong in GetElectionType");
                return new StatusCodeResult(500);
            }
        }

        /// <summary>
        ///     Returns either a shallow or a deep Election object, where a deep object contains the entire hierarchy of data from
        ///     the Election down.
        /// </summary>
        /// <param name="deep">Optional boolean parameter, the method returns a deep list if true</param>
        /// <param name="countryCode">Two character country code, ISO 3166-1 alpha-2</param>
        /// <param name="electionCode">Two character election type code</param>
        /// <param name="year">Four digit election year</param>
        /// <returns>Election of a given type and a given year for a given country</returns>
        [ProducesResponseType(typeof(Election), 200)]
        [ProducesResponseType(500)]
        [HttpGet("{countryCode}/{electionCode}/{year}")]
        public IActionResult GetElection(string countryCode, string electionCode, int year, bool? deep)
        {
            _logger.LogInformation("GetElectionType called with parameters countryCode = " + countryCode +
                                   ", electionCode = " + electionCode + ", year = " + year + ", deep = " + deep);
            try
            {
                if (deep.HasValue && deep.Value)
                {
                    return Ok(
                        _context.Countries
                            .Include(c => c.ElectionTypes)
                            .ThenInclude(c => c.Elections)
                            .ThenInclude(c => c.Counties)
                            .ThenInclude(c => c.Results)
                            .First(c => c.CountryCode == countryCode.ToUpperInvariant())
                            .ElectionTypes
                            .First(c => c.InternationalName == ETNameUtilities.CodeToName(electionCode))
                            .Elections
                            .First(c => c.Year == year)
                    );
                }

                return Ok(
                    _context.Countries
                        .Include(c => c.ElectionTypes)
                        .ThenInclude(c => c.Elections)
                        .ThenInclude(c => c.Counties)
                        .First(c => c.CountryCode == countryCode.ToUpperInvariant())
                        .ElectionTypes
                        .First(c => c.InternationalName == ETNameUtilities.CodeToName(electionCode))
                        .Elections
                        .First(c => c.Year == year)
                );
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Something has gone terribly wrong in GetElection");
                return new StatusCodeResult(500);
            }
        }
    }
}