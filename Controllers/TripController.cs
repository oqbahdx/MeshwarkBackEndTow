using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Meshwark.Data;
using Meshwark.DTOs;
using Meshwark.Models;

namespace Meshwark.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TripController : ControllerBase
    {
        private readonly ApiContext _context;

        public TripController(ApiContext context)
        {
            _context = context;
        }

        // GET: api/Trip/userId
        [HttpGet("{userId}")]
        public async Task<ActionResult<IEnumerable<TripDto>>> GetTripsByUserId(Guid userId)
        {
            var user = await _context.Users
                                     .Include(u => u.Trips)
                                     .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            var trips = user.Trips?.Select(trip => new TripDto
            {
                Id = trip.Id,
                Date = trip.Date,
                Time = trip.Time,
                StartPoint = trip.StartPoint,
                EndPoint = trip.EndPoint,
                Price = trip.Price,
                RiderIds = trip.RiderIds
            }).ToList();

            return Ok(trips ?? new List<TripDto>());
        }

        // GET: api/Trip/all
        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<TripDto>>> GetAllTrips()
        {
            var trips = await _context.Trips.Include(t => t.User)
                                            .ToListAsync();

            var tripDtos = trips.Select(trip => new TripDto
            {
                Id = trip.Id,
                Date = trip.Date,
                Time = trip.Time,
                StartPoint = trip.StartPoint,
                EndPoint = trip.EndPoint,
                Price = trip.Price,
                UserId = trip.UserId,
                RiderIds = trip.RiderIds
            }).ToList();

            return Ok(tripDtos);
        }

        // POST: api/Trip
        [HttpPost]
        public async Task<ActionResult<Trip>> AddTrip([FromBody] TripDto tripDto)
        {
            var user = await _context.Users.FindAsync(tripDto.UserId);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Mark user (driver) as having an active trip
            user.IsTripActive = true;

            var trip = new Trip
            {
                Id = Guid.NewGuid(),
                Date = tripDto.Date,
                Time = tripDto.Time,
                StartPoint = tripDto.StartPoint,
                EndPoint = tripDto.EndPoint,
                Price = tripDto.Price,
                UserId = tripDto.UserId,
                RiderIds = tripDto.RiderIds
            };

            // Mark all riders as having an active trip
            if (trip.RiderIds != null && trip.RiderIds.Any())
            {
                var riders = await _context.Users
                                           .Where(u => trip.RiderIds.Contains(u.Id))
                                           .ToListAsync();

                foreach (var rider in riders)
                {
                    rider.IsTripActive = true;
                }
            }

            _context.Trips.Add(trip);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTripsByUserId), new { userId = tripDto.UserId }, trip);
        }

        // PUT: api/Trip/endTrip/tripId
        [HttpPut("endTrip/{tripId}")]
        public async Task<IActionResult> EndTrip(Guid tripId)
        {
            var trip = await _context.Trips.FindAsync(tripId);

            if (trip == null)
            {
                return NotFound("Trip not found.");
            }

            // Set the driver as not having an active trip
            var driver = await _context.Users.FindAsync(trip.UserId);
            if (driver != null)
            {
                driver.IsTripActive = false;
            }

            // Set the riders as not having an active trip
            if (trip.RiderIds != null && trip.RiderIds.Any())
            {
                var riders = await _context.Users
                                           .Where(u => trip.RiderIds.Contains(u.Id))
                                           .ToListAsync();

                foreach (var rider in riders)
                {
                    rider.IsTripActive = false;
                }
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Trip/deleteTrip/tripId
        [HttpDelete("deleteTrip/{tripId}")]
        public async Task<IActionResult> DeleteTrip(Guid tripId)
        {
            var trip = await _context.Trips.FindAsync(tripId);
            if (trip == null)
            {
                return NotFound("Trip not found.");
            }

            _context.Trips.Remove(trip);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
