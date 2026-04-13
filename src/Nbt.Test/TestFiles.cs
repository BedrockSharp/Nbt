using System;
using System.IO;

namespace Nbt.Test {
    public static class TestFiles {
        public static readonly string DirName = Path.Combine(AppContext.BaseDirectory, "TestFiles");
        public static readonly string Small = Path.Combine(DirName, "test.nbt");
        public static readonly string SmallGZip = Path.Combine(DirName, "test.nbt.gz");
        public static readonly string SmallZLib = Path.Combine(DirName, "test.nbt.z");
        public static readonly string Big = Path.Combine(DirName, "bigtest.nbt");
        public static readonly string BigGZip = Path.Combine(DirName, "bigtest.nbt.gz");
        public static readonly string BigZLib = Path.Combine(DirName, "bigtest.nbt.z");


        // creates a compound containing lists of every kind of tag
        public static CompoundTag MakeListTest() {
            return new CompoundTag("Root") {
                new ListTag("ByteList") {
                    new ByteTag(100),
                    new ByteTag(20),
                    new ByteTag(3)
                },
                new ListTag("DoubleList") {
                    new DoubleTag(1d),
                    new DoubleTag(2000d),
                    new DoubleTag(-3000000d)
                },
                new ListTag("FloatList") {
                    new FloatTag(1f),
                    new FloatTag(2000f),
                    new FloatTag(-3000000f)
                },
                new ListTag("IntList") {
                    new IntTag(1),
                    new IntTag(2000),
                    new IntTag(-3000000)
                },
                new ListTag("LongList") {
                    new LongTag(1L),
                    new LongTag(2000L),
                    new LongTag(-3000000L)
                },
                new ListTag("ShortList") {
                    new ShortTag(1),
                    new ShortTag(200),
                    new ShortTag(-30000)
                },
                new ListTag("StringList") {
                    new StringTag("one"),
                    new StringTag("two thousand"),
                    new StringTag("negative three million")
                },
                new ListTag("CompoundList") {
                    new CompoundTag(),
                    new CompoundTag(),
                    new CompoundTag()
                },
                new ListTag("ListList") {
                    new ListTag(TagType.List),
                    new ListTag(TagType.List),
                    new ListTag(TagType.List)
                },
                new ListTag("ByteArrayList") {
                    new ByteArrayTag(new byte[] {
                        1, 2, 3
                    }),
                    new ByteArrayTag(new byte[] {
                        11, 12, 13
                    }),
                    new ByteArrayTag(new byte[] {
                        21, 22, 23
                    })
                },
                new ListTag("IntArrayList") {
                    new IntArrayTag(new[] {
                        1, -2, 3
                    }),
                    new IntArrayTag(new[] {
                        1000, -2000, 3000
                    }),
                    new IntArrayTag(new[] {
                        1000000, -2000000, 3000000
                    })
                },
                new ListTag("LongArrayList") {
                    new LongArrayTag(new long[] {
                        10, -20, 30
                    }),
                    new LongArrayTag(new long[] {
                        100, -200, 300
                    }),
                    new LongArrayTag(new long[] {
                        100, -200, 300
                    })
                }
            };
        }


        // creates a file with lots of compounds and lists, used to test NbtReader compliance
        public static Stream MakeReaderTest() {
            var root = new CompoundTag("root") {
                new IntTag("first"),
                new IntTag("second"),
                new CompoundTag("third-comp") {
                    new IntTag("inComp1"),
                    new IntTag("inComp2"),
                    new IntTag("inComp3")
                },
                new ListTag("fourth-list") {
                    new ListTag {
                        new CompoundTag {
                            new CompoundTag("inList1")
                        }
                    },
                    new ListTag {
                        new CompoundTag {
                            new CompoundTag("inList2")
                        }
                    },
                    new ListTag {
                        new CompoundTag {
                            new CompoundTag("inList3")
                        }
                    }
                },
                new IntTag("fifth"),
                new ByteArrayTag("hugeArray", new byte[1024*1024])
            };
            byte[] testData = new NbtFile(root).SaveToBuffer(NbtCompression.None);
            return new MemoryStream(testData);
        }


        // creates an NbtFile with contents identical to "test.nbt"
        public static NbtFile MakeSmallFile() {
            return new NbtFile(new CompoundTag("hello world") {
                new StringTag("name", "Bananrama")
            });
        }


        public static void AssertNbtSmallFile(NbtFile file) {
            Assert.IsInstanceOfType<CompoundTag>(file.RootTag);

            CompoundTag root = file.RootTag;
            Assert.AreEqual("hello world", root.Name);
            Assert.AreEqual(1, root.Count);

            Assert.IsInstanceOfType<StringTag>(root["name"]);

            var node = (StringTag)root["name"];
            Assert.AreEqual("name", node.Name);
            Assert.AreEqual("Bananrama", node.Value);
        }


        public static void AssertNbtBigFile(NbtFile file) {
            Assert.IsInstanceOfType<CompoundTag>(file.RootTag);

            CompoundTag root = file.RootTag;
            Assert.AreEqual("Level", root.Name);
            Assert.AreEqual(13, root.Count);

            Assert.IsInstanceOfType<LongTag>(root["longTest"]);
            Tag node = root["longTest"];
            Assert.AreEqual("longTest", node.Name);
            Assert.AreEqual(9223372036854775807, ((LongTag)node).Value);

            Assert.IsInstanceOfType<ShortTag>(root["shortTest"]);
            node = root["shortTest"];
            Assert.AreEqual("shortTest", node.Name);
            Assert.AreEqual(32767, ((ShortTag)node).Value);

            Assert.IsInstanceOfType<StringTag>(root["stringTest"]);
            node = root["stringTest"];
            Assert.AreEqual("stringTest", node.Name);
            Assert.AreEqual("HELLO WORLD THIS IS A TEST STRING ÅÄÖ!", ((StringTag)node).Value);

            Assert.IsInstanceOfType<FloatTag>(root["floatTest"]);
            node = root["floatTest"];
            Assert.AreEqual("floatTest", node.Name);
            Assert.AreEqual(0.49823147f, ((FloatTag)node).Value);

            Assert.IsInstanceOfType<IntTag>(root["intTest"]);
            node = root["intTest"];
            Assert.AreEqual("intTest", node.Name);
            Assert.AreEqual(2147483647, ((IntTag)node).Value);

            Assert.IsInstanceOfType<CompoundTag>(root["nested compound test"]);
            node = root["nested compound test"];
            Assert.AreEqual("nested compound test", node.Name);
            Assert.AreEqual(2, ((CompoundTag)node).Count);

            // First nested test
            Assert.IsInstanceOfType<CompoundTag>(node["ham"]);
            var subNode = (CompoundTag)node["ham"];
            Assert.AreEqual("ham", subNode.Name);
            Assert.AreEqual(2, subNode.Count);

            // Checking sub node values
            Assert.IsInstanceOfType<StringTag>(subNode["name"]);
            Assert.AreEqual("name", subNode["name"].Name);
            Assert.AreEqual("Hampus", ((StringTag)subNode["name"]).Value);

            Assert.IsInstanceOfType<FloatTag>(subNode["value"]);
            Assert.AreEqual("value", subNode["value"].Name);
            Assert.AreEqual(0.75, ((FloatTag)subNode["value"]).Value);
            // End sub node

            // Second nested test
            Assert.IsInstanceOfType<CompoundTag>(node["egg"]);
            subNode = (CompoundTag)node["egg"];
            Assert.AreEqual("egg", subNode.Name);
            Assert.AreEqual(2, subNode.Count);

            // Checking sub node values
            Assert.IsInstanceOfType<StringTag>(subNode["name"]);
            Assert.AreEqual("name", subNode["name"].Name);
            Assert.AreEqual("Eggbert", ((StringTag)subNode["name"]).Value);

            Assert.IsInstanceOfType<FloatTag>(subNode["value"]);
            Assert.AreEqual("value", subNode["value"].Name);
            Assert.AreEqual(0.5, ((FloatTag)subNode["value"]).Value);
            // End sub node

            Assert.IsInstanceOfType<ListTag>(root["listTest (long)"]);
            node = root["listTest (long)"];
            Assert.AreEqual("listTest (long)", node.Name);
            Assert.AreEqual(5, ((ListTag)node).Count);

            // The values should be: 11, 12, 13, 14, 15
            for (int nodeIndex = 0; nodeIndex < ((ListTag)node).Count; nodeIndex++) {
                Assert.IsInstanceOfType<LongTag>(node[nodeIndex]);
                Assert.AreEqual(null, node[nodeIndex].Name);
                Assert.AreEqual(nodeIndex + 11, ((LongTag)node[nodeIndex]).Value);
            }

            Assert.IsInstanceOfType<ListTag>(root["listTest (compound)"]);
            node = root["listTest (compound)"];
            Assert.AreEqual("listTest (compound)", node.Name);
            Assert.AreEqual(2, ((ListTag)node).Count);

            // First Sub Node
            Assert.IsInstanceOfType<CompoundTag>(node[0]);
            subNode = (CompoundTag)node[0];

            // First node in sub node
            Assert.IsInstanceOfType<StringTag>(subNode["name"]);
            Assert.AreEqual("name", subNode["name"].Name);
            Assert.AreEqual("Compound tag #0", ((StringTag)subNode["name"]).Value);

            // Second node in sub node
            Assert.IsInstanceOfType<LongTag>(subNode["created-on"]);
            Assert.AreEqual("created-on", subNode["created-on"].Name);
            Assert.AreEqual(1264099775885, ((LongTag)subNode["created-on"]).Value);

            // Second Sub Node
            Assert.IsInstanceOfType<CompoundTag>(node[1]);
            subNode = (CompoundTag)node[1];

            // First node in sub node
            Assert.IsInstanceOfType<StringTag>(subNode["name"]);
            Assert.AreEqual("name", subNode["name"].Name);
            Assert.AreEqual("Compound tag #1", ((StringTag)subNode["name"]).Value);

            // Second node in sub node
            Assert.IsInstanceOfType<LongTag>(subNode["created-on"]);
            Assert.AreEqual("created-on", subNode["created-on"].Name);
            Assert.AreEqual(1264099775885, ((LongTag)subNode["created-on"]).Value);

            Assert.IsInstanceOfType<ByteTag>(root["byteTest"]);
            node = root["byteTest"];
            Assert.AreEqual("byteTest", node.Name);
            Assert.AreEqual(127, ((ByteTag)node).Value);

            const string byteArrayName =
                "byteArrayTest (the first 1000 values of (n*n*255+n*7)%100, starting with n=0 (0, 62, 34, 16, 8, ...))";
            Assert.IsInstanceOfType<ByteArrayTag>(root[byteArrayName]);
            node = root[byteArrayName];
            Assert.AreEqual(byteArrayName, node.Name);
            Assert.AreEqual(1000, ((ByteArrayTag)node).Value.Length);

            // Values are: the first 1000 values of (n*n*255+n*7)%100, starting with n=0 (0, 62, 34, 16, 8, ...)
            for (int n = 0; n < 1000; n++) {
                Assert.AreEqual((n * n * 255 + n * 7) % 100, ((ByteArrayTag)node)[n]);
            }

            Assert.IsInstanceOfType<DoubleTag>(root["doubleTest"]);
            node = root["doubleTest"];
            Assert.AreEqual("doubleTest", node.Name);
            Assert.AreEqual(0.4931287132182315, ((DoubleTag)node).Value);

            Assert.IsInstanceOfType<IntArrayTag>(root["intArrayTest"]);
            var intArrayTag = root.Get<IntArrayTag>("intArrayTest");
            Assert.IsNotNull(intArrayTag);
            Assert.AreEqual(10, intArrayTag.Value.Length);
            var rand = new Random(0);
            for (int i = 0; i < 10; i++) {
                Assert.AreEqual(rand.Next(), intArrayTag.Value[i]);
            }

            Assert.IsInstanceOfType<LongArrayTag>(root["longArrayTest"]);
            var longArrayTag = root.Get<LongArrayTag>("longArrayTest");
            Assert.IsNotNull(longArrayTag);
            Assert.AreEqual(5, longArrayTag.Value.Length);
            var rand2 = new Random(0);
            for (int i = 0; i < 5; i++) {
                Assert.AreEqual(((long)rand2.Next() << 32) | (uint)rand2.Next(), longArrayTag.Value[i]);
            }
        }


        #region Value test

        // creates an CompoundTag with one of tag of each value-type
        public static CompoundTag MakeValueTest() {
            return new CompoundTag("root") {
                new ByteTag("byte", 1),
                new ShortTag("short", 2),
                new IntTag("int", 3),
                new LongTag("long", 4L),
                new FloatTag("float", 5f),
                new DoubleTag("double", 6d),
                new ByteArrayTag("byteArray", new byte[] { 10, 11, 12 }),
                new IntArrayTag("intArray", new[] { 20, 21, 22 }),
                new LongArrayTag("longArray", new long[] { 200, 210, 220 }),
                new StringTag("string", "123")
            };
        }


        public static void AssertValueTest(NbtFile file) {
            Assert.IsInstanceOfType<CompoundTag>(file.RootTag);

            CompoundTag root = file.RootTag;
            Assert.AreEqual("root", root.Name);
            Assert.AreEqual(10, root.Count);

            Assert.IsInstanceOfType<ByteTag>(root["byte"]);
            Tag node = root["byte"];
            Assert.AreEqual("byte", node.Name);
            Assert.AreEqual(1, node.ByteValue);

            Assert.IsInstanceOfType<ShortTag>(root["short"]);
            node = root["short"];
            Assert.AreEqual("short", node.Name);
            Assert.AreEqual(2, node.ShortValue);

            Assert.IsInstanceOfType<IntTag>(root["int"]);
            node = root["int"];
            Assert.AreEqual("int", node.Name);
            Assert.AreEqual(3, node.IntValue);

            Assert.IsInstanceOfType<LongTag>(root["long"]);
            node = root["long"];
            Assert.AreEqual("long", node.Name);
            Assert.AreEqual(4L, node.LongValue);

            Assert.IsInstanceOfType<FloatTag>(root["float"]);
            node = root["float"];
            Assert.AreEqual("float", node.Name);
            Assert.AreEqual(5f, node.FloatValue);

            Assert.IsInstanceOfType<DoubleTag>(root["double"]);
            node = root["double"];
            Assert.AreEqual("double", node.Name);
            Assert.AreEqual(6d, node.DoubleValue);

            Assert.IsInstanceOfType<ByteArrayTag>(root["byteArray"]);
            node = root["byteArray"];
            Assert.AreEqual("byteArray", node.Name);
            CollectionAssert.AreEqual(new byte[] { 10, 11, 12 }, node.ByteArrayValue);

            Assert.IsInstanceOfType<IntArrayTag>(root["intArray"]);
            node = root["intArray"];
            Assert.AreEqual("intArray", node.Name);
            CollectionAssert.AreEqual(new[] { 20, 21, 22 }, node.IntArrayValue);

            Assert.IsInstanceOfType<LongArrayTag>(root["longArray"]);
            node = root["longArray"];
            Assert.AreEqual("longArray", node.Name);
            CollectionAssert.AreEqual(new long[] { 200, 210, 220 }, node.LongArrayValue);

            Assert.IsInstanceOfType<StringTag>(root["string"]);
            node = root["string"];
            Assert.AreEqual("string", node.Name);
            Assert.AreEqual("123", node.StringValue);
        }

        #endregion
    }
}


