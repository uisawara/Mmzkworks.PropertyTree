using BenchmarkDotNet.Running;

namespace PropertyTree.Tests.Benchmarks
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<PropertySystemBenchmarks>();
        }
    }
} 