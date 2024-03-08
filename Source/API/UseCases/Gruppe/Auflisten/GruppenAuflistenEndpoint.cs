using Marten;
using Microsoft.AspNetCore.Mvc;

namespace API.UseCases.Gruppe.Auflisten;

public class GruppenAuflistenEndpoint
{
    public static IEnumerable<Perosnaldisposition.Gruppe> Handle([FromServices] IDocumentSession session)
    {
        return session.Query<Perosnaldisposition.Gruppe>();
    }
}