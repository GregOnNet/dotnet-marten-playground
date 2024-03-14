using System.Text.Json.Serialization;
using CSharpFunctionalExtensions;

namespace Perosnaldisposition;

public class Taetigkeit
{
    [JsonConstructor]
    private Taetigkeit(string bezeichnung)
    {
        Id = Guid.NewGuid();
        Bezeichnung = bezeichnung;
    }

    public Guid Id { get; set; }
    
    [JsonInclude]
    public string Bezeichnung { get; private set; }

    public static Result<Taetigkeit> Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure<Taetigkeit>("Bitte geben Sie die Bezeichnung der Tätigkeit an.");

        return Result.Success(new Taetigkeit(name));
    }

    public Result<Taetigkeit> KorrigiereBezeichnung(string neueBezeichnung)
    {
        if (string.IsNullOrWhiteSpace(neueBezeichnung))
            return Result.Failure<Taetigkeit>("Die Bezeichnung einer Tätigkeit darf nicht leer sein.");

        Bezeichnung = neueBezeichnung;
        
        return Result.Success(this);
    }
}