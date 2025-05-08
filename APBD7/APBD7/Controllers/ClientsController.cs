using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using APBD7.DTOs;
using APBD7.Services;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Mvc;


namespace APBD7.Controllers
{

    public class ClientsController : ControllerBase
    {
        private readonly IClientService _clientService;

        public ClientsController(IClientService clientService)
        {
            _clientService = clientService;
        }

        [HttpGet("{id}/trips")]
        public async Task<IActionResult> GetClientTrips(int id)
        {
            var result = await _clientService.GetClientTripsAsync(id);
            if (result is null) return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateClient([FromBody] ClientDto client)
        {
            var createdId = await _clientService.CreateClientAsync(client);
            return CreatedAtAction(nameof(GetClientTrips), new { id = createdId }, null);
        }

        [HttpPut("{id}/trips/{tripId}")]
        public async Task<IActionResult> RegisterToTrip(int id, int tripId)
        {
            var success = await _clientService.RegisterClientToTripAsync(id, tripId);
            return success ? Ok() : BadRequest();
        }

        [HttpDelete("{id}/trips/{tripId}")]
        public async Task<IActionResult> UnregisterFromTrip(int id, int tripId)
        {
            var success = await _clientService.UnregisterClientFromTripAsync(id, tripId);
            return success ? Ok() : NotFound();
        }
    }
}
