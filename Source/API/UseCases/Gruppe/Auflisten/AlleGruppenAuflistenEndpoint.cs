using JasperFx.Core;
using Marten;
using Microsoft.AspNetCore.Mvc;

namespace API.UseCases.Gruppe.Auflisten;

public class AlleGruppenAuflistenEndpoint
{
    public static IEnumerable<GruppeAuflistenReadDto> Handle([FromServices] IDocumentSession session)
    {
        return session.Query<Perosnaldisposition.Gruppe>()
                      .Map(gruppe => new GruppeAuflistenReadDto
                                        {
                                            Id = gruppe.Id,
                                            Name = gruppe.Name
                                        });
    }
}

public record struct GruppeAuflistenReadDto(Guid Id, string Name);