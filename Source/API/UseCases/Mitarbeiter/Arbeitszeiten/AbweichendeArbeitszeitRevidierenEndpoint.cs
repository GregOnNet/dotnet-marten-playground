using CSharpFunctionalExtensions;
using Marten;
using Microsoft.AspNetCore.Mvc;
using IResult = Microsoft.AspNetCore.Http.IResult;

namespace API.UseCases.Mitarbeiter.Arbeitszeiten;

public class AbweichendeArbeitszeitRevidierenEndpoint
{
    public static async Task<IResult> Handle([FromRoute] Guid id,
                                             [FromRoute] DayOfWeek tag,
                                             [FromServices] IDocumentSession session)
    {
        var mitarbeiter = await session.LoadAsync<Perosnaldisposition.Mitarbeiter>(id);

        if (mitarbeiter is null)
            return Results.BadRequest($"Der Mitarbeiter mit der Id \"{id}\" wurde nicht gefunden");

        return await mitarbeiter.RevidiereAbweichendeArbeitszeit(tag)
                                .Map(async mitarbeiterMitRevidierterAbweichenderArbeitszeit =>
                                     {
                                         session.Store(mitarbeiterMitRevidierterAbweichenderArbeitszeit);

                                         await session.SaveChangesAsync();

                                         return Result.Success();
                                     })
                                .Finally(result => result.IsSuccess
                                                       ? Results.Ok()
                                                       : Results.BadRequest(result.Error));
    }
}