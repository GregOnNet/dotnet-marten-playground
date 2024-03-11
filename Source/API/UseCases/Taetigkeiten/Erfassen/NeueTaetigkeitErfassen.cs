using CSharpFunctionalExtensions.ValueTasks;
using Marten;
using Microsoft.AspNetCore.Mvc;

namespace API.UseCases.Taetigkeiten.Erfassen;

public class NeueTaetigkeitErfassenEndpoint
{
    public static async Task<IResult> Handle([FromBody] CreateTaetigkeitRequest create,
                                             [FromServices] IDocumentSession session)
    {
        return await Perosnaldisposition.Taetigkeit.Create(create.name)
                                        .Map(async taetigkeit =>
                                             {
                                                 session.Store(taetigkeit);

                                                 await session.SaveChangesAsync();

                                                 return new CreateTaetigkeitResponse(taetigkeit.Id);
                                             })
                                        .Finally(result => result.IsSuccess
                                                               ? Results
                                                                  .CreatedAtRoute(nameof(NeueTaetigkeitErfassenEndpoint),
                                                                       result.Value.Id, result.Value)
                                                               : Results.BadRequest(result.Error));
    }
}

public record struct CreateTaetigkeitRequest(string name);

public record struct CreateTaetigkeitResponse(Guid Id);
