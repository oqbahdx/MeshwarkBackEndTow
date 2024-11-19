using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Meshwark.Data;
using Meshwark.DTOs;
using Meshwark.Models;
using Microsoft.AspNetCore.Authorization;
using Meshwark.Hubs;
using System.Text.Json;

namespace Meshwark.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ApiContext _context;
        private readonly JwtService _jwtService;
        private readonly IHubContext<DriverHub> _hubContext;
        private readonly IHubContext<MoveHub> _hubMoveContext;
        private readonly IHubContext<ChatHub> _hubChatContext;

        public UsersController(ApiContext context, JwtService jwtService, IHubContext<DriverHub> hubContext, IHubContext<MoveHub> hubMoveContext, IHubContext<ChatHub> hubChatContext)
        {
            _context = context;
            _jwtService = jwtService;
            _hubContext = hubContext;
            _hubMoveContext = hubMoveContext;
            _hubChatContext = hubChatContext;
        }

        // POST: api/Users/register
        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(UserRegistrationDto userDto)
        {
            if (await _context.Users.AnyAsync(u => u.PhoneNumber == userDto.PhoneNumber))
            {
                return BadRequest("Phone number is already in use.");
            }

            if (await _context.Users.AnyAsync(u => u.Email == userDto.Email))
            {
                return BadRequest("Email is already in use.");
            }

            var user = new User
            {
                Id = Guid.NewGuid(),
                PhoneNumber = userDto.PhoneNumber,
                Email = userDto.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(userDto.Password),
                Role = userDto.Role,
                HasProfile = userDto.Role == "Admin" ? true : false, // Admin is active by default
                IsApproved = userDto.IsApproved,
                FcmToken = userDto.FcmToken,
            };

           await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "User has been registered successfully" });
        }

        // POST: api/Users/login
        [HttpPost("login")]
        public async Task<ActionResult<object>> Login(UserLoginDto loginDto)
        {
            var user = await _context.Users
                .SingleOrDefaultAsync(u => u.PhoneNumber == loginDto.PhoneNumber);

            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.Password))
            {
                return Unauthorized("Invalid phone number or password.");
            }

            var token = _jwtService.GenerateToken(user);

            var response = new
            {
                Token = token,
                Message = $"{user.Role} login successful",
                Role = user.Role,
                Id = user.Id,
                HasProfile = user.HasProfile
            };

            return Ok(response);
        }

        // GET: api/Users/{id}
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<UserDto>> GetUserById(Guid id)
        {
            var user = await _context.Users
                                     .Include(u => u.Trips) // Include trips to count them
                                     .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            var userDto = new UserDto
            {
                Id = user.Id,
                Role = user.Role,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                Email = user.Email,
                Gender = user.Gender,
                HasProfile = user.HasProfile,
                IsOnline = user.IsOnline,
                IsApproved = user.IsApproved,
                AvailableSeats = user.AvailableSeats,
                ReservedSeats = user.ReservedSeats,
                NextDestination = user.NextDestination,
                Longitude = user.Longitude,
                Latitude = user.Latitude,
                CarColor = user.CarColor,
                CarYear = user.CarYear,
                CarModel = user.CarModel,
                TypeOfTrip = user.TypeOfTrip,
                PlateImage = user.PlateImage,
                InsuranceImage = user.InsuranceImage,
                NumberPlate = user.NumberPlate,
                LicenseImage = user.LicenseImage,
                Rating = user.Rating,
                
                CompletedTrips = user.CompletedTrips,
                CanceledTrips = user.CanceledTrips,
                FcmToken = user.FcmToken,
                PersonalImagePath = user.PersonalImagePath != null
    ? $"{Request.Scheme}://{Request.Host}{user.PersonalImagePath}"
    : null,
                TotalTrips = user.Trips.Count // Count the number of trips
            };

            return Ok(userDto);
        }

        // GET: api/Users
        [HttpGet]
        
        public async Task<ActionResult<IEnumerable<User>>> GetAllUsers()
        {
            return await _context.Users.ToListAsync();
        }

        // PUT: api/Users/update/{id}
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateUser(Guid id, [FromForm] UserUpdateDto userDto)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound($"User with ID {id} not found.");
            }

            // Update fields if provided in the DTO
            user.Role = userDto.Role ?? user.Role;
            user.FirstName = userDto.FirstName ?? user.FirstName;
            user.LastName = userDto.LastName ?? user.LastName;
            user.PhoneNumber = userDto.PhoneNumber ?? user.PhoneNumber;
            user.Email = userDto.Email ?? user.Email;

            if (!string.IsNullOrEmpty(userDto.Password))
            {
                user.Password = BCrypt.Net.BCrypt.HashPassword(userDto.Password); // Password hashing
            }

            user.Gender = userDto.Gender ?? user.Gender;
            user.HasProfile = userDto.IsActive ?? user.HasProfile;
            user.IsOnline = userDto.IsOnline ?? user.IsOnline;
            user.IsApproved = userDto.IsApproved ?? user.IsApproved;
            user.AvailableSeats = userDto.AvailableSeats ?? user.AvailableSeats;
            user.ReservedSeats = userDto.ReservedSeats ?? user.ReservedSeats;
            user.NextDestination = userDto.NextDestination ?? user.NextDestination;
            user.Longitude = userDto.Longitude ?? user.Longitude;
            user.Latitude = userDto.Latitude ?? user.Latitude;
            user.CarColor = userDto.CarColor ?? user.CarColor;
            user.CarYear = userDto.CarYear ?? user.CarYear;
            user.CarModel = userDto.CarModel ?? user.CarModel;
            user.TypeOfTrip = userDto.TypeOfTrip ?? user.TypeOfTrip;
            user.NumberPlate = userDto.NumberPlate ?? user.NumberPlate;
            user.Rating = userDto.Rating ?? user.Rating;
            user.CompletedTrips = userDto.CompletedTrips ?? user.CompletedTrips;
            user.CanceledTrips = userDto.CanceledTrips ?? user.CanceledTrips;
            user.IsTripActive = userDto.IsTripActive ?? user.IsTripActive;
            user.FcmToken = userDto.FcmToken ?? user.FcmToken;
            
            // Handle file uploads
            await SaveFile(userDto.PersonalImage, id, "PersonalImagePath");
            await SaveFile(userDto.LicenseImage, id, "LicenseImage");
            await SaveFile(userDto.InsuranceImage, id, "InsuranceImage");
            await SaveFile(userDto.PlateImage, id, "PlateImage");
            await SaveFile(userDto.IdImage, id, "IdImage");

            await _context.SaveChangesAsync();
            await _hubContext.Clients.All.SendAsync("UserUpdated", user);

            return Ok(user);
        }

        private async Task SaveFile(IFormFile file, Guid id, string propertyName)
        {
            if (file != null)
            {
                try
                {
                    // Ensure the directory exists
                    var uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                    if (!Directory.Exists(uploadsDir))
                    {
                        Directory.CreateDirectory(uploadsDir);
                    }
                    var cleanFileName = file.FileName.Replace(" ", "_");
                    var fileName = $"{id}_{cleanFileName}";
                  
                    var filePath = Path.Combine(uploadsDir, fileName);

                    // Save the file
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }

                    // Store relative path (for URLs)
                    var relativePath = $"/images/{fileName}";

                    var user = await _context.Users.FindAsync(id);
                    if (user != null)
                    {
                        var prop = user.GetType().GetProperty(propertyName);
                        if (prop != null)
                        {
                            prop.SetValue(user, relativePath); // Store the relative path
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Log the exception
                //    _logger.LogError(ex, "Error saving file for user ID {id}", id);
                    throw;
                }
            }
        }



        // DELETE: api/Users/{id}
        [HttpDelete("{id}")]
        
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            await _hubContext.Clients.All.SendAsync("UserDeleted", user);

            return NoContent();
        }

        // GET: api/Users/drivers
        [HttpGet("drivers")]
        public async Task<IActionResult> GetAllDrivers()
        {
            var drivers = await _context.Users
                .Where(u => u.Role == "Driver")
                .ToListAsync();

            return Ok(drivers);
        }

        // GET: api/Users/riders
        [HttpGet("riders")]
        
        public async Task<IActionResult> GetAllRiders()
        {
            var riders = await _context.Users
                .Where(u => u.Role == "Rider")
                .ToListAsync();

            return Ok(riders);
        }

        // GET: api/Users/approved
        [HttpGet("approved")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetApprovedUsers()
        {
            var approvedUsers = await _context.Users
                .Where(u => u.IsApproved)
                .ToListAsync();

            return Ok(approvedUsers);
        }
        [HttpGet("count")]
        public async Task<ActionResult> GetCounts()
        {
            var totalUsers = await _context.Users.CountAsync();
            var totalAdmins = await _context.Users.CountAsync(u => u.Role == "Admin");
            var totalDrivers = await _context.Users.CountAsync(u => u.Role == "Driver");
            var totalTrips = await _context.Trips.CountAsync();

            return Ok(new
            {
                TotalUsers = totalUsers,
                TotalAdmins = totalAdmins,
                TotalDrivers = totalDrivers,
                TotalTrips = totalTrips
            });
        }
        [HttpGet("admins")]
        public async Task<ActionResult<IEnumerable<User>>> GetAllAdmins()
        {
            var admins = await _context.Users
                .Where(u => u.Role == "Admin")
                .ToListAsync();

            return Ok(admins);
        }
        [HttpGet("users")]
        public async Task<ActionResult<IEnumerable<User>>> GetUsersByDate(
    int? day = null,
    int? month = null,
    int? year = null)
        {
            IQueryable<User> query = _context.Users;

            if (year.HasValue)
            {
                query = query.Where(u => u.CreatedAt.Year == year.Value);
            }
            if (month.HasValue)
            {
                query = query.Where(u => u.CreatedAt.Month == month.Value);
            }
            if (day.HasValue)
            {
                query = query.Where(u => u.CreatedAt.Day == day.Value);
            }

            var users = await query.ToListAsync();

            if (users == null || users.Count == 0)
            {
                return NotFound("No users found for the specified date.");
            }

            return Ok(users);
        }
        [HttpPost("updateDriverTrip")]
        public async Task<IActionResult> UpdateDriver([FromBody] DriverUpdateTrip update)
        {
            await _hubContext.Clients.All.SendAsync("BroadcastDriverUpdate", update);
            return Ok();
        }
        [HttpPut("updateDriverTrip/{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateUserRealTime(Guid id, DriverUpdateTrip updateDto)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // Update only provided fields
            if (updateDto.IsOnline.HasValue)
            {
                user.IsOnline = updateDto.IsOnline.Value;
            }
            if (updateDto.Latitude.HasValue)
            {
                user.Latitude = updateDto.Latitude.Value;
            }
            if (updateDto.Longitude.HasValue)
            {
                user.Longitude = updateDto.Longitude.Value;
            }
            if (updateDto.AvailableSeats.HasValue)
            {
                user.AvailableSeats = updateDto.AvailableSeats.Value;
            }
            if (updateDto.ReservedSeats.HasValue)
            {
                user.ReservedSeats = updateDto.ReservedSeats.Value;
            }
            if (!string.IsNullOrEmpty(updateDto.NextDestination))
            {
                user.NextDestination = updateDto.NextDestination;
            }
            if (!string.IsNullOrEmpty(updateDto.Gender))
            {
                user.Gender = updateDto.Gender;
            }
            if (!string.IsNullOrEmpty(updateDto.NumberPlate))
            {
                user.NumberPlate = updateDto.NumberPlate;
            }


            await _context.SaveChangesAsync();

            // Serialize the user object to JSON
            var userJson = JsonSerializer.Serialize(user);

            // Use the DriverHub to broadcast the update
            await _hubContext.Clients.All.SendAsync("BroadcastDriverUpdate", userJson);
            

            // Broadcast to all connected drivers
            await _hubContext.Clients.Group("Drivers").SendAsync("ReceiveDriverInformationUpdate", userJson);


            return Ok(user);
        }
        [HttpPost("send")]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageDto dto)
        {
            await _hubContext.Clients.User(dto.ReceiverUserId).SendAsync("ReceiveMessage", dto.Message);
            return Ok();
        }
    }


    }
