using Marten;
using Microsoft.AspNetCore.Mvc;
using Perosnaldisposition;

namespace API.UseCases.Gruppen.Entfernen;

public class GruppeEntfernenEndpoint
{
    public static async Task Handle(Guid id,
                                    [FromServices] IDocumentSession session)
    {
        session.Delete<Gruppe>(id);
        await session.SaveChangesAsync();
    }
}