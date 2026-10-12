namespace Lab8;

public abstract class SmartDevice
{
    public string Name { get; }
    public abstract string Status { get; }
    public abstract double PowerConsumptionWatts { get; }

    protected SmartDevice(string name) =>
        Name = !string.IsNullOrWhiteSpace(name) ? name : throw new ArgumentException("Имя обязательно.");

    public abstract void TurnOn();
    public abstract void TurnOff();
}

public sealed class SmartLamp(string name, int brightness) : SmartDevice(name)
{
    public int Brightness { get; private set; } = Math.Clamp(brightness, 0, 100);
    public bool IsOn { get; private set; }
    public override string Status => IsOn ? $"включена, яркость {Brightness}%" : "выключена";
    public override double PowerConsumptionWatts => IsOn ? 9 * Brightness / 100d : 0;
    public override void TurnOn() => IsOn = true;
    public override void TurnOff() => IsOn = false;
    public void SetBrightness(int value) => Brightness = Math.Clamp(value, 0, 100);
}

public sealed class SmartThermostat(string name, double targetTemperature) : SmartDevice(name)
{
    public double TargetTemperature { get; private set; } = targetTemperature;
    public bool Heating { get; private set; }
    public override string Status => Heating ? $"нагрев до {TargetTemperature:F1} °C" : "ожидание";
    public override double PowerConsumptionWatts => Heating ? 1200 : 2;
    public override void TurnOn() => Heating = true;
    public override void TurnOff() => Heating = false;
    public void SetTargetTemperature(double value) => TargetTemperature = value is >= 5 and <= 35
        ? value
        : throw new ArgumentOutOfRangeException(nameof(value));
}

public sealed class SmartSpeaker(string name, int volume) : SmartDevice(name)
{
    public int Volume { get; private set; } = Math.Clamp(volume, 0, 100);
    public bool IsPlaying { get; private set; }
    public override string Status => IsPlaying ? $"воспроизведение, громкость {Volume}%" : "пауза";
    public override double PowerConsumptionWatts => IsPlaying ? 15 : 1;
    public override void TurnOn() => IsPlaying = true;
    public override void TurnOff() => IsPlaying = false;
    public void SetVolume(int value) => Volume = Math.Clamp(value, 0, 100);
}

internal static class Program
{
    private static void ShowAndActivate(SmartDevice device)
    {
        Console.WriteLine($"{device.Name}: {device.Status}; мощность {device.PowerConsumptionWatts:F1} Вт");
        device.TurnOn();
        Console.WriteLine($"После включения: {device.Status}; мощность {device.PowerConsumptionWatts:F1} Вт\n");
    }

    private static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        SmartLamp lamp = new("Лампа в кабинете", 70);
        SmartThermostat thermostat = new("Термостат", 22.5);
        SmartSpeaker speaker = new("Колонка", 35);

        lamp.SetBrightness(80);
        thermostat.SetTargetTemperature(23);
        speaker.SetVolume(40);

        ShowAndActivate(lamp);
        ShowAndActivate(thermostat);
        ShowAndActivate(speaker);
    }
}
