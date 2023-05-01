

using Example.EntityAutoMapper.Entities;
using Microsoft.EntityFrameworkCore;

namespace Example.EntityAutoMapper;

internal static class Program
{
    public static int Main(string[] args)
    {
        var benchmark = new Benchmark();

        var detailedBenchmark = true;
        BenchmarkUtil.Start(options =>
        {
            options.Iterations = detailedBenchmark ? 5000 : 1;
            options.IterationsBefore = detailedBenchmark ? 50 : 2;
            options.Benchmarks = new List<Action> { benchmark.TestEfCore, benchmark.TestSam };
            options.GlobalSetup = benchmark.Setup;
            options.IterationCleanup = benchmark.IterationCleanup;
        });
        return 0;
    }
}