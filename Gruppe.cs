using CSharpFunctionalExtensions;

namespace Perosnaldisposition;

public class Gruppe
{
    private readonly string _name;

    private Gruppe(string name)
    {
        _name = name;
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