using CSharpFunctionalExtensions;
using Marten;
using Microsoft.AspNetCore.Mvc;
using Perosnaldisposition;
using IResult = Microsoft.AspNetCore.Http.IResult;

namespace API.UseCases.Plaene.Erfassen;

public class AbwesenheitFuerMitarbeiterErfassenEndpoint
{
    public static async Task<IResult> Handle([FromRoute] DateOnly tag, [FromBody] CreateAbwesenheitRequest create, [FromServices] IDocumentSession session)
    {
        var mitarbeiter = await session.LoadAsync<Perosnaldisposition.Mitarbeiter>(create.MitarbeiterId);

        if (mitarbeiter is null)
            return Results.BadRequest($"Der Mitarbeiter mit der Id \"{create.MitarbeiterId}\" wurde nicht gefunden.");
        
        return await Result.Try(() => session.Query<Plan>().Where(plan => plan.Tag == tag).SingleAsync())
                           .OnFailureCompensate(() => Plan.Create(tag))
                           .Bind(plan => plan.BeruecksichtigeAbwesenheitVon(mitarbeiter))
                           .Map(async plan =>
                                {
                                    session.Store(plan);

                                    await session.SaveChangesAsync();

                                    return new CreatePlanResponse(plan.Id);
                                })
                           .Finally(result => result.IsSuccess ? Results.Created() : Results.BadRequest(result.Error));
    }
}

public record struct CreateAbwesenheitRequest(Guid MitarbeiterId);