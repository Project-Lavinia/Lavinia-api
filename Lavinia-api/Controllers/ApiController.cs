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

namespace LaviniaApi.Controllers
{
    /// <inheritdoc />
    /// <summary>
    ///     Controller that serves data regarding political elections in various levels, for the Mandater project at the
    ///     University of Oslo, department of political science
    /// </summary>
    [EnableCors("CorsPolicy")]
    [Produces("application/json")]
    [Route("api/v2.0.0/")]
    public class ApiController : Controller
    {
        private readonly ElectionContext _context;
        private readonly ILogger _logger;

        /// <summary>
        ///     Constructor for the ApiController, enables database access and logging
        /// </summary>
        /// <param name="context">ElectionContext object that allows access to the database</param>
        /// <param name="logger">Logger that gives information about the context of a log message</param>
        public ApiController(ElectionContext context, ILogger<ApiController> logger)
        {
            _context = context;
            _logger = logger;
        }


        /// <summary>
        ///     Returns a list of all Party votes that meet the required parameters.
        /// </summary>
        /// <param name="year">Four digit election year</param>
        /// <param name="partyCode">One to N character party code</param>
        /// <param name="district">Name of district</param>
        /// <returns>Party votes for a given year</returns>
        [ProducesResponseType(typeof(IEnumerable<PartyVotes>), 200)]
        [ProducesResponseType(500)]
        [HttpGet("votes")]
        public IActionResult GetVotes(int year = 0, string partyCode = "ALL", string district = "ALL")
        {
            _logger.LogInformation("GetVotes called with parameters year = " + year + ", partyCode = " + partyCode + ", district = " + district);
            try
            {
                return Ok(
                    _context.PartyVotes
                        .Where(pV => (pV.ElectionYear == year || year == 0) && (pV.Party.Equals(partyCode) || partyCode.Equals("ALL")) && (pV.District.Equals(district) || district.Equals("ALL")))
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
        public IActionResult GetMetrics(int year = 0, string district = "ALL")
        {
            _logger.LogInformation("GetMetrics called with parameters year = " + year + ", district = " + district);
            try
            {
                return Ok(
                    _context.DistrictMetrics
                        .Where(dM => (dM.ElectionYear == year || year == 0) && (dM.District.Equals(district) || district.Equals("ALL")))
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
        [ProducesResponseType(typeof(IEnumerable<ElectionParameters>), 200)]
        [ProducesResponseType(500)]
        [HttpGet("parameters")]
        public IActionResult GetParameters(int year = 0)
        {
            _logger.LogInformation("GetParameters called with parameters year = " + year);
            try
            {
                return Ok(
                    _context.ElectionParameters
                        .Include(eP => eP.Algorithm)
                        .Where(eP => eP.ElectionYear == year || year == 0)
                );
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Something has gone terribly wrong in GetParameters");
                return new StatusCodeResult(500);
            }
        }
    }
}
