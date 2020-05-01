using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LaviniaApi.Data;
using LaviniaApi.Models;
using LaviniaApi.Utilities;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LaviniaApi.Controllers.v3
{
    /// <inheritdoc />
    /// <summary>
    ///     Controller that serves data regarding political elections in various levels, for the Mandater project at the
    ///     University of Oslo, department of political science
    /// </summary>
    [EnableCors("CorsPolicy")]
    [Produces("application/json")]
    [Route("api/v3.0.0/")]
    public class NOController : Controller
    {
        private const int DefaultNumberOfYears = 3;
        private const string DefaultPartyCode = "ALL";
        private const string DefaultDistrict = "ALL";

        private readonly NOContext _context;
        private readonly ILogger _logger;

        /// <summary>
        ///     Constructor for the ApiController, enables database access and logging
        /// </summary>
        /// <param name="context">NOContext object that allows access to the database</param>
        /// <param name="logger">Logger that gives information about the context of a log message</param>
        public NOController(NOContext context, ILogger<NOController> logger)
        {
            _context = context;
            _logger = logger;
        }


        /// <summary>
        ///     Returns a list of all election years available in the API.
        ///     The list is sorted so the most recent years are at the top of the list.
        /// </summary>
        /// <returns>List of all election years in the database</returns>
        [ProducesResponseType(typeof(IEnumerable<int>), 200)]
        [ProducesResponseType(500)]
        [HttpGet("years")]
        public IActionResult GetYears()
        {
            _logger.LogInformation("GetYears was called");
            try
            {
                List<int> years = _context.ElectionParameters.Select(ep => ep.ElectionYear).ToList();
                years.Sort((a, b) => b.CompareTo(a));

                return Ok(
                    years
                );
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Something has gone terribly wrong in GetYears");
                return new StatusCodeResult(500);
            }
        }


        /// <summary>
        ///     Returns a map from party code to party name for all parties in the API.
        /// </summary>
        /// <returns>Map from party code to party name for all parties.</returns>
        [ProducesResponseType(typeof(IDictionary<string, string>), 200)]
        [ProducesResponseType(500)]
        [HttpGet("parties")]
        public IActionResult GetParties()
        {
            _logger.LogInformation("GetParties was called");
            try
            {
                List<Party> parties = _context.Parties.ToList();
                IEnumerable<KeyValuePair<string, string>> partyMapping = parties.Select(p => new KeyValuePair<string, string>(p.Code, p.Name));
                Dictionary<string, string> partyDict = new Dictionary<string, string>(partyMapping);

                return Ok(
                    partyDict
                );
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Something has gone terribly wrong in GetParties");
                return new StatusCodeResult(500);
            }
        }


        /// <summary>
        ///     Returns a list of all districts for a given year in the API.
        /// </summary>
        /// <returns>List of all districts for a given year.</returns>
        [ProducesResponseType(typeof(IEnumerable<int>), 200)]
        [ProducesResponseType(500)]
        [HttpGet("districts")]
        public IActionResult GetDistricts(int? year)
        {
            _logger.LogInformation("GetDistricts was called with parameters year = " + year);
            try
            {
                return Ok(
                    _context.DistrictMetrics
                        .Where(dm => dm.ElectionYear == year || year == null)
                        .Select(dm => dm.District).ToHashSet().ToList()
                );
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Something has gone terribly wrong in GetDistricts");
                return new StatusCodeResult(500);
            }
        }


        /// <summary>
        ///     Returns a list of all Party votes that meet the required parameters.
        /// </summary>
        /// <param name="year">Four digit election year</param>
        /// <param name="partyCode">One to N character party code</param>
        /// <param name="district">Name of district</param>
        /// <returns>Party votes meeting the requirements</returns>
        [ProducesResponseType(typeof(IEnumerable<PartyVotes>), 200)]
        [ProducesResponseType(500)]
        [HttpGet("votes")]
        public IActionResult GetVotes(int? year, string partyCode = DefaultPartyCode, string district = DefaultDistrict)
        {
            _logger.LogInformation("GetVotes called with parameters year = " + year + ", partyCode = " + partyCode +
                                   ", district = " + district);
            try
            {
                return Ok(
                    _context.PartyVotes
                        .Where(pV =>
                            (pV.ElectionYear == year || year == null) &&
                            (pV.Party.Equals(partyCode) || partyCode.Equals(DefaultPartyCode)) &&
                            (pV.District.Equals(district) || district.Equals(DefaultDistrict))
                        )
                );
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Something has gone terribly wrong in GetVotes");
                return new StatusCodeResult(500);
            }
        }

        /// <summary>
        ///     Returns a list of all Party votes from a number of elections before the specified year.
        /// </summary>
        /// <param name="year">Four digit election year</param>
        /// <param name="partyCode">One to N character party code</param>
        /// <param name="district">Name of district</param>
        /// <param name="number">Number of elections</param>
        /// <returns>Party votes for a number of elections</returns>
        [ProducesResponseType(typeof(IEnumerable<PartyVotes>), 200)]
        [ProducesResponseType(500)]
        [HttpGet("votes/previous")]
        public IActionResult GetPreviousVotes(int? year, int? number, string partyCode = DefaultPartyCode,
            string district = DefaultDistrict)
        {
            _logger.LogInformation("GetVotes called with parameters year = " + year + ", partyCode = " + partyCode +
                                   ", district = " + district);
            try
            {
                int definedYear = year ?? DateTime.UtcNow.Year;
                int definedNumber = number ?? DefaultNumberOfYears;

                List<int> years = GetPreviousNYears(definedYear, definedNumber);

                return Ok(
                    _context.PartyVotes
                        .Where(pV =>
                            years.Contains(pV.ElectionYear) &&
                            (pV.Party.Equals(partyCode) || partyCode.Equals(DefaultPartyCode)) &&
                            (pV.District.Equals(district) || district.Equals(DefaultDistrict)))
                );
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Something has gone terribly wrong in GetVotes");
                return new StatusCodeResult(500);
            }
        }

        /// <summary>
        ///     Returns a list of all District metrics that matches the required parameters.
        /// </summary>
        /// <param name="year">Four digit election year</param>
        /// <param name="district">Name of district</param>
        /// <returns>District metrics matching the requirements</returns>
        [ProducesResponseType(typeof(IEnumerable<DistrictMetrics>), 200)]
        [ProducesResponseType(500)]
        [HttpGet("metrics")]
        public IActionResult GetMetrics(int? year, string district = DefaultDistrict)
        {
            _logger.LogInformation("GetMetrics called with parameters year = " + year + ", district = " + district);
            try
            {
                return Ok(
                    _context.DistrictMetrics
                        .Where(dM =>
                            (dM.ElectionYear == year || year == null) &&
                            (dM.District.Equals(district) || district.Equals(DefaultDistrict)))
                );
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Something has gone terribly wrong in GetMetrics");
                return new StatusCodeResult(500);
            }
        }

        /// <summary>
        ///     Returns a list of all District metrics from a number of elections before the specified year.
        /// </summary>
        /// <param name="year">Four digit election year</param>
        /// <param name="number">Number of District metrics to return</param>
        /// <param name="district">Name of district</param>
        /// <returns>District metrics for a number of elections</returns>
        [ProducesResponseType(typeof(IEnumerable<DistrictMetrics>), 200)]
        [ProducesResponseType(500)]
        [HttpGet("metrics/previous")]
        public IActionResult GetPreviousMetrics(int? year, int? number, string district = DefaultDistrict)
        {
            _logger.LogInformation("GetMetrics called with parameters year = " + year + ", district = " + district);
            try
            {
                int definedYear = year ?? DateTime.UtcNow.Year;
                int definedNumber = number ?? DefaultNumberOfYears;

                List<int> years = GetPreviousNYears(definedYear, definedNumber);

                return Ok(
                    _context.DistrictMetrics
                        .Where(dM =>
                            years.Contains(dM.ElectionYear) &&
                            (dM.District.Equals(district) || district.Equals(DefaultDistrict)))
                );
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Something has gone terribly wrong in GetMetrics");
                return new StatusCodeResult(500);
            }
        }

        /// <summary>
        ///     Returns a list of all Election parameters that matches the required parameters.
        /// </summary>
        /// <param name="year">Four digit election year</param>
        /// <returns>Election parameters matching the requirements</returns>
        [ProducesResponseType(typeof(IEnumerable<ElectionParametersV3>), 200)]
        [ProducesResponseType(500)]
        [HttpGet("parameters")]
        public IActionResult GetParameters(int? year)
        {
            _logger.LogInformation("GetParameters called with parameters year = " + year);
            try
            {
                return Ok(
                    _context.ElectionParametersV3
                        .Include(eP => eP.Algorithm.Parameters)
                        .Where(eP => eP.ElectionYear == year || year == null)
                );
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Something has gone terribly wrong in GetParameters");
                return new StatusCodeResult(500);
            }
        }


        /// <summary>
        ///     Returns a list of all Election parameters from a number of elections before the specified year.
        /// </summary>
        /// <param name="year">Four digit election year</param>
        /// <param name="number">Number of years to return</param>
        /// <returns>Election parameters for a number of years</returns>
        [ProducesResponseType(typeof(IEnumerable<ElectionParametersV3>), 200)]
        [ProducesResponseType(500)]
        [HttpGet("parameters/previous")]
        public IActionResult GetPreviousParameters(int? year, int? number)
        {
            _logger.LogInformation("GetParameters called with parameters year = " + year);
            try
            {
                int definedYear = year ?? DateTime.UtcNow.Year;
                int definedNumber = number ?? DefaultNumberOfYears;

                List<int> years = GetPreviousNYears(definedYear, definedNumber);

                return Ok(
                    _context.ElectionParametersV3
                        .Include(eP => eP.Algorithm.Parameters)
                        .Where(eP => years.Contains(eP.ElectionYear))
                );
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Something has gone terribly wrong in GetParameters");
                return new StatusCodeResult(500);
            }
        }

        private List<int> GetPreviousNYears(int year, int number)
        {
            List<int> years = _context.ElectionParameters.Where(eP => eP.ElectionYear <= year || year == 0)
                .Select(eP => eP.ElectionYear).ToList();

            years.Sort();
            years.RemoveRange(0, Math.Max(0, years.Count - number));

            return years;
        }
    }
}