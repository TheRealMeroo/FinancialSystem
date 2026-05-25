using BuildingBlocks.Domain.Entities;

namespace Identity.Domain.Entities;

public class Person : Entity
{
    public string Name { get; private set; }
    public DateTime RegisteredAt { get; private set; }

    private Person() { }

    private Person(string name)
    {
        Name = name;
        RegisteredAt = DateTime.UtcNow;
    }

    public static Person Create(string name)
        => new Person(name);

}
