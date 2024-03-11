using CSharpFunctionalExtensions;
using Marten;
using Microsoft.AspNetCore.Mvc;
using Perosnaldisposition;
using IResult = Microsoft.AspNetCore.Http.IResult;

namespace API.UseCases.Mitarbeiter.Qualifizieren;

public class MitarbeiterQualifikationEntziehenEndpoint
{
    public static async Task<IResult> Handle([FromRoute] Guid id,
                                             [FromRoute] Guid taetigkeitId,
                                             [FromServices] IDocumentSession session)
    {
        // TODO: Können wir das mit einer Result-Monade abbilden?
        var mitarbeiter = await session.LoadAsync<Perosnaldisposition.Mitarbeiter>(id);
        var taetigkeit = await session.LoadAsync<Taetigkeit>(taetigkeitId);

        if (mitarbeiter is null)
            return Results.BadRequest($"Der Mitarbeiter mit der Id \"{id}\" wurde nicht gefunden");
        if (taetigkeit is null)
            return Results.BadRequest($"Die Tätigkeit mit der Id \"{taetigkeitId}\" wurde nicht gefunden");

        return await mitarbeiter
                    .EntzieheQualifikationFuer(taetigkeit)
                    .Map(async mitarbeiterMitEntzogenerQualifikation =>
                         {
                             session.Store(mitarbeiterMitEntzogenerQualifikation);

                             await session.SaveChangesAsync();

                             return new
                                 MitarbeiterMitEntzogenerQualifikationResponse(mitarbeiterMitEntzogenerQualifikation
                                                                                  .Id);
                         })
                    .Finally(result => result.IsSuccess
                                           ? Results.Ok()
                                           : Results.BadRequest(result.Error));
    }
}

public record struct MitarbeiterMitEntzogenerQualifikationResponse(Guid Id);