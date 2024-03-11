using JasperFx.Core;
using Marten;
using Microsoft.AspNetCore.Mvc;

namespace API.UseCases.Taetigkeiten.Auflisten;

public class AlleTaetigkeitenAuflistenEndpoint
{
    public static IEnumerable<TaetigkeitenAuflistenReadDto> Handle([FromServices] IDocumentSession session)
    {
        return session.Query<Perosnaldisposition.Taetigkeit>()
                      .Map(mitarbeiter => new TaetigkeitenAuflistenReadDto
                                          {
                                              Id = mitarbeiter.Id,
                                              Name = mitarbeiter.Name,
                                          });
    }
}

public record struct TaetigkeitenAuflistenReadDto(Guid Id, string Name);