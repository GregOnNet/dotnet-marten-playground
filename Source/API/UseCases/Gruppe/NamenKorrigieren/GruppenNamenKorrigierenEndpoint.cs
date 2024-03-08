using CSharpFunctionalExtensions;
using Marten;
using Microsoft.AspNetCore.Mvc;
using IResult = Microsoft.AspNetCore.Http.IResult;

namespace API.UseCases.Gruppe.NamenKorrigieren;

public class GruppenNamenKorrigierenEndpoint
{
    public static async Task<IResult> Handle(Guid id,
                                             [FromBody] KorrigiereGruppenNameRequest korrektur,
                                             [FromServices] IDocumentSession session)
    {
        var d = await session.LoadAsync<Perosnaldisposition.Gruppe>(id);
        
        return await Result.Try(() => session.LoadAsync<Perosnaldisposition.Gruppe>(id))
                           .Ensure(gruppe => gruppe?.Id == id,
                                   $"Es wurde keine Gruppe mit der Id {id} gefunden.")
                           .Bind(gruppe => gruppe!.KorrigiereNamen(korrektur.name))
                           .MapTry(gruppe =>
                                   {
                                       session.Store(gruppe);
                                       return session.SaveChangesAsync();
                                   })
                           .Finally(result => result.IsSuccess ? Results.Ok() : Results.BadRequest(result.Error));
    }
}

public record struct KorrigiereGruppenNameRequest(string name);