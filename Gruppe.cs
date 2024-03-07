using CSharpFunctionalExtensions;

namespace Perosnaldisposition;

public class Gruppe
{
    public string Name { get; }

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
}