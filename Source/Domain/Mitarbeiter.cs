using System.Collections.Immutable;
using CSharpFunctionalExtensions;
using Newtonsoft.Json;

namespace Perosnaldisposition;

public class Mitarbeiter : IEquatable<Mitarbeiter>
{
    [JsonConstructor]
    public Mitarbeiter(string vorname, string nachname, Guid gruppeId)
    {
        Vorname = vorname;
        Nachname = nachname;
        GruppeId = gruppeId;

        AbweichendeArbeitszeiten = ImmutableDictionary<DayOfWeek, AbweichendeArbeitszeit>.Empty;
        QualifizierteTaetigkeiten = Array.Empty<Taetigkeit>();
    }

    public Guid Id { get; set; }

    public Guid GruppeId { get; }

    public string Vorname { get; }

    public string Nachname { get; }


    public ImmutableDictionary<DayOfWeek, AbweichendeArbeitszeit> AbweichendeArbeitszeiten { get; private set; }
    public IEnumerable<Taetigkeit> QualifizierteTaetigkeiten { get; private set; }


    public bool Equals(Mitarbeiter? andererMitarbeiter)
    {
        if (ReferenceEquals(null, andererMitarbeiter)) return false;
        if (ReferenceEquals(this, andererMitarbeiter)) return true;

        return andererMitarbeiter?.Vorname == Vorname
            && andererMitarbeiter?.Nachname == Nachname
            && andererMitarbeiter.GruppeId == GruppeId;
    }

    public static Result<Mitarbeiter> Create(string vorname, string nachname, Guid gruppeId)
    {
        if (string.IsNullOrWhiteSpace(vorname))
            return Result.Failure<Mitarbeiter>("Bitte geben Sie den Vornamen des Mitarbeiters an.");

        if (string.IsNullOrWhiteSpace(nachname))
            return Result.Failure<Mitarbeiter>("Bitte geben Sie den Nachnamen des Mitarbeiters an.");

        if (gruppeId == Guid.Empty)
            return Result.Failure<Mitarbeiter>("Bitte weisen Sie dem Mitarbeiter eine Gruppe zu.");

        return Result.Success(new Mitarbeiter(vorname, nachname, gruppeId));
    }

    public Result QualifiziereFuer(Taetigkeit taetigkeit)
    {
        var istBereitsFuerTaetigkeitQualidizierung =
            QualifizierteTaetigkeiten.Any(qualifizierteTaetigkeit => qualifizierteTaetigkeit.Name == taetigkeit.Name);

        if (istBereitsFuerTaetigkeitQualidizierung)
            return
                Result.Failure($"Dem Mitarbeiter {Vorname} {Nachname} ist für die Tätigkeit {taetigkeit.Name} bereits qualifiziert");

        QualifizierteTaetigkeiten = QualifizierteTaetigkeiten.Append(taetigkeit);

        return Result.Success();
    }


    public void Vereinbare(AbweichendeArbeitszeit abweichendeArbeitszeit)
    {
        AbweichendeArbeitszeiten = AbweichendeArbeitszeiten.SetItem(abweichendeArbeitszeit.Tag, abweichendeArbeitszeit);
    }

    public void LoeseArbeitszeitVereinbarungFuer(DayOfWeek tag)
    {
        AbweichendeArbeitszeiten = AbweichendeArbeitszeiten.Remove(tag);
    }

    public bool IstQualifiziertFuer(Taetigkeit taetigkeit)
    {
        return QualifizierteTaetigkeiten.Any(q => q.Name == taetigkeit.Name);
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
        return $"{Vorname} {Nachname} {GruppeId}".GetHashCode();
    }
}