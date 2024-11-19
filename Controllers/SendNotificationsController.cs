using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class SendNotificationController : ControllerBase
{
    private readonly FirebaseService _firebaseService;

    public SendNotificationController(FirebaseService firebaseService)
    {
        _firebaseService = firebaseService;
    }

    [HttpPost("send")]
    public async Task<IActionResult> SendNotification([FromBody] NotificationRequest request)
    {
        await _firebaseService.SendNotification(request.FcmToken, request.Title, request.Body);
        return Ok(new { Message = "Notification sent successfully" });
    }
}

public class NotificationRequest
{
    public string FcmToken { get; set; }
    public string Title { get; set; }
    public string Body { get; set; }
}
