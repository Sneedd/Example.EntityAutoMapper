using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using System.Xml.Linq;

namespace Example.EntityAutoMapper
{
    public class BenchmarkUtil
    {
        public class Options
        {
            public int IterationsBefore { get; set; } = 2;
            public int Iterations { get; set; } = 10;
            public TextWriter Writer { get; set; } = Console.Out;

            public List<Action> Benchmarks { get; set; } = new List<Action>();
            public Action GlobalSetup { get; set; }
            public Action GlobalCleanup { get; set; }
            public Action IterationSetup { get; set; }
            public Action IterationCleanup { get; set; }
        }

        private class Result
        {
            public string Name { get; set; }
            public TimeSpan TimeSpan { get; set; }
            public int Iterations { get; set; }
        }

        public static void Start(Action<Options> onConfigure) 
        {
            if (onConfigure == null)
            {
                throw new ArgumentNullException(nameof(onConfigure));
            }
            
            var options = new Options();
            onConfigure(options);
            
            if (options.Iterations <= 0)
            {
                throw new ArgumentException($"{nameof(Options.Iterations)} must be bigger than 0.");
            }
            if (options.IterationsBefore < 0)
            {
                throw new ArgumentException($"{nameof(Options.IterationsBefore)} must be bigger or equal than 0.");
            }
            if (options.Benchmarks == null || options.Benchmarks.Count == 0)
            {
                throw new ArgumentException($"No benchmarks provided.");
            }

            var util = new BenchmarkUtil
            {
                _options = options,
                _results = new List<Result>()
            };

            try
            {
                var watch = Stopwatch.StartNew();
                util.DoGlobalSetup();
                util.DoBeforeIterations();

                foreach (var benchmark in options.Benchmarks)
                {
                    try
                    {
                        util.DoIterations(benchmark);
                    }
                    catch (OperationCanceledException) { /* Ok, next benchmark */ }
                }

                util.DoGlobalCleanup();

                options.Writer.WriteLine($"Overall benchmark took {watch.Elapsed.TotalMilliseconds} ms");
                util.PrintOverview();
            }
            catch (OperationCanceledException) { /* Ok, aborting */ }            
        }

        private Options _options;
        private List<Result> _results;

        private void DoGlobalSetup()
        {
            if (_options.GlobalSetup != null)
            {
                _options.Writer.Write("Preparing global setup ... ");
                var watch = Stopwatch.StartNew();
                try
                {
                    _options.GlobalSetup();
                }
                catch (Exception ex)
                {
                    _options.Writer.WriteLine("failed with following exeption.");
                    _options.Writer.WriteLine(ex.ToString());
                    throw new OperationCanceledException();
                }
                _options.Writer.WriteLine($"took {watch.Elapsed.TotalMilliseconds:0.000} ms");
            }
        }

        private void DoBeforeIterations()
        {
            _options.Writer.Write($"Before benchmark iterations (Count = {_options.IterationsBefore}*{_options.Benchmarks.Count}):");
            var aroundTenthTimes = _options.IterationsBefore * _options.Benchmarks.Count / 10;
            if (aroundTenthTimes <= 0) aroundTenthTimes = 1;
            var watch = Stopwatch.StartNew();
            foreach (var benchmark in _options.Benchmarks)
            {
                for (int i = 0; i < _options.IterationsBefore; ++i)
                {
                    if (i % aroundTenthTimes == aroundTenthTimes - 1)
                    {
                        _options.Writer.Write('.');
                    }
                    this.DoIterationSetup();

                    try
                    {
                        benchmark.Invoke();
                    }
                    catch (Exception ex)
                    {
                        _options.Writer.WriteLine(" failed with following exeption.");
                        _options.Writer.WriteLine(ex.ToString());
                        break;
                    }

                    this.DoIterationCleanup();
                }
            }
            _options.Writer.WriteLine($" took {watch.Elapsed.TotalMilliseconds:0.000} ms");
        }

        private void DoIterationSetup()
        {
            if (_options.IterationSetup != null)
            {
                try
                {
                    _options.IterationSetup();
                }
                catch (Exception ex)
                {
                    _options.Writer.WriteLine(" Iteration setup failed with following exeption.");
                    _options.Writer.WriteLine(ex.ToString());
                    throw new OperationCanceledException();
                }
            }
        }

        private void DoCollect()
        {
            for (int i = 0; i < 3; ++i)
            {
                GC.Collect();
            }
        }

        private void DoIterations(Action benchmark)
        {
            _options.Writer.Write($"Benchmark '{benchmark.Method.Name}' iterations (Count = {_options.Iterations}): ");

            var watch = new Stopwatch();
            var aroundTenthTimes = _options.Iterations / 10;
            if (aroundTenthTimes <= 0) aroundTenthTimes = 1;
            for (int i = 0; i < _options.Iterations; ++i)
            {
                if (i % aroundTenthTimes == aroundTenthTimes - 1)
                {
                    _options.Writer.Write('.');
                }
                this.DoIterationSetup();
                this.DoCollect();

                try
                {
                    watch.Start();
                    benchmark.Invoke();
                    watch.Stop();
                }
                catch (Exception ex)
                {
                    _options.Writer.WriteLine(" Iteration failed with following exeption.");
                    _options.Writer.WriteLine(ex.ToString());
                    break;
                }

                this.DoIterationCleanup();
            }

            _options.Writer.WriteLine($" took {watch.Elapsed.TotalMilliseconds:0.000} ms");
            _results.Add(new Result
            {
                Name = benchmark.Method.Name,
                TimeSpan = watch.Elapsed,
                Iterations = _options.Iterations,
            });
        }

        private void DoIterationCleanup()
        {
            if (_options.IterationCleanup != null)
            {
                try
                {
                    _options.IterationCleanup();
                }
                catch (Exception ex)
                {
                    _options.Writer.WriteLine(" Iteration cleanup failed with following exeption.");
                    _options.Writer.WriteLine(ex.ToString());
                    throw new OperationCanceledException();
                }
            }
        }

        private void DoGlobalCleanup()
        {
            if (_options.GlobalCleanup != null)
            {
                _options.Writer.Write("Preparing global cleanup ... ");
                var watch = Stopwatch.StartNew();
                try
                {
                    _options.GlobalCleanup();
                }
                catch (Exception ex)
                {
                    _options.Writer.WriteLine("failed with following exeption.");
                    _options.Writer.WriteLine(ex.ToString());
                    throw new OperationCanceledException();
                }
                _options.Writer.Write($"took {watch.Elapsed:0.000} ms");
            }
        }

        private void PrintOverview()
        {
            _options.Writer.WriteLine();
            _options.Writer.WriteLine("## Benchmark result");
            _options.Writer.WriteLine();
            string format = "| {0,-30} | {1,15} | {2,15:0.000} | {3,15:N0} |";
            _options.Writer.WriteLine(format, "Name", "Iteration", "Global", "Count");
            _options.Writer.WriteLine("|--------------------------------|-----------------|-----------------|-----------------|");

            foreach (var result in _results)
            {
                _options.Writer.WriteLine(format, 
                    result.Name, 
                    (result.TimeSpan.TotalMilliseconds / (double)result.Iterations).ToString("0.000") + " ms",
                    result.TimeSpan.TotalMilliseconds.ToString("0.000") + " ms",
                    result.Iterations.ToString("N0"));
            }
        }
    }
}
