using API.UseCases.Gruppe.Erfassen;
using CSharpFunctionalExtensions;
using Marten;
using Microsoft.AspNetCore.Mvc;
using IResult = Microsoft.AspNetCore.Http.IResult;

namespace API.UseCases.Mitarbeiter.Erfassen;

public class NeuenMitarbeiterErfassenEndpoint
{
    public static async Task<IResult> Handle([FromBody] CreateMitarbeiterRequest create,
                                             [FromServices] IDocumentSession session)
    {
        return await Perosnaldisposition.Mitarbeiter.Create(create.Vorname, create.Nachname, create.GruppeId)
                                        .Map(async mitarbeiter =>
                                             {
                                                 session.Store(mitarbeiter);
                                            
                                                 await session.SaveChangesAsync();

                                                 return new CreateMitarbeiterResponse(mitarbeiter.Id);
                                             })
                                        .Finally(result => result.IsSuccess
                                                               ? Results.CreatedAtRoute(nameof(NeuenMitarbeiterErfassenEndpoint), result.Value.Id, result.Value)
                                                               : Results.BadRequest(result.Error));
    }
}

public record struct CreateMitarbeiterRequest(Guid GruppeId, string Vorname, string Nachname);
public record struct CreateMitarbeiterResponse(Guid Id);
