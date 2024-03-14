using API;
using API.UseCases.Gruppen.Auflisten;
using API.UseCases.Gruppen.Entfernen;
using API.UseCases.Gruppen.Erfassen;
using API.UseCases.Gruppen.Korrekturen;
using API.UseCases.Mitarbeiter.Arbeitszeiten;
using API.UseCases.Mitarbeiter.Auflisten;
using API.UseCases.Mitarbeiter.Details;
using API.UseCases.Mitarbeiter.Erfassen;
using API.UseCases.Mitarbeiter.Gruppe;
using API.UseCases.Mitarbeiter.Qualifizieren;
using API.UseCases.Plaene.Details;
using API.UseCases.Plaene.Dispositionieren;
using API.UseCases.Plaene.Erfassen;
using API.UseCases.Taetigkeiten.Auflisten;
using API.UseCases.Taetigkeiten.Details;
using API.UseCases.Taetigkeiten.Erfassen;
using API.UseCases.Taetigkeiten.Korrektur;

var builder = WebApplication.CreateBuilder(args);

Setup.Marten(builder.Environment, builder.Services, builder.Configuration.GetConnectionString("Personaldisposition")!);

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

app.MapPost("/gruppen/{id:guid}/namens-korrektur", GruppennamenKorrigierenEndpoint.Handle)
   .WithOpenApi();

/* /mitarbeiter */
var mitarbeiter = app.MapGroup("/mitarbeiter").WithOpenApi();

mitarbeiter.MapGet("", AlleMitarbeiterAuflistenEndpoint.Handle);

app.MapGet("/mitarbeiter/{id:guid}", MitarbeiterDetailsEndpoint.Handle)
   .WithName(nameof(MitarbeiterDetailsEndpoint))
   .WithOpenApi();

app.MapPost("/mitarbeiter", NeuenMitarbeiterErfassenEndpoint.Handle)
   .WithOpenApi();

app.MapPost("/mitarbeiter/{id:guid}/qualifizierte-taetigkeiten/{taetigkeitId:guid}",
            MitarbeiterFuerTaetigkeitQualifizierenEndpoint.Handle)
   .WithName(nameof(MitarbeiterFuerTaetigkeitQualifizierenEndpoint))
   .WithOpenApi();

app.MapDelete("/mitarbeiter/{id:guid}/qualifizierte-taetigkeiten/{taetigkeitId:guid}",
              MitarbeiterQualifikationEntziehenEndpoint.Handle)
   .WithOpenApi();

app.MapPost("/mitarbeiter/{id:guid}/abweichende-arbeitszeiten/{tag}", AbweichendeArbeitszeitErfassenEndpoint.Handle)
   .WithName(nameof(AbweichendeArbeitszeitErfassenEndpoint))
   .WithOpenApi();

app.MapDelete("/mitarbeiter/{id:guid}/abweichende-arbeitszeiten/{tag}", AbweichendeArbeitszeitRevidierenEndpoint.Handle)
   .WithOpenApi();

app.MapPatch("/mitarbeiter/{id:guid}/gruppe/{gruppeId:guid}", MitarbeiterWechseltInAndereGruppeEndpoint.Handle)
   .WithOpenApi();

/* /taetigkeiten */

var taetigkeiten = app.MapGroup("/taetigkeiten").WithOpenApi();

taetigkeiten.MapGet("", AlleTaetigkeitenAuflistenEndpoint.Handle);
taetigkeiten.MapGet("{id}", TaetigkeitDetailsEndpoint.Handle);

taetigkeiten.MapPost("", NeueTaetigkeitErfassenEndpoint.Handle)
            .WithName(nameof(NeueTaetigkeitErfassenEndpoint));

taetigkeiten.MapPost("{id:guid}/bezeichnung-korrektur", TaetigkeitBezeichnungKorrigierenEndpoint.Handle);

/* /plaene */
app.MapGet("/plaene/{tag}", PlanDetailsEndpoint.Handle)
   .WithName(nameof(PlanDetailsEndpoint))
   .WithOpenApi();

app.MapPost("/plaene", NeuenPlanErfassenEndpoint.Handle)
   .WithName(nameof(NeuenPlanErfassenEndpoint))
   .WithOpenApi();

app.MapPost("/plaene/{tag}/abwesenheiten", AbwesenheitFuerMitarbeiterErfassenEndpoint.Handle)
   .WithName(nameof(AbwesenheitFuerMitarbeiterErfassenEndpoint))
   .WithOpenApi();

app.MapPut("/plaene/{tag}/dispositionen", MitarbeiterDisponierenEndpoint.Handle)
   .WithOpenApi();

app.Run();