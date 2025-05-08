using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using APBD7.Services;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Mvc;

namespace APBD7.Controllers
{

    public class TripsController : ControllerBase
    {
        private readonly ITripService _tripService;
        public TripsController(ITripService tripService)
        {
            _tripService = tripService;
        }

        [HttpGet]
        public async Task<IActionResult> GetTrips()
        {
            var result = await _tripService.GetTripsAsync();
            return Ok(result);
        }
    }
}
