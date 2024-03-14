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
        AusgefalleneMitarbeiterIds = ImmutableHashSet<Guid>.Empty;
        AnwesendeMitarbeiterIds = ImmutableHashSet<Guid>.Empty;
    }

    public Guid Id { get; init; }

    public DateOnly Tag { get; }

    [JsonInclude] public ImmutableDictionary<Guid, Disposition> Dispositionen { get; private set; }

    [JsonInclude] public ImmutableHashSet<Guid> AusgefalleneMitarbeiterIds { get; private set; }

    [JsonInclude] public ImmutableHashSet<Guid> AnwesendeMitarbeiterIds { get; private set; }

    public static Result<Plan> Create(DateOnly tag)
    {
        var heute = DateOnly.FromDateTime(DateTime.Today);

        if (tag < heute) return Result.Failure<Plan>("Ein Plan kann nicht für einen vergangenen Tag angelegt werden.");

        return Result.Success(new Plan(tag));
    }

    public Result<Plan> Disponiere(Disposition disposition)
    {
        var mitarbeiterKannNichtDisponiertWerden =
            MitarbeiterIstNichtEingebucht(disposition) &&
            MitarbeiterIstAusgefallen(disposition);

        if (mitarbeiterKannNichtDisponiertWerden)
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

    public Result<Plan> VermerkeAusfallVonMitarbeiter(Mitarbeiter mitarbeiter)
    {
        var istAusfallNochNichtFuerMitarbeiterHinterlegt = !AusgefalleneMitarbeiterIds.Contains(mitarbeiter.Id);

        if (istAusfallNochNichtFuerMitarbeiterHinterlegt)
            AusgefalleneMitarbeiterIds = AusgefalleneMitarbeiterIds.Add(mitarbeiter.Id);


        return Result.Success(this);
    }

    public Result<Plan> BucheMitarbeiterEin(Mitarbeiter mitarbeiter)
    {
        AnwesendeMitarbeiterIds = AnwesendeMitarbeiterIds.Add(mitarbeiter.Id);

        return Result.Success(this);
    }

    private bool MitarbeiterIstAusgefallen(Disposition disposition)
    {
        return AusgefalleneMitarbeiterIds.Any(mitarbeiterId => mitarbeiterId == disposition.MitarbeiterId);
    }

    private bool MitarbeiterIstNichtEingebucht(Disposition disposition)
    {
        return AnwesendeMitarbeiterIds.All(mitarbeiterId => mitarbeiterId != disposition.MitarbeiterId);
    }
}