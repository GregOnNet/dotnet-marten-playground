using System.Text.Json.Serialization;
using CSharpFunctionalExtensions;

namespace Perosnaldisposition;

public class Disposition
{
    [JsonConstructor]
    private Disposition(Guid mitarbeiterId, IEnumerable<Guid> taetigkeitenIds)
    {
        MitarbeiterId = mitarbeiterId;
        TaetigkeitenIds = taetigkeitenIds;
    }

    public Guid MitarbeiterId { get; }
    public IEnumerable<Guid> TaetigkeitenIds { get; set; }

    public static Result<Disposition> Create(Mitarbeiter mitarbeiter, Taetigkeit taetigkeit)
    {
        return mitarbeiter
              .IstQualifiziert(taetigkeit)
              .Map(qualifizierteTaetigkeit => new Disposition(mitarbeiter.Id, [qualifizierteTaetigkeit.Id]))
              .MapError(reason => reason);
    }

    public static Result<Disposition> Create(Mitarbeiter mitarbeiter,
                                             IEnumerable<Taetigkeit> taetigkeiten)
    {
        return taetigkeiten
              .Select(taetigkeit => mitarbeiter.IstQualifiziert(taetigkeit)).Combine(", ")
              .Map(qualifizierteTaetigkeiten =>
                       new Disposition(
                                       mitarbeiter.Id,
                                       qualifizierteTaetigkeiten.Select(taetigkeit => taetigkeit.Id)
                                      )
                  )
              .MapError(reason => reason);
    }
}