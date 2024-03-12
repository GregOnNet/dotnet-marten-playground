using CSharpFunctionalExtensions;
using Newtonsoft.Json;

namespace Perosnaldisposition;

public class Disposition
{
    [JsonConstructor]
    private Disposition(Mitarbeiter mitarbeiter, IEnumerable<Taetigkeit> taetigkeiten)
    {
        Mitarbeiter = mitarbeiter;
        TaetigkeitenIds = taetigkeiten.Select(taetigkeit => taetigkeit.Id);
    }

    public Mitarbeiter Mitarbeiter { get; }

    public IEnumerable<Guid> TaetigkeitenIds { get; set; }

    public static Result<Disposition> Create(Mitarbeiter mitarbeiter, Taetigkeit taetigkeit)
    {
        return mitarbeiter
              .IstQualifiziert(taetigkeit)
              .Map(qualifizierteTaetigkeit => new Disposition(mitarbeiter, [qualifizierteTaetigkeit]))
              .MapError(reason => reason);
    }

    public static Result<Disposition> Create(Mitarbeiter mitarbeiter,
                                             IEnumerable<Taetigkeit> taetigkeiten)
    {
        return taetigkeiten
              .Select(taetigkeit => mitarbeiter.IstQualifiziert(taetigkeit)).Combine(", ")
              .Map(qualifizierteTaetigkeiten => new Disposition(mitarbeiter, qualifizierteTaetigkeiten))
              .MapError(reason => reason);
    }
}