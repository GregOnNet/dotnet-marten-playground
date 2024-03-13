using System.Collections.Immutable;
using JasperFx.Core;
using Marten;
using Microsoft.AspNetCore.Mvc;
using Perosnaldisposition;

namespace API.UseCases.Plaene.Details;

public class PlanDetailsEndpoint
{
    public static PlanDetailsResponse Handle([FromRoute] DateOnly tag, IDocumentSession session)
    {
        var plan = session.Query<Plan>()
                          .Single(plan => plan.Tag == tag);

        return new PlanDetailsResponse
               {
                   Id = plan.Id,
                   AbwesendeMitarbeiterIds = plan.AbwesendeMitarbeiterIds,
                   Dispositionen = plan.Dispositionen
               };
    }
}

public record struct PlanDetailsResponse(
    Guid Id,
    IEnumerable<Guid> AbwesendeMitarbeiterIds,
    ImmutableDictionary<Guid, Disposition> Dispositionen
);