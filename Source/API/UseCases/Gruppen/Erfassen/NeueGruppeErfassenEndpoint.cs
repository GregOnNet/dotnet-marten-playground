using CSharpFunctionalExtensions;
using Marten;
using Microsoft.AspNetCore.Mvc;
using Perosnaldisposition;
using IResult = Microsoft.AspNetCore.Http.IResult;

namespace API.UseCases.Gruppen.Erfassen;

public class NeueGruppeErfassenEndpoint
{
    public static async Task<IResult> Handle([FromBody] CreateGruppeRequest create,
                                             [FromServices] IDocumentSession session)
    {
        return await Gruppe.Create(create.Name)
                           .Map(async gruppe =>
                                {
                                    session.Store(gruppe);

                                    await session.SaveChangesAsync();

                                    return new CreateGruppeResponse(gruppe.Id);
                                })
                           .Finally(result => result.IsSuccess
                                                  ? Results.CreatedAtRoute(nameof(NeueGruppeErfassenEndpoint),
                                                                           result.Value.Id, result.Value)
                                                  : Results.BadRequest(result.Error));
    }
}

public record struct CreateGruppeRequest(string Name);

public record struct CreateGruppeResponse(Guid Id);