using Meshwark.Data;
using Meshwark.DTOs;
using Meshwark.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly ApiContext _context;
    private readonly IEmailSender _emailSender;

    public AccountController(ApiContext context, IEmailSender emailSender)
    {
        _context = context;
        _emailSender = emailSender;
    }

    [HttpPost("forgotPassword")]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordDto dto)
    {
        var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == dto.Email);
        if (user == null)
        {
            return NotFound("Email not found.");
        }

        var otp = GenerateOTP();
        var expirationTime = DateTime.UtcNow.AddMinutes(15); // OTP valid for 15 minutes

        user.Otp = otp;
        user.OtpExpirationTime = expirationTime;
        await _context.SaveChangesAsync();

        var emailSent = await _emailSender.SendEmailAsync(dto.Email, "Your OTP", $"Your OTP is {otp}");
        if (!emailSent)
        {
            return StatusCode(500, "Failed to send email.");
        }

        return Ok("OTP sent to your email.");
    }

    [HttpPost("resetPassword")]
    public async Task<IActionResult> ResetPassword(ResetPasswordDto dto)
    {
        var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == dto.Email && u.Otp == dto.Otp);
        if (user == null || user.OtpExpirationTime < DateTime.UtcNow)
        {
            return BadRequest("Invalid or expired OTP.");
        }

        user.Password = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
        user.Otp = null;
        user.OtpExpirationTime = null;
        await _context.SaveChangesAsync();

        return Ok("Password reset successful.");
    }

    private string GenerateOTP()
    {
        var random = new Random();
        return random.Next(100000, 999999).ToString(); // Generate a 6-digit OTP
    }
}
