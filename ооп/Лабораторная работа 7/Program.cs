using FileSize = long;
using static System.Math;

namespace Lab7;

public class MediaFile
{
    public string Name { get; }
    public FileSize SizeInBytes { get; }

    public MediaFile(string name, FileSize sizeInBytes)
    {
        Name = !string.IsNullOrWhiteSpace(name) ? name : throw new ArgumentException("Имя файла обязательно.");
        SizeInBytes = sizeInBytes >= 0 ? sizeInBytes : throw new ArgumentOutOfRangeException(nameof(sizeInBytes));
    }

    public double SizeInMegabytes => Round(SizeInBytes / 1024d / 1024d, 2);

    public override string ToString() => $"Файл {Name}, {SizeInMegabytes:F2} МБ";
}

public sealed class AudioFile : MediaFile
{
    public int BitrateKbps { get; }

    public AudioFile(string name, FileSize sizeInBytes, int bitrateKbps) : base(name, sizeInBytes) =>
        BitrateKbps = bitrateKbps > 0 ? bitrateKbps : throw new ArgumentOutOfRangeException(nameof(bitrateKbps));

    public double EstimateDurationSeconds() => SizeInBytes * 8d / (BitrateKbps * 1000d);

    public override string ToString() =>
        $"Аудио {Name}, {SizeInMegabytes:F2} МБ, {BitrateKbps} кбит/с, ≈{EstimateDurationSeconds():F1} с";
}

public sealed class VideoFile : MediaFile
{
    public int Width { get; }
    public int Height { get; }

    public VideoFile(string name, FileSize sizeInBytes, int width, int height) : base(name, sizeInBytes)
    {
        Width = width > 0 ? width : throw new ArgumentOutOfRangeException(nameof(width));
        Height = height > 0 ? height : throw new ArgumentOutOfRangeException(nameof(height));
    }

    public string GetResolution() => $"{Width}x{Height}";

    public override string ToString() =>
        $"Видео {Name}, {SizeInMegabytes:F2} МБ, разрешение {GetResolution()}";
}

public sealed class Catalog
{
    private readonly List<MediaFile> _files = [];

    public void Add(MediaFile file)
    {
        ArgumentNullException.ThrowIfNull(file);
        _files.Add(file);
    }

    public (int AudioCount, int VideoCount) GetStatistics() =>
        (_files.OfType<AudioFile>().Count(), _files.OfType<VideoFile>().Count());
}

internal static class Program
{
    private static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        MediaFile readme = new("readme.txt", 8_192);
        AudioFile song = new("song.mp3", 5_242_880, 320);
        VideoFile movie = new("movie.mp4", 734_003_200, 1920, 1080);

        MediaFile[] polymorphicFiles = [readme, song, movie];
        Catalog catalog = new();
        foreach (MediaFile file in polymorphicFiles)
        {
            Console.WriteLine(file);
            catalog.Add(file);
        }

        Console.WriteLine($"Длительность аудио: {song.EstimateDurationSeconds():F1} с");
        Console.WriteLine($"Разрешение видео: {movie.GetResolution()}");
        (int audioCount, int videoCount) = catalog.GetStatistics();
        Console.WriteLine($"Аудиофайлов: {audioCount}; видеофайлов: {videoCount}");
    }
}
