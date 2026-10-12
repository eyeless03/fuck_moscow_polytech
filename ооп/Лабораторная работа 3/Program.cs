using System.Text.Json;

namespace Lab3.Models
{
    public sealed class Student
    {
        private string _firstName = string.Empty;
        private string _lastName = string.Empty;
        private int _age;
        private double _averageGrade;

        public string FirstName
        {
            get => _firstName;
            set => _firstName = RequireText(value, nameof(FirstName));
        }

        public string LastName
        {
            get => _lastName;
            set => _lastName = RequireText(value, nameof(LastName));
        }

        public int Age
        {
            get => _age;
            set => _age = value is >= 14 and <= 100
                ? value
                : throw new ArgumentOutOfRangeException(nameof(Age), "Возраст должен быть от 14 до 100 лет.");
        }

        public double AverageGrade
        {
            get => _averageGrade;
            set => _averageGrade = value is >= 0 and <= 5
                ? value
                : throw new ArgumentOutOfRangeException(nameof(AverageGrade), "Средний балл должен быть от 0 до 5.");
        }

        public Student(string firstName, string lastName, int age, double averageGrade)
        {
            FirstName = firstName;
            LastName = lastName;
            Age = age;
            AverageGrade = averageGrade;
        }

        public override string ToString() => $"{LastName} {FirstName}, {Age} лет, средний балл {AverageGrade:F2}";

        private static string RequireText(string value, string propertyName) =>
            !string.IsNullOrWhiteSpace(value) ? value.Trim() : throw new ArgumentException("Строка не может быть пустой.", propertyName);
    }
}

namespace Lab3.DataAccess
{
    using Lab3.Models;

    public interface IStudentsRepository
    {
        void Save(IEnumerable<Student> students);
        IReadOnlyList<Student> Load();
    }

    public sealed class StudentsRepository(string filePath) : IStudentsRepository
    {
        private readonly JsonSerializerOptions _options = new() { WriteIndented = true };

        public void Save(IEnumerable<Student> students)
        {
            ArgumentNullException.ThrowIfNull(students);
            File.WriteAllText(filePath, JsonSerializer.Serialize(students, _options));
        }

        public IReadOnlyList<Student> Load()
        {
            if (!File.Exists(filePath)) return [];
            return JsonSerializer.Deserialize<List<Student>>(File.ReadAllText(filePath), _options) ?? [];
        }
    }
}

namespace Lab3.Services
{
    using Lab3.DataAccess;
    using Lab3.Models;

    public sealed class University(IStudentsRepository repository)
    {
        private readonly List<Student> _students = [];

        public IReadOnlyList<Student> Students => _students.AsReadOnly();

        public void AddStudent(Student student)
        {
            ArgumentNullException.ThrowIfNull(student);
            _students.Add(student);
        }

        public bool RemoveStudent(string lastName)
        {
            Student? student = FindStudent(lastName);
            return student is not null && _students.Remove(student);
        }

        public Student? FindStudent(string lastName) =>
            _students.FirstOrDefault(s => s.LastName.Equals(lastName, StringComparison.OrdinalIgnoreCase));

        public void Save() => repository.Save(_students);

        public void Load()
        {
            _students.Clear();
            _students.AddRange(repository.Load());
        }
    }
}

namespace Lab3
{
    using DataAccess;
    using Models;
    using Services;

    internal static class Program
    {
        private static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            string filePath = Path.Combine(AppContext.BaseDirectory, "students.json");
            IStudentsRepository repository = new StudentsRepository(filePath);
            University university = new(repository);

            university.AddStudent(new Student("Анна", "Соколова", 19, 4.75));
            university.AddStudent(new Student("Иван", "Петров", 20, 4.20));
            university.Save();

            Console.WriteLine($"Найден: {university.FindStudent("Петров")}");
            university.RemoveStudent("Петров");
            university.Load();
            Console.WriteLine("Студенты после чтения из файла:");
            foreach (Student student in university.Students) Console.WriteLine(student);
        }
    }
}
