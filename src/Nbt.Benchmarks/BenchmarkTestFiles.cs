using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nbt.Benchmarks {
    public static class BenchmarkTestFiles {
        public static readonly string DirName = Path.Combine(AppContext.BaseDirectory, "TestFiles");
        public static readonly string BigTestFile = Path.Combine(DirName, "bigtest.nbt");
        private static NbtFile bigFile;

        // --- Pre-allocated names and values for benchmarks ---
        private const int ComplexCompoundTagCount = 1000;
        private static readonly string[] IntNames = new string[ComplexCompoundTagCount / 4];
        private static readonly string[] StringNames = new string[ComplexCompoundTagCount / 4];
        private static readonly string[] StringValues = new string[ComplexCompoundTagCount / 4];
        private static readonly string[] ListNames = new string[ComplexCompoundTagCount / 4];
        private static readonly string[] ByteArrayNames = new string[ComplexCompoundTagCount / 4];

        public static void Setup() {
            if (!File.Exists(BigTestFile)) {
                throw new FileNotFoundException(
                    "Benchmark data file not found. Please ensure 'bigtest.nbt' is in the TestFiles sub-directory and set to 'Copy to Output Directory'.",
                    BigTestFile);
            }

            // Pre-generate all strings to avoid allocation during benchmarks
            for (int i = 0; i < ComplexCompoundTagCount / 4; i++) {
                IntNames[i] = $"int_{i}";
                StringNames[i] = $"string_{i}";
                StringValues[i] = $"value_{i}";
                ListNames[i] = $"list_{i}";
                ByteArrayNames[i] = $"byteArray_{i}";
            }
        }

        public static NbtFile GetBigFile() {
            return bigFile ?? (bigFile = new NbtFile(BigTestFile));
        }

        public static CompoundTag MakeComplexCompound() {
            var root = new CompoundTag("root");
            for (int i = 0; i < ComplexCompoundTagCount / 4; i++) {
                root.Add(new IntTag(IntNames[i], i));
                root.Add(new StringTag(StringNames[i], StringValues[i]));
                root.Add(new ListTag(ListNames[i], TagType.Byte) {
                new ByteTag(1), new ByteTag(2), new ByteTag(3)
            });
                root.Add(new ByteArrayTag(ByteArrayNames[i], new byte[64]));
            }
            return root;
        }
    }
}


