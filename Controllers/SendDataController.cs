using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Meshwark.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RideRequestController : ControllerBase
    {
        private readonly IHubContext<DriverHub> _hubContext;
        private readonly ILogger<RideRequestController> _logger;

        public RideRequestController(IHubContext<DriverHub> hubContext, ILogger<RideRequestController> logger)
        {
            _hubContext = hubContext;
            _logger = logger;
        }

        [HttpPost("sendTripRequest")]
        public async Task<IActionResult> SendRideRequest([FromBody] RideRequestDto dto)
        {
            try
            {
                var request = new RideRequest
                {
                    Price = dto.Price,
                    Passengers = dto.Passengers,
                    Latitude = dto.Latitude,
                    Longitude = dto.Longitude,
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    PhoneNumber = dto.PhoneNumber,
                };
                _logger.LogInformation($"Sending ride request via API - RiderId: {dto.RiderId}, DriverId: {dto.DriverId}, Request: {JsonSerializer.Serialize(request)}");
                await _hubContext.Clients.Group(dto.DriverId).SendAsync("ReceiveRideRequest", new { RiderId = dto.RiderId, Request = request });
                return Ok(new { message = "Ride request sent successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending ride request: {ex.Message}");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost("send-driver-response")]
        public async Task<IActionResult> SendDriverResponse([FromBody] DriverResponseRequest request)
        {
            if (string.IsNullOrEmpty(request.RiderId))
            {
                return BadRequest("RiderId is required.");
            }

            _logger.LogInformation($"Sending driver response for RiderId: {request.RiderId}, Accepted: {request.Accepted}");

            try
            {
                _logger.LogInformation($"Driver response: RiderId: {request.RiderId}, Accepted: {request.Accepted}");
                await _hubContext.Clients.Group(request.RiderId).SendAsync("ReceiveDriverResponse", request.Accepted);
                _logger.LogInformation($"Driver response sent to RiderId: {request.RiderId}");

                return Ok(new { message = "Driver response sent successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending driver response: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        // New endpoint for trip cancellation
        [HttpPost("cancelTrip")]
        public async Task<IActionResult> CancelTrip([FromBody] CancelTripRequest request)
        {
            if (string.IsNullOrEmpty(request.RiderId) || string.IsNullOrEmpty(request.DriverId))
            {
                return BadRequest("RiderId and DriverId are required.");
            }

            _logger.LogInformation($"Trip cancellation initiated by {request.CancellingParty} - RiderId: {request.RiderId}, DriverId: {request.DriverId}");

            try
            {
                var cancellationData = new
                {
                    RiderId = request.RiderId,
                    DriverId = request.DriverId,
                    Reason = request.Reason,
                    CancellingParty = request.CancellingParty
                };

                // Notify both rider and driver
                await _hubContext.Clients.Group(request.RiderId).SendAsync("ReceiveCancellationNotification", JsonSerializer.Serialize(cancellationData));
                await _hubContext.Clients.Group(request.DriverId).SendAsync("ReceiveCancellationNotification", JsonSerializer.Serialize(cancellationData));

                _logger.LogInformation($"Cancellation notification sent to RiderId: {request.RiderId} and DriverId: {request.DriverId}");

                return Ok(new { message = "Trip cancellation sent successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending trip cancellation: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }


        [HttpPost("sendDriverArrivalNotification")]
        public async Task<IActionResult> SendDriverArrivalNotification([FromBody] DriverArrivalNotificationDto dto)
        {
            if (string.IsNullOrEmpty(dto.RiderId) || string.IsNullOrEmpty(dto.DriverId) || string.IsNullOrEmpty(dto.Message))
            {
                return BadRequest("RiderId, DriverId, and Message are required.");
            }

            _logger.LogInformation($"Sending driver arrival notification - RiderId: {dto.RiderId}, DriverId: {dto.DriverId}, Message: {dto.Message}");

            try
            {
                await _hubContext.Clients.Group(dto.RiderId).SendAsync("ReceiveDriverArrivalNotification", new
                {
                    RiderId = dto.RiderId,
                    DriverId = dto.DriverId,
                    Message = dto.Message
                });

                return Ok(new { message = "Driver arrival notification sent successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending driver arrival notification: {ex.Message}");
                return StatusCode(500, new { error = ex.Message });
            }
        }


    }

    // DTO for ride request
    public class RideRequestDto
    {
        public string RiderId { get; set; }
        public string DriverId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public double? Price { get; set; }
        public int? Passengers { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string? PhoneNumber { get; set; }
    }

    // DTO for driver response
    public class DriverResponseRequest
    {
        public string RiderId { get; set; }
        public bool Accepted { get; set; }
    }

    // New DTO for trip cancellation
    public class CancelTripRequest
    {
        public string RiderId { get; set; }
        public string DriverId { get; set; }
        public string Reason { get; set; } // Optional field for reason
        public string CancellingParty { get; set; } // "Rider" or "Driver" to specify who is cancelling
    }


    public class DriverArrivalNotificationDto
    {
        public string RiderId { get; set; }
        public string DriverId { get; set; }
        public string Message { get; set; }
    }

}
