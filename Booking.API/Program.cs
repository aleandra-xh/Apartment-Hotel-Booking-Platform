using Booking.Api;
using Booking.Api.Exceptions;
using Booking.API.Endpoints;
using Booking.Application.DependencyInjection;
using Booking.Infrastructure;
using Booking.Infrastructure.DependencyInjection;
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
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

var app = builder.Build();

app.UseExceptionHandler();        
app.UseHttpsRedirection();       
app.UseAuthentication();          
app.UseAuthorization();

app.MapOpenApi();
app.MapUserEndpoints();
app.MapPropertyEndpoints();
app.MapReservationEndpoints();
app.MapReviewEndpoints();
app.MapPropertyImageEndpoints();
app.MapNotificationEndpoints();


app.Run();