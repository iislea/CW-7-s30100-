using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using APBD7.DTOs;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;


namespace APBD7.Services
{

    public class ClientService : IClientService
    {
        private readonly IConfiguration _config;
        public ClientService(IConfiguration config) => _config = config;

        public async Task<IEnumerable<TripDto>?> GetClientTripsAsync(int clientId)
        {
            var trips = new List<TripDto>();
            var connStr = _config.GetConnectionString("DefaultConnection");

            using var conn = new SqlConnection(connStr);
            using var cmd = new SqlCommand(@"
            SELECT t.IdTrip, t.Name, t.Description, t.DateFrom, t.DateTo, t.MaxPeople, ct.RegisteredAt, ct.PaymentDate
            FROM Client_Trip ct
            JOIN Trip t ON t.IdTrip = ct.IdTrip
            WHERE ct.IdClient = @IdClient
        ", conn);

            cmd.Parameters.AddWithValue("@IdClient", clientId);

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                trips.Add(new TripDto
                {
                    IdTrip = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Description = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                    DateFrom = reader.GetDateTime(3),
                    DateTo = reader.GetDateTime(4),
                    MaxPeople = reader.GetInt32(5)
                });
            }
            return trips.Count == 0 ? null : trips;
        }

        public async Task<int> CreateClientAsync(ClientDto client)
        {
            var connStr = _config.GetConnectionString("DefaultConnection");

            using var conn = new SqlConnection(connStr);
            using var cmd = new SqlCommand(@"
            INSERT INTO Client (FirstName, LastName, Email, Telephone, Pesel)
            VALUES (@FirstName, @LastName, @Email, @Telephone, @Pesel);
            SELECT SCOPE_IDENTITY();
        ", conn);

            cmd.Parameters.AddWithValue("@FirstName", client.FirstName);
            cmd.Parameters.AddWithValue("@LastName", client.LastName);
            cmd.Parameters.AddWithValue("@Email", client.Email);
            cmd.Parameters.AddWithValue("@Telephone", client.Telephone);
            cmd.Parameters.AddWithValue("@Pesel", client.Pesel);

            await conn.OpenAsync();
            var id = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(id);
        }

        public async Task<bool> RegisterClientToTripAsync(int clientId, int tripId)
        {
            var connStr = _config.GetConnectionString("DefaultConnection");
            using var conn = new SqlConnection(connStr);
            await conn.OpenAsync();

            using var checkCmd = new SqlCommand("SELECT COUNT(*) FROM Client_Trip WHERE IdClient = @ClientId AND IdTrip = @TripId", conn);
            checkCmd.Parameters.AddWithValue("@ClientId", clientId);
            checkCmd.Parameters.AddWithValue("@TripId", tripId);
            var exists = (int)await checkCmd.ExecuteScalarAsync() > 0;
            if (exists) return false;

            using var cmd = new SqlCommand(@"
            INSERT INTO Client_Trip (IdClient, IdTrip, RegisteredAt)
            VALUES (@ClientId, @TripId, @Now)
        ", conn);

            cmd.Parameters.AddWithValue("@ClientId", clientId);
            cmd.Parameters.AddWithValue("@TripId", tripId);
            cmd.Parameters.AddWithValue("@Now", DateTime.UtcNow.Year * 10000 + DateTime.UtcNow.Month * 100 + DateTime.UtcNow.Day);

            var rows = await cmd.ExecuteNonQueryAsync();
            return rows > 0;
        }

        public async Task<bool> UnregisterClientFromTripAsync(int clientId, int tripId)
        {
            var connStr = _config.GetConnectionString("DefaultConnection");
            using var conn = new SqlConnection(connStr);
            using var cmd = new SqlCommand(@"DELETE FROM Client_Trip WHERE IdClient = @ClientId AND IdTrip = @TripId", conn);

            cmd.Parameters.AddWithValue("@ClientId", clientId);
            cmd.Parameters.AddWithValue("@TripId", tripId);

            await conn.OpenAsync();
            var rows = await cmd.ExecuteNonQueryAsync();
            return rows > 0;
        }
    }
}
