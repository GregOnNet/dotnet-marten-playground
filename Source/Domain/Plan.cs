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
        Dispositionen = ImmutableDictionary<Guid, Disposition>.Empty;
        AbwesendeMitarbeiterIds = ImmutableHashSet<Guid>.Empty;
    }

    public Guid Id { get; set; }

    public DateOnly Tag { get; }

    public ImmutableDictionary<Guid, Disposition> Dispositionen { get; set; }

    public ImmutableHashSet<Guid> AbwesendeMitarbeiterIds { get; set; }

    public static Result<Plan> Create(DateOnly tag)
    {
        var heute = DateOnly.FromDateTime(DateTime.Today);

        if (tag < heute) return Result.Failure<Plan>("Ein Plan kann nicht für einen vergangenen Tag angelegt werden.");

        return Result.Success(new Plan(tag));
    }

    public Result Disponiere(Disposition neueDisposition)
    {
        var istMitarbeiterAbwesend =
            AbwesendeMitarbeiterIds.Any(mitarbeiterId => mitarbeiterId == neueDisposition.Mitarbeiter.Id);

        if (istMitarbeiterAbwesend)
            return
                Result.Failure<Plan>($"{neueDisposition.Mitarbeiter.Vorname} {neueDisposition.Mitarbeiter.Nachname} kann wegen eingetragener Abwesenheit nicht disponiert werden.");

        if (Dispositionen.ContainsKey(neueDisposition.Mitarbeiter.Id))
        {
            if (neueDisposition.TaetigkeitenIds.Count() > 1)
            {
                Dispositionen = Dispositionen.SetItem(neueDisposition.Mitarbeiter.Id, neueDisposition);
            }
            else
            {
                var dispositionAktualisiert = Dispositionen[neueDisposition.Mitarbeiter.Id];

                dispositionAktualisiert.TaetigkeitenIds = dispositionAktualisiert.TaetigkeitenIds
                   .Select((id, index) =>
                               index == 0
                                   ? neueDisposition
                                    .TaetigkeitenIds
                                    .First()
                                   : id)
                   .Distinct();

                Dispositionen = Dispositionen.SetItem(neueDisposition.Mitarbeiter.Id, dispositionAktualisiert);
            }
        }
        else
        {
            Dispositionen = Dispositionen.Add(neueDisposition.Mitarbeiter.Id, neueDisposition);
        }


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