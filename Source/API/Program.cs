using API.UseCases.Gruppe.Auflisten;
using API.UseCases.Gruppe.Entfernen;
using API.UseCases.Gruppe.Erfassen;
using API.UseCases.Gruppe.NamenKorrigieren;
using Marten;
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

app.MapGet("/gruppen", GruppenAuflistenEndpoint.Handle)
   .WithOpenApi();

app.MapPost("/gruppen", NeueGruppeErfassenEndpoint.Handle)
   .WithOpenApi();


app.MapDelete("/gruppen/{id}", GruppeEntfernenEndpoint.Handle)
   .WithOpenApi();

app.MapPost("/gruppen/{id}/namens-korrektur", GruppenNamenKorrigierenEndpoint.Handle)
   .WithOpenApi();

app.Run();

