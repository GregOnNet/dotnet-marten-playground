﻿using Marten;
using Microsoft.AspNetCore.Mvc;

namespace API.UseCases.Gruppe.Entfernen;

public class GruppeEntfernenEndpoint
{
    public static async Task Handle(Guid id,
                                    [FromServices] IDocumentSession session)
    {
        session.Delete<Perosnaldisposition.Gruppe>(id);
        await session.SaveChangesAsync();
    }
}