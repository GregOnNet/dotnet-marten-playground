using CSharpFunctionalExtensions;
using Marten;
using Microsoft.AspNetCore.Mvc;
using IResult = Microsoft.AspNetCore.Http.IResult;

namespace API.UseCases.Gruppe.Erfassen;

public class ErfassenEndpoint
{
    public static async Task<IResult> Handle([FromBody] CreateGruppeRequest create,
                                             [FromServices] IDocumentSession session)
    {
        return await Perosnaldisposition.Gruppe.Create(create.Name)
                                        .Tap(async gruppe =>
                                             {
                                                 session.Store(gruppe);
                                                 await session.SaveChangesAsync();
                                             })
                                        .Finally(result => result.IsSuccess
                                                               ? Results.Created()
                                                               : Results.BadRequest(result.Error));
    }
}