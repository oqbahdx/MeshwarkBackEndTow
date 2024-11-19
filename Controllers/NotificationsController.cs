using Meshwark.Data;
using Meshwark.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Meshwark.DTOs;

namespace Meshwark.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly ApiContext _context;

        public NotificationsController(ApiContext context)
        {
            _context = context;
        }

        // Get all notifications for a user
        [HttpGet("{userId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<NotificationDto>>> GetUserNotifications(Guid userId)
        {
            var notifications = await _context.Notifications
                                              .Where(n => n.UserId == userId)
                                              .Select(n => new NotificationDto
                                              {
                                                  Id = n.Id,
                                                  UserId = n.UserId,
                                                  Title = n.Title,
                                                  Body = n.Body,
                                                  Date = n.Date,
                                                  Time = n.Time
                                              })
                                              .ToListAsync();

            return Ok(notifications);  // Return the list of notifications (empty if none)
        }

        // Add a notification for a user
        [HttpPost("{userId}")]
        [Authorize]
        public async Task<ActionResult<NotificationDto>> AddNotification(Guid userId, NotificationDto notificationDto)
        {
            if (notificationDto == null)
            {
                return BadRequest("Invalid notification data.");
            }

            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Title = notificationDto.Title,
                Body = notificationDto.Body,
                Date = notificationDto.Date,
                Time = notificationDto.Time
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            notificationDto.Id = notification.Id;

            return CreatedAtAction(nameof(GetUserNotifications), new { userId = userId }, notificationDto);
        }

        // Delete a notification
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteNotification(Guid id)
        {
            var notification = await _context.Notifications.FindAsync(id);
            if (notification == null)
            {
                return NotFound("Notification not found.");
            }

            _context.Notifications.Remove(notification);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
