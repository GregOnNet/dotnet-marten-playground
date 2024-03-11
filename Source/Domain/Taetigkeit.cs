using CSharpFunctionalExtensions;
using Newtonsoft.Json;

namespace Perosnaldisposition;

public class Taetigkeit
{
    [JsonConstructor]
    private Taetigkeit(string name)
    {
        Name = name;
    }

    public Guid Id { get; set; }
    
    public string Name { get; }

    public static Result<Taetigkeit> Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure<Taetigkeit>("Bitte geben Sie den Namen der Tätigkeit an");

        return Result.Success(new Taetigkeit(name));
    }
}