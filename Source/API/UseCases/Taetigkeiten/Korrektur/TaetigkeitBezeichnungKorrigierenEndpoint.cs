using CSharpFunctionalExtensions;
using Marten;
using Microsoft.AspNetCore.Mvc;
using Perosnaldisposition;
using IResult = Microsoft.AspNetCore.Http.IResult;

namespace API.UseCases.Taetigkeiten.Korrektur;

public class TaetigkeitBezeichnungKorrigierenEndpoint
{
    public static async Task<IResult> Handle([FromRoute] Guid id,
                                             [FromBody] KorrigiereBezeichnungRequest request,
                                             [FromServices] IDocumentSession session)
    {
        var d = await session.LoadAsync<Taetigkeit>(id);

        return await Result.Try(() => session.LoadAsync<Taetigkeit>(id))
                           .Ensure(gruppe => gruppe?.Id == id,
                                   $"Es wurde keine Tätigkeit mit der Id {id} gefunden.")
                           .Bind(taetigkeit => taetigkeit!.KorrigiereBezeichnung(request.NeueBezeichnung))
                           .MapTry(taetigkeit =>
                                   {
                                       session.Store(taetigkeit);
                                       return session.SaveChangesAsync();
                                   })
                           .Finally(result => result.IsSuccess ? Results.Ok() : Results.BadRequest(result.Error));
    }
}

public record struct KorrigiereBezeichnungRequest(string NeueBezeichnung);
