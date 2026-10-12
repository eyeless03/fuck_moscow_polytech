namespace Lab9;

public abstract class Notification
{
    public string Message { get; }
    public int Priority { get; }

    protected Notification(string message, int priority)
    {
        Message = !string.IsNullOrWhiteSpace(message) ? message : throw new ArgumentException("Сообщение обязательно.");
        Priority = priority is >= 1 and <= 5 ? priority : throw new ArgumentOutOfRangeException(nameof(priority));
    }

    public abstract void Send();
    public override string ToString() => $"Приоритет {Priority}: {Message}";
}

public sealed class EmailNotification(string message, int priority, string address)
    : Notification(message, priority), IComparable<EmailNotification>
{
    public string Address { get; } = address;
    public override void Send() => Console.WriteLine($"Email для {Address}: {Message}");
    public int CompareTo(EmailNotification? other) => other is null ? 1 : Priority.CompareTo(other.Priority);
    public override string ToString() => $"Email → {Address}; {base.ToString()}";
}

public sealed class SmsNotification(string message, int priority, string phoneNumber)
    : Notification(message, priority), IComparable<SmsNotification>
{
    public string PhoneNumber { get; } = phoneNumber;
    public override void Send() => Console.WriteLine($"SMS для {PhoneNumber}: {Message}");
    public int CompareTo(SmsNotification? other) => other is null ? 1 : Priority.CompareTo(other.Priority);
    public override string ToString() => $"SMS → {PhoneNumber}; {base.ToString()}";
}

public sealed class PushNotification(string message, int priority, string deviceId)
    : Notification(message, priority), IComparable<PushNotification>
{
    public string DeviceId { get; } = deviceId;
    public override void Send() => Console.WriteLine($"Push для {DeviceId}: {Message}");
    public int CompareTo(PushNotification? other) => other is null ? 1 : Priority.CompareTo(other.Priority);
    public override string ToString() => $"Push → {DeviceId}; {base.ToString()}";
}

public interface INotificationSource<out T> where T : Notification
{
    IEnumerable<T> GetAll();
}

public sealed class NotificationContainer<T> : INotificationSource<T>
    where T : Notification, IComparable<T>
{
    private readonly List<T> _items = [];

    public int Count => _items.Count;
    public bool HasItems => _items.Count > 0;
    public void Add(T notification) => _items.Add(notification ?? throw new ArgumentNullException(nameof(notification)));
    public T Get(int index) => _items[index];
    public bool Contains(T notification) => _items.Contains(notification);
    public void Sort() => _items.Sort();
    public IEnumerable<T> GetAll() => _items.AsReadOnly();
}

internal static class Program
{
    private static void PrintNotifications(INotificationSource<Notification> source)
    {
        foreach (Notification notification in source.GetAll())
            Console.WriteLine(notification);
    }

    private static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        NotificationContainer<EmailNotification> emails = new();
        EmailNotification urgent = new("Срок сдачи сегодня", 1, "student@example.com");
        emails.Add(new EmailNotification("Добро пожаловать", 4, "student@example.com"));
        emails.Add(urgent);
        emails.Sort();

        NotificationContainer<SmsNotification> sms = new();
        sms.Add(new SmsNotification("Код подтверждения: 1234", 2, "+7 900 000-00-00"));

        NotificationContainer<PushNotification> pushes = new();
        pushes.Add(new PushNotification("Доступно обновление", 3, "device-42"));

        Console.WriteLine($"Email-контейнер содержит элементы: {emails.HasItems}");
        Console.WriteLine($"Содержит срочное уведомление: {emails.Contains(urgent)}");
        Console.WriteLine($"Первый элемент после сортировки: {emails.Get(0)}\n");

        INotificationSource<EmailNotification> emailSource = emails;
        INotificationSource<Notification> covariantSource = emailSource;
        Console.WriteLine("Ковариантное представление email-контейнера:");
        PrintNotifications(covariantSource);

        Console.WriteLine("\nДругие контейнеры:");
        PrintNotifications(sms);
        PrintNotifications(pushes);
        urgent.Send();
    }
}
