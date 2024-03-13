using System.Collections.Immutable;
using JasperFx.Core;
using Marten;
using Perosnaldisposition;

namespace API.UseCases.Mitarbeiter.Details;

public class MitarbeiterDetailsEndpoint
{
    public static MitarbeiterDetailsResponse Handle(Guid id, IDocumentSession session)
    {
        var gruppen = new Dictionary<Guid, Perosnaldisposition.Gruppe>();
        var taetigkeiten = new Dictionary<Guid, Taetigkeit>();

        var mitarbeiter = session.Query<Perosnaldisposition.Mitarbeiter>()
                                 .Include(mitarbeiter => mitarbeiter.GruppeId, gruppen)
                                 .Include(mitarbeiter => mitarbeiter.QualifizierteTaetigkeitenIds, taetigkeiten)
                                 .Single(candidate => candidate.Id == id);

        return new MitarbeiterDetailsResponse
               {
                   Id = mitarbeiter.Id,
                   Vorname = mitarbeiter.Vorname,
                   Nachname = mitarbeiter.Nachname,
                   Gruppe = gruppen[mitarbeiter.GruppeId].Name,
                   AbweichendeArbeitszeiten = mitarbeiter.AbweichendeArbeitszeiten,
                   QualifizierteTaetigkeiten =
                       mitarbeiter.QualifizierteTaetigkeitenIds
                                  .Map(taetigkeitenId => taetigkeiten[taetigkeitenId].Name)
               };
    }
}

public record struct MitarbeiterDetailsResponse(
    Guid Id,
    string Vorname,
    string Nachname,
    string Gruppe,
    ImmutableDictionary<DayOfWeek, AbweichendeArbeitszeit> AbweichendeArbeitszeiten,
    IEnumerable<string> QualifizierteTaetigkeiten);