using Booking.Api;
using Booking.Api.Exceptions;
using Booking.API.Endpoints;
using Booking.API.Hubs;
using Booking.API.Realtime;
using Booking.Application.Abstractions.Notifications;
using Booking.Application.DependencyInjection;
using Booking.Infrastructure;
using Booking.Infrastructure.DependencyInjection;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<BookingDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    ));

builder.Services.AddProblemDetails();
builder.Services.AddOpenApi();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddHttpContextAccessor();

builder.Services.AddSignalR();
builder.Services.AddScoped<INotificationRealtimeService, NotificationRealtimeService>();
builder.Services.AddSingleton<IUserIdProvider, SignalRUserIdProvider>();

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

var app = builder.Build();

app.UseExceptionHandler();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();

app.MapOpenApi();
app.MapUserEndpoints();
app.MapPropertyEndpoints();
app.MapReservationEndpoints();
app.MapReviewEndpoints();
app.MapPropertyImageEndpoints();
app.MapNotificationEndpoints();
app.MapAdminEndpoints();
app.MapHub<NotificationHub>("/hubs/notifications");

app.Run();