using CSharpFunctionalExtensions;
using Marten;
using Microsoft.AspNetCore.Mvc;
using IResult = Microsoft.AspNetCore.Http.IResult;

namespace API.UseCases.Mitarbeiter.Arbeitszeiten;

public class AbweichendeArbeitszeitErfassenEndpoint
{
    public static async Task<IResult> Handle([FromRoute] Guid id,
                                             [FromRoute] DayOfWeek tag,
                                             [FromBody] CreateAbweichendeArbeitszeitRequest create,
                                             [FromServices] IDocumentSession session)
    {

        var mitarbeiter = await session.LoadAsync<Perosnaldisposition.Mitarbeiter>(id);

        if (mitarbeiter is null)
            return Results.BadRequest($"Der Mitarbeiter mit der Id \"{id}\" wurde nicht gefunden");
        
        return await Perosnaldisposition.AbweichendeArbeitszeit
                                        .Create(tag, create.Beginn, create.Ende)
                                        .Bind(abweichendeArbeitszeit => mitarbeiter.Vereinbare(abweichendeArbeitszeit))
                                        .Map(async mitarbeiterMitAbweichenderArbeitszeit =>
                                             {
                                                 session.Store(mitarbeiterMitAbweichenderArbeitszeit);

                                                 await session.SaveChangesAsync();

                                                 return new CreateMitarbeiterResponse(mitarbeiterMitAbweichenderArbeitszeit.Id);
                                             })
                                        .Finally(result => result.IsSuccess
                                                               ? Results
                                                                  .CreatedAtRoute(nameof(AbweichendeArbeitszeitErfassenEndpoint),
                                                                       result.Value.Id, result.Value)
                                                               : Results.BadRequest(result.Error));
    }
}

public record struct CreateAbweichendeArbeitszeitRequest(TimeOnly Beginn, TimeOnly Ende);

public record struct CreateMitarbeiterResponse(Guid Id);