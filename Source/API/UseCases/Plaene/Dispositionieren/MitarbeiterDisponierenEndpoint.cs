using CSharpFunctionalExtensions;
using Marten;
using Microsoft.AspNetCore.Mvc;
using Perosnaldisposition;
using IResult = Microsoft.AspNetCore.Http.IResult;

namespace API.UseCases.Plaene.Dispositionieren;

public class MitarbeiterDisponierenEndpoint
{
    public static async Task<IResult> Handle(
        [FromRoute] DateOnly tag,
        [FromBody] DisponiereRequest request,
        [FromServices] IDocumentSession session)
    {
        var mitarbeiter = await session.LoadAsync<Perosnaldisposition.Mitarbeiter>(request.MitarbeiterId);
        var taetigkeiten = await session.LoadManyAsync<Taetigkeit>(request.TaetigkeitenIds);

        if (mitarbeiter is null)
            return Results.BadRequest($"Der Mitarbeiter mit der Id \"{request.MitarbeiterId}\" wurde nicht gefunden.");

        if (taetigkeiten.Count != request.TaetigkeitenIds.Count())
            return
                Results.BadRequest($"Nicht alle Tätigkeiten wurden gefunden (Ids: {string.Join(",", request.TaetigkeitenIds)}");


        return await Result.Try(() => session.Query<Plan>().Where(plan => plan.Tag == tag).SingleAsync())
                           .MapError(_ => $"Es existiert kein Plan für den {tag.ToShortDateString()}.")
                           .Bind(plan => plan.Disponiere(Disposition.Create(mitarbeiter, taetigkeiten).Value))
                           .MapTry(async plan =>
                                   {
                                       session.Store(plan);

                                       await session.SaveChangesAsync();
                                   })
                           .Finally(result => result.IsSuccess
                                                  ? Results.Ok()
                                                  : Results.BadRequest(result.Error));
    }
}

public record struct DisponiereRequest(Guid MitarbeiterId, IEnumerable<Guid> TaetigkeitenIds);

public record struct CreatePlanResponse(Guid Id);