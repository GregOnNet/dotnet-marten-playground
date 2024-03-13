using CSharpFunctionalExtensions;
using Newtonsoft.Json;

namespace Perosnaldisposition;

public class Disposition
{
    [JsonConstructor]
    private Disposition(Guid mitarbeiterId, IList<Guid> taetigkeitenIds)
    {
        MitarbeiterId = mitarbeiterId;
        TaetigkeitenIds = taetigkeitenIds;
    }

    public Guid MitarbeiterId { get; }
    public IList<Guid> TaetigkeitenIds { get; set; }

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
                                       // TODO: We use .ToList(), because we persist Data with Marten
                                       //       CSharpFunctionalExtension's Combine yields a SelectListIterator<Result<Taetigkeit>, Taetigkeit>
                                       //       Marten persists type information in order to instantiate and hydrate instances of the stored data.
                                       //       In the end Newtonsoft. JSON cannot create a list contains Result<T>.
                                       //       After calling .ToList() the Result-type is gone.
                                       qualifizierteTaetigkeiten.Select(taetigkeit => taetigkeit.Id).ToList()
                                      )
                  )
              .MapError(reason => reason);
    }
}