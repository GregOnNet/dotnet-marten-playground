﻿using JasperFx.Core;
using Marten;
using Microsoft.AspNetCore.Mvc;
using Perosnaldisposition;

namespace API.UseCases.Taetigkeiten.Auflisten;

public class AlleTaetigkeitenAuflistenEndpoint
{
    public static IEnumerable<TaetigkeitenAuflistenReadDto> Handle([FromServices] IDocumentSession session)
    {
        return session.Query<Taetigkeit>()
                      .Map(mitarbeiter => new TaetigkeitenAuflistenReadDto
                                          {
                                              Id = mitarbeiter.Id,
                                              Name = mitarbeiter.Bezeichnung,
                                          });
    }
}

public record struct TaetigkeitenAuflistenReadDto(Guid Id, string Name);