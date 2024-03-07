using CSharpFunctionalExtensions;

namespace Perosnaldisposition;

public class Disposition
{
    public Mitarbeiter Mitarbeiter { get; }
    public Taetigkeit Taetigkeit { get; }

    private Disposition(Mitarbeiter mitarbeiter, Taetigkeit taetigkeit)
    {
        Mitarbeiter = mitarbeiter;
        Taetigkeit = taetigkeit;
    }

    public static Result<Disposition> Create(Mitarbeiter mitarbeiter, Taetigkeit taetigkeit)
    {
        if (mitarbeiter.IstQualifiziertFuer(taetigkeit))
        {
            return Result.Success(new Disposition(mitarbeiter, taetigkeit));
        }
        
        return Result.Failure<Disposition>($"Der Mitarbeiter {mitarbeiter.Vorname} {mitarbeiter.Nachname} ist für die Tätigkeit {taetigkeit.Name} nicht qualifiziert.");
    }
}