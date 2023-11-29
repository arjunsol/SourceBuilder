using System;

namespace ExampleNamespace;
public class Person
{
    public string Name { get; private set; }

    public Person(string name)
    {
        Name = name;
    }

    public void Greet()
    {
        Console.WriteLine($"Hello, {Name}!");
    }
}