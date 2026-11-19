using HjemIKroppenBooking.Api.Extensions;
using HjemIKroppenBooking.Application;
using HjemIKroppenBooking.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders().AddJsonConsole(options =>
{
    options.IncludeScopes = true;
    options.UseUtcTimestamp = true;
    options.TimestampFormat = "O";
});

builder.Services.AddApplication().AddInfrastructure(builder.Configuration).AddApi(builder.Configuration);

var app = builder.Build();
app.UseApi();
await app.RunAsync();
