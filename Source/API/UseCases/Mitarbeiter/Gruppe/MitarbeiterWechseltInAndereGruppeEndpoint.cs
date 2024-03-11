using CSharpFunctionalExtensions;
using Marten;
using Microsoft.AspNetCore.Mvc;
using IResult = Microsoft.AspNetCore.Http.IResult;

namespace API.UseCases.Mitarbeiter.Gruppe;

public class MitarbeiterWechseltInAndereGruppeEndpoint
{
    public static async Task<IResult> Handle([FromRoute] Guid id,
                                             [FromRoute] Guid gruppeId,
                                             [FromServices] IDocumentSession session)
    {
        // TODO: Können wir das mit einer Result-Monade abbilden?
        var mitarbeiter = await session.LoadAsync<Perosnaldisposition.Mitarbeiter>(id);
        var gruppe = await session.LoadAsync<Perosnaldisposition.Gruppe>(gruppeId);

        if (mitarbeiter is null)
            return Results.BadRequest($"Der Mitarbeiter mit der Id \"{id}\" wurde nicht gefunden");
        if (gruppe is null)
            return Results.BadRequest($"Die Gruppe mit der Id \"{gruppeId}\" wurde nicht gefunden");

        return await mitarbeiter
                    .WechselInGruppe(gruppe)
                    .Map(async mitarbeiterNachGruppenWechsel =>
                         {
                             session.Store(mitarbeiterNachGruppenWechsel);

                             await session.SaveChangesAsync();

                             return Result.Success();
                         })
                    .Finally(result => result.IsSuccess
                                           ? Results.Ok()
                                           : Results.BadRequest(result.Error));
    }
}

public record struct MitarbeiterQualifiziertResponse(Guid Id);