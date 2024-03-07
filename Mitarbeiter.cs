using CSharpFunctionalExtensions;

namespace Perosnaldisposition;

public class Mitarbeiter
{
    public string Vorname { get; }

    public string Nachname { get; }
    
    public Gruppe Gruppe { get; }

    public IEnumerable<Taetigkeit> QualifizierteTaetigkeiten { get; private set; }
    
    public IEnumerable<Taetigkeit> ZugewieseneTaetigkeiten { get; set; }

    private Mitarbeiter(string vorname, string nachname, Gruppe gruppe)
    {
        Vorname = vorname;
        Nachname = nachname;
        Gruppe = gruppe;

        QualifizierteTaetigkeiten = Array.Empty<Taetigkeit>();
        ZugewieseneTaetigkeiten = Array.Empty<Taetigkeit>();
    }
    
    public static Result<Mitarbeiter> Create(string vorname, string nachname, Gruppe gruppe)
    {
        if (string.IsNullOrWhiteSpace(vorname))
        {
            return Result.Failure<Mitarbeiter>("Bitte geben Sie den Vornamen des Mitarbeiters an.");
        }
        
        if (string.IsNullOrWhiteSpace(nachname))
        {
            return Result.Failure<Mitarbeiter>("Bitte geben Sie den Nachnamen des Mitarbeiters an.");
        }

        if (gruppe is null)
        {
            return Result.Failure<Mitarbeiter>("Bitte weisen Sie dem Mitarbeiter eine Gruppe zu.");
        }
        
        return Result.Success(new Mitarbeiter(vorname, nachname, gruppe));
    }

    public Result WeiseTaetigkeitZu(Taetigkeit taetigkeit)
    {
        var istMitarbeiterFuerTaetigkeitQualifiziert = QualifizierteTaetigkeiten.Any(q => q.Name == taetigkeit.Name);
        
        if (!istMitarbeiterFuerTaetigkeitQualifiziert)
        {
            return Result.Failure($"Dem Mitarbeiter {Vorname} {Nachname} kann die Tätigkeit {taetigkeit.Name} nicht zugewiesen werden, weil er für sie nicht qualifiziert ist.");
        }

        ZugewieseneTaetigkeiten = ZugewieseneTaetigkeiten.Append(taetigkeit);
        
        return Result.Success();
    }

    public Result QualifiziereFuerTaetigkeit(Taetigkeit taetigkeit)
    {
        var istBereitsFuerTaetigkeitQualidizierung =
            QualifizierteTaetigkeiten.Any(qualifizierteTaetigkeit => qualifizierteTaetigkeit.Name == taetigkeit.Name);

        if (istBereitsFuerTaetigkeitQualidizierung)
        {
            return
                Result.Failure($"Dem Mitarbeiter {Vorname} {Nachname} ist für die Tätigkeit {taetigkeit.Name} bereits qualifiziert");
        }
        
        QualifizierteTaetigkeiten = QualifizierteTaetigkeiten.Append(taetigkeit);
        
        return Result.Success();
    }
}