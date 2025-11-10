using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Enable CORS for Shopify
builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowAll", policy =>
	{
		policy.AllowAnyOrigin()
			  .AllowAnyMethod()
			  .AllowAnyHeader();
	});
});

// Add controllers
builder.Services.AddControllers();

var app = builder.Build();

// Middleware
// Remove HTTPS redirection for local testing
// app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.MapControllers(); // Map API controllers

// Listen on dynamic port (5000 default for local testing)
var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
app.Run($"http://0.0.0.0:{port}");
