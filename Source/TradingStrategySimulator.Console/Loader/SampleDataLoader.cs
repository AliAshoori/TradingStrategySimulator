using System.Text.Json;
using TradingStrategySimulator.Application.Contracts.DTOs;

namespace TradingStrategySimulator.ConsoleApp.Loaders;

internal static class SampleDatasetLoader
{
    private const string SampleDataFolderName = "SampleData";

    public static SampleDataset Load(string datasetFileName)
    {
        if (string.IsNullOrWhiteSpace(datasetFileName))
        {
            throw new ArgumentException("Dataset file name is required.", nameof(datasetFileName));
        }

        string fileName = datasetFileName.EndsWith(".json", StringComparison.OrdinalIgnoreCase)
            ? datasetFileName
            : $"{datasetFileName}.json";

        string filePath = GetDatasetFilePath(fileName);

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException(
                $"Sample dataset file '{fileName}' was not found at path '{filePath}'.",
                filePath);
        }

        return LoadFromFile(filePath);
    }

    public static IReadOnlyCollection<SampleDataset> LoadAll()
    {
        string sampleDataDirectory = GetSampleDataDirectoryPath();

        if (!Directory.Exists(sampleDataDirectory))
        {
            throw new DirectoryNotFoundException(
                $"Sample data directory was not found at path '{sampleDataDirectory}'.");
        }

        string[] datasetFiles = Directory.GetFiles(sampleDataDirectory, "*.json", SearchOption.TopDirectoryOnly);

        if (datasetFiles.Length == 0)
        {
            throw new InvalidOperationException(
                $"No sample dataset files were found in '{sampleDataDirectory}'.");
        }

        List<SampleDataset> datasets = [];

        foreach (string datasetFile in datasetFiles.OrderBy(Path.GetFileNameWithoutExtension))
        {
            datasets.Add(LoadFromFile(datasetFile));
        }

        return datasets;
    }

    private static SampleDataset LoadFromFile(string filePath)
    {
        string json = File.ReadAllText(filePath);

        JsonSerializerOptions options = new()
        {
            PropertyNameCaseInsensitive = true
        };

        SampleDatasetFileModel? fileModel = JsonSerializer.Deserialize<SampleDatasetFileModel>(json, options);

        if (fileModel is null)
        {
            throw new InvalidOperationException(
                $"Failed to deserialize dataset file '{filePath}'.");
        }

        if (string.IsNullOrWhiteSpace(fileModel.Name))
        {
            throw new InvalidOperationException(
                $"Dataset file '{filePath}' must contain a non-empty 'name' field.");
        }

        if (string.IsNullOrWhiteSpace(fileModel.Description))
        {
            throw new InvalidOperationException(
                $"Dataset file '{filePath}' must contain a non-empty 'description' field.");
        }

        if (fileModel.PriceSeries is null || fileModel.PriceSeries.Count == 0)
        {
            throw new InvalidOperationException(
                $"Dataset '{fileModel.Name}' does not contain any price points.");
        }

        return new SampleDataset
        {
            Name = fileModel.Name,
            Description = fileModel.Description,
            PriceSeries = fileModel.PriceSeries.ToList(),
            SourceFileName = Path.GetFileName(filePath)
        };
    }

    private static string GetSampleDataDirectoryPath()
    {
        return Path.Combine(AppContext.BaseDirectory, SampleDataFolderName);
    }

    private static string GetDatasetFilePath(string fileName)
    {
        return Path.Combine(GetSampleDataDirectoryPath(), fileName);
    }

    private sealed class SampleDatasetFileModel
    {
        public string Name { get; init; } = string.Empty;

        public string Description { get; init; } = string.Empty;

        public List<PricePointDto> PriceSeries { get; init; } = [];
    }
}