using CSharpFunctionalExtensions;

namespace Perosnaldisposition;

public class Mitarbeiter: IEquatable<Mitarbeiter>
{
    public string Vorname { get; }

    public string Nachname { get; }
    
    public Gruppe Gruppe { get; }

    public IEnumerable<Taetigkeit> QualifizierteTaetigkeiten { get; private set; }
    
    private Mitarbeiter(string vorname, string nachname, Gruppe gruppe)
    {
        Vorname = vorname;
        Nachname = nachname;
        Gruppe = gruppe;

        QualifizierteTaetigkeiten = Array.Empty<Taetigkeit>();
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

    public Result QualifiziereFuer(Taetigkeit taetigkeit)
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

    public bool IstQualifiziertFuer(Taetigkeit taetigkeit)
    {
        return QualifizierteTaetigkeiten.Any(q => q.Name == taetigkeit.Name);
    }

    public bool Equals(Mitarbeiter? andererMitarbeiter)
    {
        if (ReferenceEquals(null, andererMitarbeiter)) return false;
        if (ReferenceEquals(this, andererMitarbeiter)) return true;
        
        return andererMitarbeiter?.Vorname == Vorname 
            && andererMitarbeiter?.Nachname == Nachname 
            && Gruppe.Equals(andererMitarbeiter.Gruppe);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;

        if (obj.GetType() != GetType()) return false;

        return Equals((Mitarbeiter)obj);
    }

    public override int GetHashCode()
    {
        return $"{Vorname} {Nachname} {Gruppe.Name}".GetHashCode();
    }
}