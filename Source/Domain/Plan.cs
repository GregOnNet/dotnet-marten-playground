using System.Collections.Immutable;
using CSharpFunctionalExtensions;
using Newtonsoft.Json;

namespace Perosnaldisposition;

public class Plan
{
    [JsonConstructor]
    private Plan(DateOnly tag)
    {
        Id = Guid.NewGuid();

        Tag = tag;
        Dispositionen = Array.Empty<Disposition>();
        AbwesendeMitarbeiterIds = ImmutableHashSet<Guid>.Empty;
    }

    public Guid Id { get; set; }

    public DateOnly Tag { get; }

    public IEnumerable<Disposition> Dispositionen { get; set; }

    public ImmutableHashSet<Guid> AbwesendeMitarbeiterIds { get; set; }

    public static Result<Plan> Create(DateOnly tag)
    {
        var heute = DateOnly.FromDateTime(DateTime.Today);

        if (tag < heute) return Result.Failure<Plan>("Ein Plan kann nicht für einen vergangenen Tag angelegt werden.");

        return Result.Success(new Plan(tag));
    }

    public Result Disponiere(Disposition neueDisposition)
    {
        if (Dispositionen.Any(disposition => disposition.Mitarbeiter.Equals(neueDisposition.Mitarbeiter)))
            return
                Result.Failure($"Der Mitarbeiter {neueDisposition.Mitarbeiter.Vorname} {neueDisposition.Mitarbeiter.Nachname} kann nicht mehrfach disponiert werden.");

        Dispositionen = Dispositionen.Append(neueDisposition);

        return Result.Success();
    }

    public Result<Plan> BeruecksichtigeAbwesenheitVon(Mitarbeiter mitarbeiter)
    {
        var istAbwesenheitNochNichtFuerMitarbeiterHinterlegt = !AbwesendeMitarbeiterIds.Contains(mitarbeiter.Id);

        if (istAbwesenheitNochNichtFuerMitarbeiterHinterlegt)
            AbwesendeMitarbeiterIds = AbwesendeMitarbeiterIds.Add(mitarbeiter.Id);


        return Result.Success(this);
    }
}