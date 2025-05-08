using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using APBD7.DTOs;

namespace APBD7.Services
{
    public interface IClientService
    {
        Task<IEnumerable<TripDto>?> GetClientTripsAsync(int clientId);
        Task<int> CreateClientAsync(ClientDto client);
        Task<bool> RegisterClientToTripAsync(int clientId, int tripId);
        Task<bool> UnregisterClientFromTripAsync(int clientId, int tripId);
    }
}
