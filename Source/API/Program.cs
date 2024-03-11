using API.UseCases.Gruppe.Auflisten;
using API.UseCases.Gruppe.Entfernen;
using API.UseCases.Gruppe.Erfassen;
using API.UseCases.Gruppe.NamenKorrigieren;
using API.UseCases.Mitarbeiter.Auflisten;
using API.UseCases.Mitarbeiter.Erfassen;
using API.UseCases.Mitarbeiter.Qualifizieren;
using API.UseCases.Taetigkeiten.Auflisten;
using API.UseCases.Taetigkeiten.Erfassen;
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

/* /gruppen */
app.MapGet("/gruppen", AlleGruppenAuflistenEndpoint.Handle)
   .WithOpenApi();

app.MapPost("/gruppen", NeueGruppeErfassenEndpoint.Handle)
   .WithName(nameof(NeueGruppeErfassenEndpoint))
   .WithOpenApi();

app.MapDelete("/gruppen/{id:guid}", GruppeEntfernenEndpoint.Handle)
   .WithOpenApi();

app.MapPost("/gruppen/{id:guid}/namens-korrektur", GruppenNamenKorrigierenEndpoint.Handle)
   .WithOpenApi();

/* /mitarbeiter */

app.MapGet("/mitarbeiter", AlleMitarbeiterAuflistenEndpoint.Handle)
   .WithOpenApi();

app.MapPost("/mitarbeiter", NeuenMitarbeiterErfassenEndpoint.Handle)
   .WithName(nameof(NeuenMitarbeiterErfassenEndpoint))
   .WithOpenApi();

app.MapPost("/mitarbeiter/{id:guid}/qualifizierte-taetigkeiten/{taetigkeitId:guid}", MitarbeiterFuerTaetigkeitQualifizierenEndpoint.Handle)
   .WithName(nameof(MitarbeiterFuerTaetigkeitQualifizierenEndpoint))
   .WithOpenApi();

app.MapDelete("/mitarbeiter/{id:guid}/qualifizierte-taetigkeiten/{taetigkeitId:guid}",
              MitarbeiterQualifikationEntziehenEndpoint.Handle)
   .WithOpenApi();


/* /taetigkeiten */

app.MapGet("/taetigkeiten", AlleTaetigkeitenAuflistenEndpoint.Handle)
   .WithOpenApi();

app.MapPost("/taetigkeiten", NeueTaetigkeitErfassenEndpoint.Handle)
   .WithName(nameof(NeueTaetigkeitErfassenEndpoint))
   .WithOpenApi();

app.Run();

