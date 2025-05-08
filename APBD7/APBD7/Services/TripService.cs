using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using APBD7.DTOs;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace APBD7.Services
{


    public class TripService : ITripService
    {
        private readonly IConfiguration _config;
        public TripService(IConfiguration config) => _config = config;

        public async Task<IEnumerable<TripDto>> GetTripsAsync()
        {
            var trips = new List<TripDto>();
            var connStr = _config.GetConnectionString("DefaultConnection");

            using var conn = new SqlConnection(connStr);
            using var cmd = new SqlCommand(@"
            SELECT t.IdTrip, t.Name, t.Description, t.DateFrom, t.DateTo, t.MaxPeople, c.Name as CountryName
            FROM Trip t
            LEFT JOIN Country_Trip ct ON t.IdTrip = ct.IdTrip
            LEFT JOIN Country c ON ct.IdCountry = c.IdCountry
        ", conn);

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var id = reader.GetInt32(0);
                var trip = trips.FirstOrDefault(x => x.IdTrip == id);
                if (trip == null)
                {
                    trip = new TripDto
                    {
                        IdTrip = id,
                        Name = reader.GetString(1),
                        Description = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                        DateFrom = reader.GetDateTime(3),
                        DateTo = reader.GetDateTime(4),
                        MaxPeople = reader.GetInt32(5),
                        Countries = new List<string>()
                    };
                    trips.Add(trip);
                }
                if (!reader.IsDBNull(6))
                    trip.Countries.Add(reader.GetString(6));
            }
            return trips;
        }
    }
}
