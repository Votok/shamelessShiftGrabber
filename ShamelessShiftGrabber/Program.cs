using Microsoft.AspNetCore.Mvc;
using ShamelessShiftGrabber;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient("macrodroid", c =>
{
    c.BaseAddress = new Uri("https://trigger.macrodroid.com/");
    c.DefaultRequestHeaders.Add("Accept", "application/json");
});

builder.Services.AddLogging();

builder.Services.AddTransient<Macrodroid>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();



app.MapPost("/shifts", async (
    [FromBody] ShiftRequest shiftRequest,
    Macrodroid macrodroid,
    ILogger<Program> logger
) =>
{
    if (shiftRequest == null || shiftRequest.Shifts == null)
    {
        return Results.BadRequest("Shifts are required");
    }

    logger.LogInformation($"Received shifts: {shiftRequest.Shifts.Length}");

    if (shiftRequest.Shifts.Length > 0)
    {
       return await macrodroid.Send(shiftRequest.Shifts);
    }

    return Results.Ok("No shifts were received or processed.");
});

app.Run();