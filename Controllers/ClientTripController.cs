using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using DatabaseFirst.Data;
using DatabaseFirst.Models;

namespace DatabaseFirst.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientTripController : ControllerBase
    {
        private readonly s24412Context _context;

        public ClientTripController(s24412Context context)
        {
            _context = context;
        }

        [HttpPost("{idTrip}")]
        public async Task<IActionResult> AssignClientToTrip(int idTrip, ClientRequest clientRequest)
        {
            var existingClient = await _context.Clients.FirstOrDefaultAsync(c => c.Pesel == clientRequest.Pesel);
            if (existingClient != null)
            {
                return BadRequest("Client with provided PESEL already exists.");
            }

            var trip = await _context.Trips.FindAsync(idTrip);
            if (trip == null)
            {
                return NotFound("Trip not found.");
            }

            if (trip.DateFrom <= DateTime.Now)
            {
                return BadRequest("Cannot assign to past trips.");
            }

            var clientTrip = new ClientTrip
            {
                IdTrip = idTrip,
                IdClientNavigation = new Client
                {
                    FirstName = clientRequest.FirstName,
                    LastName = clientRequest.LastName,
                    Email = clientRequest.Email,
                    Telephone = clientRequest.Telephone,
                    Pesel = clientRequest.Pesel
                },
                RegisteredAt = DateTime.Now,
                PaymentDate = clientRequest.PaymentDate
            };

            _context.ClientTrips.Add(clientTrip);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetClientTrip), new { idClient = clientTrip.IdClient, idTrip = clientTrip.IdTrip }, clientTrip);
        }

        private async Task<ActionResult<ClientTrip>> GetClientTrip(int idClient, int idTrip)
        {
            var clientTrip = await _context.ClientTrips
                .Include(ct => ct.IdClientNavigation)
                .Include(ct => ct.IdTripNavigation)
                .FirstOrDefaultAsync(ct => ct.IdClient == idClient && ct.IdTrip == idTrip);

            if (clientTrip == null)
            {
                return NotFound();
            }

            return clientTrip;
        }
    }
}
