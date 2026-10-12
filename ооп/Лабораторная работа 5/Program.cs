namespace Lab5;

public static class PeopleRegistry
{
    private static readonly Dictionary<int, Person> People = [];

    public static int Count => People.Count;

    public static void Register(Person person)
    {
        ArgumentNullException.ThrowIfNull(person);
        if (!People.TryAdd(person.PersonalNumber, person))
            throw new InvalidOperationException("Личный номер уже зарегистрирован.");
    }

    public static Person? FindByPersonalNumber(int personalNumber) =>
        People.GetValueOrDefault(personalNumber);

    public static IReadOnlyCollection<Person> GetAll() => People.Values.ToArray();
}

public class Person
{
    private static int _lastPersonalNumber;
    private string _fullName = string.Empty;

    public int PersonalNumber { get; }

    public string FullName
    {
        get => _fullName;
        set => _fullName = !string.IsNullOrWhiteSpace(value)
            ? value.Trim()
            : throw new ArgumentException("ФИО не может быть пустым.", nameof(value));
    }

    public DateOnly BirthDate { get; }

    public Person(string fullName, DateOnly birthDate)
    {
        if (birthDate > DateOnly.FromDateTime(DateTime.Today))
            throw new ArgumentOutOfRangeException(nameof(birthDate), "Дата рождения не может быть в будущем.");

        PersonalNumber = Interlocked.Increment(ref _lastPersonalNumber);
        FullName = fullName;
        BirthDate = birthDate;
        PeopleRegistry.Register(this);
    }

    public override string ToString() => $"№{PersonalNumber}: {FullName}, дата рождения {BirthDate:dd.MM.yyyy}";
}

public sealed class Student : Person
{
    public double AverageGrade { get; set; }

    public Student(string fullName, DateOnly birthDate, double averageGrade) : base(fullName, birthDate)
    {
        AverageGrade = averageGrade is >= 0 and <= 5
            ? averageGrade
            : throw new ArgumentOutOfRangeException(nameof(averageGrade));
    }

    public override string ToString() => $"{base.ToString()}, средний балл {AverageGrade:F2}";
}

public sealed class University(string name)
{
    private readonly Dictionary<int, Student> _studentsByRecordBook = [];
    private int _lastRecordBookNumber;

    public string Name { get; } = name;

    public int Enroll(Student student)
    {
        ArgumentNullException.ThrowIfNull(student);
        int recordBookNumber = ++_lastRecordBookNumber;
        _studentsByRecordBook.Add(recordBookNumber, student);
        return recordBookNumber;
    }

    public Student? FindByRecordBook(int number) => _studentsByRecordBook.GetValueOrDefault(number);

    public Student? FindByPersonalNumber(int number) =>
        _studentsByRecordBook.Values.FirstOrDefault(s => s.PersonalNumber == number);

    public bool ExpelByPersonalNumber(int number)
    {
        int? recordBook = _studentsByRecordBook
            .Where(pair => pair.Value.PersonalNumber == number)
            .Select(pair => (int?)pair.Key)
            .FirstOrDefault();
        return recordBook.HasValue && _studentsByRecordBook.Remove(recordBook.Value);
    }
}

internal static class Program
{
    private static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Student anna = new("Соколова Анна Сергеевна", new DateOnly(2006, 4, 15), 4.8);
        Student ivan = new("Петров Иван Олегович", new DateOnly(2005, 11, 2), 4.2);
        University university = new("Технический университет");

        int annaRecordBook = university.Enroll(anna);
        int ivanRecordBook = university.Enroll(ivan);

        Console.WriteLine($"В реестре людей: {PeopleRegistry.Count}");
        Console.WriteLine($"По личному номеру: {PeopleRegistry.FindByPersonalNumber(anna.PersonalNumber)}");
        Console.WriteLine($"Зачётная книжка №{ivanRecordBook}: {university.FindByRecordBook(ivanRecordBook)}");
        Console.WriteLine($"Личный №{anna.PersonalNumber} в университете: {university.FindByPersonalNumber(anna.PersonalNumber)}");
        Console.WriteLine($"Номер зачётной книжки Анны: {annaRecordBook}");
    }
}
