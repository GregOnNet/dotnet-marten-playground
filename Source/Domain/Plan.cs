using System.Collections.Immutable;
using System.Text.Json.Serialization;
using CSharpFunctionalExtensions;

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

    public Result<Plan> Disponiere(Disposition disposition)
    {
        var istMitarbeiterAbwesend =
            AbwesendeMitarbeiterIds.Any(mitarbeiterId => mitarbeiterId == disposition.MitarbeiterId);

        if (istMitarbeiterAbwesend)
            return
                Result.Failure<Plan>($"Der Mitarbeiter mit der Id {disposition.MitarbeiterId} kann, wegen eingetragener Abwesenheit, nicht disponiert werden.");

        if (Dispositionen.ContainsKey(disposition.MitarbeiterId))
        {
            if (disposition.TaetigkeitenIds.Count() > 1)
            {
                Dispositionen = Dispositionen.SetItem(disposition.MitarbeiterId, disposition);
            }
            else
            {
                var dispositionAktualisiert = Dispositionen[disposition.MitarbeiterId];

                dispositionAktualisiert.TaetigkeitenIds = dispositionAktualisiert.TaetigkeitenIds
                   .Select((id, index) => index == 0
                                              ? disposition.TaetigkeitenIds.First()
                                              : id)
                   .Distinct();

                Dispositionen = Dispositionen.SetItem(disposition.MitarbeiterId, dispositionAktualisiert);
            }
        }
        else
        {
            Dispositionen = Dispositionen.Add(disposition.MitarbeiterId, disposition);
        }


        return Result.Success(this);
    }

    public Result<Plan> BeruecksichtigeAbwesenheitVon(Mitarbeiter mitarbeiter)
    {
        var istAbwesenheitNochNichtFuerMitarbeiterHinterlegt = !AbwesendeMitarbeiterIds.Contains(mitarbeiter.Id);

        if (istAbwesenheitNochNichtFuerMitarbeiterHinterlegt)
            AbwesendeMitarbeiterIds = AbwesendeMitarbeiterIds.Add(mitarbeiter.Id);


        return Result.Success(this);
    }
}