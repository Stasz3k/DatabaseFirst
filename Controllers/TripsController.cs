namespace DatabaseFirst.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using global::DatabaseFirst.Data;
    using global::DatabaseFirst.Models;

    namespace DatabaseFirst.Controllers
    {
        [Route("api/[controller]")]
        [ApiController]
        public class TripsController : ControllerBase
        {
            private readonly s24412Context _context;

            public TripsController(s24412Context context)
            {
                _context = context;
            }

            [HttpGet]
            public async Task<ActionResult<TripResponse>> GetTrips(int page = 1, int pageSize = 10)
            {
                var trips = await _context.Trips
                    .OrderByDescending(t => t.DateFrom)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var tripResponse = new TripResponse
                {
                    PageNum = page,
                    PageSize = pageSize,
                    AllPages = (int)Math.Ceiling(await _context.Trips.CountAsync() / (double)pageSize),
                    Trips = trips
                };

                return Ok(tripResponse);
            }
        }
    }

}
