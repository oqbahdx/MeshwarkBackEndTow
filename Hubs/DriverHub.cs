using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Text.Json;
using System.Threading.Tasks;

public class DriverHub : Hub
{
    private readonly ILogger<DriverHub> _logger;

    public DriverHub(ILogger<DriverHub> logger)
    {
        _logger = logger;
    }

    public override Task OnConnectedAsync()
    {
        _logger.LogInformation($"Client connected: {Context.ConnectionId}");
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        _logger.LogInformation($"Client disconnected: {Context.ConnectionId}. Exception: {exception?.Message}");
        return base.OnDisconnectedAsync(exception);
    }

    public async Task<string> IdentifyDriver(string driverId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, "Drivers");
        await Groups.AddToGroupAsync(Context.ConnectionId, driverId);
        _logger.LogInformation($"Driver {driverId} identified and added to group.");

        // Return a success message instead of null
        return $"Driver {driverId} successfully identified.";
    }


    public async Task IdentifyRider(string riderId)
    {
        _logger.LogInformation($"IdentifyRider called with riderId: {riderId}");
        try
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, riderId);
            _logger.LogInformation($"Rider {riderId} added to group successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error in IdentifyRider: {ex.Message}");
            throw;
        }
    }

    public async Task SendRideRequest(string riderId, string driverId, RideRequest request)
    {
        _logger.LogInformation($"SendRideRequest called - RiderId: {riderId}, DriverId: {driverId}, Request: {JsonSerializer.Serialize(request)}");
        try
        {
            await Clients.Group(driverId).SendAsync("ReceiveRideRequest", new { RiderId = riderId, Request = request });
            _logger.LogInformation($"Ride request sent successfully to driver {driverId}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error in SendRideRequest: {ex.Message}");
            throw;
        }
    }



    // 5. Ensure the SendDriverResponse method is using the correct client method
    public async Task SendDriverResponse(string riderId, bool accepted)
    {
        _logger.LogInformation($"SendDriverResponse called - RiderId: {riderId}, Accepted: {accepted}");

        try
        {
            await Clients.Group(riderId).SendAsync("ReceiveDriverResponse", accepted);
            _logger.LogInformation($"Driver response sent successfully to rider {riderId}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error in SendDriverResponse: {ex.Message}");
            throw;
        }
    }
  


    public async Task BroadcastDriverUpdate(string updatedUserJson)
    {
        _logger.LogInformation($"BroadcastDriverUpdate called - UpdatedUser: {updatedUserJson}");
        try
        {
            if (string.IsNullOrEmpty(updatedUserJson))
            {
                _logger.LogWarning("Attempted to broadcast null or empty updatedUserJson");
                return;
            }

            await Clients.All.SendAsync("ReceiveDriverInformationUpdate", updatedUserJson);
            _logger.LogInformation("Driver update broadcasted successfully to all clients");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error in BroadcastDriverUpdate: {ex.Message}");
            throw;
        }
    }
    public async Task TestSignalRConnection()
    {
        await Clients.All.SendAsync("TestMessage", "Hello from the server");
    }
    public async Task NotifyCancellation(string userId, string reason, bool isDriver)
    {
        _logger.LogInformation($"NotifyCancellation called - UserId: {userId}, Reason: {reason}, IsDriver: {isDriver}");
        try
        {
            // Send notification to the other party (Driver/Rider)
            if (isDriver)
            {
                await Clients.Group(userId).SendAsync("ReceiveCancellationNotification", new { IsDriver = true, Reason = reason });
                _logger.LogInformation($"Cancellation notification sent to rider {userId} by driver.");
            }
            else
            {
                await Clients.Group(userId).SendAsync("ReceiveCancellationNotification", new { IsDriver = false, Reason = reason });
                _logger.LogInformation($"Cancellation notification sent to driver {userId} by rider.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error in NotifyCancellation: {ex.Message}");
            throw;
        }
    }

    public async Task SendCancellationNotification(bool isDriver, string reason)
    {
        var connectionId = Context.ConnectionId;
        var group = isDriver ? "Riders" : "Drivers"; // Adjust according to your group structure
        var notificationData = new { IsDriver = isDriver, Reason = reason };

        await Clients.Group(group).SendAsync("ReceiveCancellationNotification", notificationData);
        _logger.LogInformation($"Cancellation notification sent: IsDriver={isDriver}, Reason={reason}");
    }

    public async Task SendDriverArrivalNotification(string riderId, string driverId, string message)
    {
        _logger.LogInformation($"SendDriverArrivalNotification called - RiderId: {riderId}, DriverId: {driverId}, Message: {message}");

        try
        {
            // Send the message to the specific rider
            await Clients.Group(riderId).SendAsync("ReceiveDriverArrivalNotification", new
            {
                RiderId = riderId,
                DriverId = driverId,
                Message = message
            });

            _logger.LogInformation($"Arrival notification sent successfully to rider {riderId}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error in SendDriverArrivalNotification: {ex.Message}");
            throw;
        }
    }


}

public class RideRequest
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public double? Price { get; set; }
    public int? Passengers { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public string? PhoneNumber { get; set; }
}