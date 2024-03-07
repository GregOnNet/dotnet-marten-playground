using CSharpFunctionalExtensions;

namespace Perosnaldisposition;

public class Taetigkeit
{
    public string Name { get; }

    private Taetigkeit(string name)
    {
        Name = name;
    }

    public static Result<Taetigkeit> Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Result.Failure<Taetigkeit>("Bitte geben Sie den Namen der Tätigkeit an");
        }
        
        return Result.Success(new Taetigkeit(name));
    }
}