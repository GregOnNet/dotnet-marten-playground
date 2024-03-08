using CSharpFunctionalExtensions;
using Newtonsoft.Json;

namespace Perosnaldisposition;

public class Gruppe: IEquatable<Gruppe>
{
    public Guid Id { get; set; }
    
    public string Name { get; private set; }

    [JsonConstructor]
    private Gruppe(string name)
    {
        Name = name;
    }
    
    public static Result<Gruppe> Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Result.Failure<Gruppe>("Bitte geben Sie den Namen der Gruppe an.");
        }

        return Result.Success(new Gruppe(name));
    }

    public bool Equals(Gruppe? andereGruppe)
    {
        if (ReferenceEquals(null, andereGruppe)) return false;
        if (ReferenceEquals(this, andereGruppe)) return true;
        
        return Name == andereGruppe.Name;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        
        if (obj.GetType() != GetType()) return false;
        
        return Equals((Gruppe)obj);
    }

    public override int GetHashCode()
    {
        return Name.GetHashCode();
    }

    public Result<Gruppe> KorrigiereNamen(string korrigierterName)
    {
        if (string.IsNullOrWhiteSpace(korrigierterName))
        {
            return Result.Failure<Gruppe>("Der Name einer Gruppe darf nicht leer sein.");
        }

        Name = korrigierterName;

        return Result.Success(this);
    }
}