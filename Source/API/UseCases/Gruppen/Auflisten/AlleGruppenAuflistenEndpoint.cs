using JasperFx.Core;
using Marten;
using Microsoft.AspNetCore.Mvc;
using Perosnaldisposition;

namespace API.UseCases.Gruppen.Auflisten;

public class AlleGruppenAuflistenEndpoint
{
    public static IEnumerable<GruppeAuflistenReadDto> Handle([FromServices] IDocumentSession session)
    {
        return session.Query<Gruppe>()
                      .Map(gruppe => new GruppeAuflistenReadDto
                                        {
                                            Id = gruppe.Id,
                                            Name = gruppe.Name
                                        });
    }
}

public record struct GruppeAuflistenReadDto(Guid Id, string Name);