using BenchmarkDotNet.Attributes;

namespace Nbt.Benchmarks;

public class EditBenchmarks {

    private Tag bigFileRoot = null!;
    private Tag bigFileRootClone = null!;

    [GlobalSetup]
    public void GlobalSetup() {
        BenchmarkTestFiles.Setup();
        bigFileRoot = BenchmarkTestFiles.GetBigFile().RootTag;
        bigFileRootClone = (Tag)bigFileRoot.Clone();
    }

    [Benchmark(Description = "Create Complex Compound")]
    public CompoundTag InMemoryCreation() {
        return BenchmarkTestFiles.MakeComplexCompound();
    }

    [Benchmark(Description = "Deep Clone Compound")]
    public object CloneBigCompound() {
        return bigFileRoot.Clone();
    }

    [Benchmark(Description = "Lookup Nested Tag")]
    public Tag? LookupNestedTag() {
        return bigFileRoot["nested compound test"]!["ham"]!["name"];
    }

    [Benchmark(Description = "Compare Two Compounds")]
    public bool CompareBigCompounds() {
        return NbtComparer.Instance.Equals(bigFileRoot, bigFileRootClone);
    }
}


