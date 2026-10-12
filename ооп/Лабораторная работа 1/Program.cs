namespace Lab1;

public sealed class Person
{
    public string Name = "Ben";
    public int Age = 18;
    public string Email = "ben@gmail.com";

    public Person(string name)
    {
        Console.WriteLine($"Вызван Person(string name), name = {name}");
        Name = name;
    }

    public Person(string name, int age) : this(name)
    {
        Console.WriteLine($"Вызван Person(string name, int age), age = {age}");
        Age = age;
    }

    public Person(string name, int age, string email) : this("Bob", age)
    {
        Console.WriteLine($"Вызван Person(string name, int age, string email), email = {email}");
        Email = email;
    }

    public string GetInfo() => $"Имя: {Name}; возраст: {Age}; email: {Email}";
}

internal static class Program
{
    private static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        Person person = new("Alice", 25, "alice@example.com");

        Console.WriteLine();
        Console.WriteLine(person.GetInfo());
        Console.WriteLine("Имя равно Bob, потому что третий конструктор вызывает this(\"Bob\", age),");
        Console.WriteLine("а переданный в него параметр name не используется. Конструкторы вызываются от первого к третьему.");
    }
}
