using System.Text;
using Meshwark.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Meshwark.Helper;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Builder;
using Meshwark.Hubs;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Meshwark.Service;
using Meshwark.Controllers;
using Meshwark.Models;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});
builder.Services.AddDbContext<ApiContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddSignalR();

// Ensure FirebaseApp is initialized only once
FirebaseApp firebaseApp;
try
{
    firebaseApp = FirebaseApp.DefaultInstance ?? FirebaseApp.Create(new AppOptions
    {
        Credential = GoogleCredential.FromFile(Path.Combine(Directory.GetCurrentDirectory(), "firebase-service-account.json"))
    });
}
catch (Exception ex)
{
    Console.WriteLine($"Error initializing FirebaseApp: {ex.Message}");
    throw;
}

builder.Services.AddSingleton(firebaseApp);
builder.Services.AddSingleton<FirebaseService>();
// Register the IWalletService as a Singleton
//builder.Services.AddSingleton<IWalletService, WalletService>();
builder.Services.AddScoped<IWalletService, WalletService>();
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddTransient<IEmailService, DriverEmailService>();

// Configure Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Your API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\""
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

    c.OperationFilter<FileUploadOperation>();
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        policyBuilder =>
        {
            policyBuilder.AllowAnyOrigin()
                         .AllowAnyMethod()
                         .AllowAnyHeader();
        });
});

var key = Encoding.ASCII.GetBytes(builder.Configuration["JWT:SecretKey"]);
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JWT:Issuer"],
        ValidAudience = builder.Configuration["JWT:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ClockSkew = TimeSpan.Zero
    };
});
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
});

builder.Services.AddSingleton<JwtService>();
builder.Services.AddSingleton<IEmailSender, EmailSender>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        policyBuilder =>
        {
            policyBuilder.WithOrigins("http://meshwark.com")
                         .AllowAnyMethod()
                         .AllowAnyHeader()
                         .AllowCredentials(); 
        });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost4200",
        policyBuilder =>
        {
            policyBuilder.WithOrigins("http://localhost:4200") 
                         .AllowAnyMethod()
                         .AllowAnyHeader()
                         .AllowCredentials(); 
        });
});


var app = builder.Build();
//app.UseEndpoints(endpoints =>
//{
//    endpoints.MapControllerRoute(
//        name: "default",
//        pattern: "{controller=Home}/{action=Index}/{id?}");
//});

//// Catch-all for React routes
//app.UseEndpoints(endpoints =>
//{
//    endpoints.MapFallbackToFile("index.html");
//});
//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
    app.UseSwaggerUI();
//}

app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseRouting();
app.UseCors("AllowAllOrigins");
app.UseCors("AllowLocalhost4200");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<DriverHub>("/driverHub");
app.MapHub<ChatHub>("/chatHub");

app.Run();