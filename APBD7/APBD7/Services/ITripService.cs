using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using APBD7.DTOs;

namespace APBD7.Services
{

    public interface ITripService
    {
        Task<IEnumerable<TripDto>> GetTripsAsync();
    }
}

