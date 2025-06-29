using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Reports;

namespace PropertyTree.Tests.Benchmarks
{
    public class JsonExporter : IExporter
    {
        public string Name => "JsonExporter";
        public string Description => "Exports benchmark results to JSON file";

        public void ExportToLog(Summary summary, ILogger logger)
        {
            var results = new List<BenchmarkResult>();

            foreach (var report in summary.Reports)
            {
                var result = new BenchmarkResult
                {
                    Method = report.BenchmarkCase.Descriptor.WorkloadMethod.Name,
                    Mean = report.ResultStatistics?.Mean ?? 0,
                    Median = report.ResultStatistics?.Median ?? 0,
                    StdDev = report.ResultStatistics?.StandardDeviation ?? 0,
                    Min = report.ResultStatistics?.Min ?? 0,
                    Max = report.ResultStatistics?.Max ?? 0,
                    OperationsPerSecond = report.ResultStatistics?.Mean > 0 ? 1.0 / report.ResultStatistics.Mean : 0,
                    AllocatedMemory = 0 // TODO: Fix GcStats property access
                };

                results.Add(result);
            }

            var json = JsonSerializer.Serialize(results, new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            var outputPath = Path.Combine(Directory.GetCurrentDirectory(), "benchmark-results.json");
            File.WriteAllText(outputPath, json);
            logger.WriteLine($"Benchmark results exported to: {outputPath}");
        }

        public IEnumerable<string> ExportToFiles(Summary summary, ILogger consoleLogger)
        {
            ExportToLog(summary, consoleLogger);
            return new[] { Path.Combine(Directory.GetCurrentDirectory(), "benchmark-results.json") };
        }
    }

    public class BenchmarkResult
    {
        public string Method { get; set; } = string.Empty;
        public double Mean { get; set; }
        public double Median { get; set; }
        public double StdDev { get; set; }
        public double Min { get; set; }
        public double Max { get; set; }
        public double OperationsPerSecond { get; set; }
        public long AllocatedMemory { get; set; }
    }
} 