using System.Collections.Immutable;
using JasperFx.Core;
using Marten;
using Microsoft.AspNetCore.Mvc;

namespace API.UseCases.Mitarbeiter.Auflisten;

public class AlleMitarbeiterAuflistenEndpoint
{
    public static IEnumerable<MitarbeiterAuflistenReadDto> Handle([FromServices] IDocumentSession session)
    {
        var gruppen = new Dictionary<Guid, Perosnaldisposition.Gruppe>();
        var taetigkeiten = new Dictionary<Guid, Perosnaldisposition.Taetigkeit>();
        
        return session.Query<Perosnaldisposition.Mitarbeiter>()
                      .Include(mitarbeiter => mitarbeiter.GruppeId, gruppen)
                      .Include(mitarbeiter => mitarbeiter.QualifizierteTaetigkeitenIds, taetigkeiten)
                      .Map(mitarbeiter => new MitarbeiterAuflistenReadDto
                                          {
                                              Id = mitarbeiter.Id,
                                              Vorname = mitarbeiter.Vorname,
                                              Nachname = mitarbeiter.Nachname,
                                              // TODO: Ich weiß nicht, ob das die beste Variante ist eine Projektion zu machen.
                                              //       Ich muss extra ein Dictionary für "gruppen" anlegen.
                                              //       Die API arbeitet anders als EntityFramework.
                                              //       (1:1 Beziehung)
                                              Gruppe = gruppen[mitarbeiter.GruppeId].Name,
                                              // TODO: Fühlt sich auch komisch an.
                                              //       Für jedes Dokument muss ein Dictionary erzeugt werden.
                                              //       (1:n Beziehung).
                                              QualifizierteTaetigkeiten = mitarbeiter.QualifizierteTaetigkeitenIds.Map(id => taetigkeiten[id].Name)
                                          });
    }
}

public record struct MitarbeiterAuflistenReadDto(Guid Id, string Vorname, string Nachname, string Gruppe, IEnumerable<string> QualifizierteTaetigkeiten);