using BenchmarkDotNet.Running;

namespace UnionContainers.Benchmarks;

class Program
{
    static void Main(string[] args)
    {
        BenchmarkRunner.Run<Benchmarks>();
    }
}