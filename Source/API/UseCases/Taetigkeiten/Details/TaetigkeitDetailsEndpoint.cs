using CSharpFunctionalExtensions;
using Marten;
using Perosnaldisposition;
using IResult = Microsoft.AspNetCore.Http.IResult;

namespace API.UseCases.Taetigkeiten.Details;

public class TaetigkeitDetailsEndpoint
{
    public static async Task<IResult> Handle(Guid id, IDocumentSession session)
    {
        // var mitarbeiter = session.Load<Perosnaldisposition.Mitarbeiter>(id)
        //                          .Include(mitarbeiter => mitarbeiter.GruppeId, gruppen)
        //                          .Include(mitarbeiter => mitarbeiter.QualifizierteTaetigkeitenIds, taetigkeiten)
        //                          .Single(candidate => candidate.Id == id);
        //
        // return new TaetigkeitDetailsResponse
        //        {
        //            Id = mitarbeiter.Id,
        //            Vorname = mitarbeiter.Vorname,
        //            Nachname = mitarbeiter.Nachname,
        //            Gruppe = gruppen[mitarbeiter.GruppeId].Name,
        //            AbweichendeArbeitszeiten = mitarbeiter.AbweichendeArbeitszeiten,
        //            QualifizierteTaetigkeiten =
        //                mitarbeiter.QualifizierteTaetigkeitenIds
        //                           .Map(taetigkeitenId => taetigkeiten[taetigkeitenId].Bezeichnung)
        //        };

        return await Result.Try(() => session.LoadAsync<Taetigkeit>(id))
                           .Ensure(taetigkeit => taetigkeit?.Id == id,
                                   $"Es wurde keine Tätigkeit mit der Id {id} gefunden.")
                           .Map(taetigkeit => new TaetigkeitDetailsResponse
                                              {
                                                  Id = taetigkeit!.Id,
                                                  Bezeichnung = taetigkeit.Bezeichnung
                                              })
                           .Finally(result => result.IsSuccess
                                                  ? Results.Ok(result.Value)
                                                  : Results.BadRequest(result.Error));
    }
}

public record struct TaetigkeitDetailsResponse(
    Guid Id,
    string Bezeichnung
);