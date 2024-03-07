using CSharpFunctionalExtensions;
using Marten;
using Microsoft.AspNetCore.Mvc;
using Perosnaldisposition;
using Weasel.Core;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMarten(options =>
                           {
                               // Establish the connection string to your Marten database
                               options.Connection(builder.Configuration.GetConnectionString("Personaldisposition")!);

                               // If we're running in development mode, let Marten just take care
                               // of all necessary schema building and patching behind the scenes
                               if (builder.Environment.IsDevelopment())
                                   options.AutoCreateSchemaObjects = AutoCreate.All;
                           }).UseLightweightSessions();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/gruppe", async (CreateGruppeRequest create, [FromServices] IDocumentSession session) =>
                       {
                           return Gruppe.Create(create.Name)
                                        .Tap(async gruppe =>
                                             {
                                                 session.Store(gruppe);
                                                 await session.SaveChangesAsync();
                                             })
                                        .Finally(result => result.IsSuccess ? Results.Created() : Results.BadRequest(result.Error));
                       })
   .WithOpenApi();

app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

public record struct CreateGruppeRequest(string Name);