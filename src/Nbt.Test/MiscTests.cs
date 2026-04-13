using System;

namespace Nbt.Test {
    [TestClass]
    public class MiscTests {
        [TestMethod]
        public void CopyConstructorTest() {
            ByteTag byteTag = new ByteTag("byteTag", 1);
            ByteTag byteTagClone = (ByteTag)byteTag.Clone();
            Assert.AreNotSame(byteTag, byteTagClone);
            Assert.AreEqual(byteTag.Name, byteTagClone.Name);
            Assert.AreEqual(byteTag.Value, byteTagClone.Value);
            Assert.Throws<ArgumentNullException>(() => new ByteTag((ByteTag)null));

            ByteArrayTag byteArrTag = new ByteArrayTag("byteArrTag", new byte[] { 1, 2, 3, 4 });
            ByteArrayTag byteArrTagClone = (ByteArrayTag)byteArrTag.Clone();
            Assert.AreNotSame(byteArrTag, byteArrTagClone);
            Assert.AreEqual(byteArrTag.Name, byteArrTagClone.Name);
            Assert.AreNotSame(byteArrTag.Value, byteArrTagClone.Value);
            CollectionAssert.AreEqual(byteArrTag.Value, byteArrTagClone.Value);
            Assert.Throws<ArgumentNullException>(() => new ByteArrayTag((ByteArrayTag)null));

            CompoundTag compTag = new CompoundTag("compTag", new Tag[] { new ByteTag("innerTag", 1) });
            CompoundTag compTagClone = (CompoundTag)compTag.Clone();
            Assert.AreNotSame(compTag, compTagClone);
            Assert.AreEqual(compTag.Name, compTagClone.Name);
            Assert.AreNotSame(compTag["innerTag"], compTagClone["innerTag"]);
            Assert.AreEqual(compTag["innerTag"].Name, compTagClone["innerTag"].Name);
            Assert.AreEqual(compTag["innerTag"].ByteValue, compTagClone["innerTag"].ByteValue);
            Assert.Throws<ArgumentNullException>(() => new CompoundTag((CompoundTag)null));

            DoubleTag doubleTag = new DoubleTag("doubleTag", 1);
            DoubleTag doubleTagClone = (DoubleTag)doubleTag.Clone();
            Assert.AreNotSame(doubleTag, doubleTagClone);
            Assert.AreEqual(doubleTag.Name, doubleTagClone.Name);
            Assert.AreEqual(doubleTag.Value, doubleTagClone.Value);
            Assert.Throws<ArgumentNullException>(() => new DoubleTag((DoubleTag)null));

            FloatTag floatTag = new FloatTag("floatTag", 1);
            FloatTag floatTagClone = (FloatTag)floatTag.Clone();
            Assert.AreNotSame(floatTag, floatTagClone);
            Assert.AreEqual(floatTag.Name, floatTagClone.Name);
            Assert.AreEqual(floatTag.Value, floatTagClone.Value);
            Assert.Throws<ArgumentNullException>(() => new FloatTag((FloatTag)null));

            IntTag intTag = new IntTag("intTag", 1);
            IntTag intTagClone = (IntTag)intTag.Clone();
            Assert.AreNotSame(intTag, intTagClone);
            Assert.AreEqual(intTag.Name, intTagClone.Name);
            Assert.AreEqual(intTag.Value, intTagClone.Value);
            Assert.Throws<ArgumentNullException>(() => new IntTag((IntTag)null));

            IntArrayTag intArrTag = new IntArrayTag("intArrTag", new[] { 1, 2, 3, 4 });
            IntArrayTag intArrTagClone = (IntArrayTag)intArrTag.Clone();
            Assert.AreNotSame(intArrTag, intArrTagClone);
            Assert.AreEqual(intArrTag.Name, intArrTagClone.Name);
            Assert.AreNotSame(intArrTag.Value, intArrTagClone.Value);
            CollectionAssert.AreEqual(intArrTag.Value, intArrTagClone.Value);
            Assert.Throws<ArgumentNullException>(() => new IntArrayTag((IntArrayTag)null));

            LongArrayTag longArrTag = new LongArrayTag("longArrTag", new long[] { 1, 2, 3, 4 });
            LongArrayTag longArrTagClone = (LongArrayTag)longArrTag.Clone();
            Assert.AreNotSame(longArrTag, longArrTagClone);
            Assert.AreEqual(longArrTag.Name, longArrTagClone.Name);
            Assert.AreNotSame(longArrTag.Value, longArrTagClone.Value);
            CollectionAssert.AreEqual(longArrTag.Value, longArrTagClone.Value);
            Assert.Throws<ArgumentNullException>(() => new LongArrayTag((LongArrayTag)null));

            ListTag listTag = new ListTag("listTag", new Tag[] { new ByteTag(1) });
            ListTag listTagClone = (ListTag)listTag.Clone();
            Assert.AreNotSame(listTag, listTagClone);
            Assert.AreEqual(listTag.Name, listTagClone.Name);
            Assert.AreNotSame(listTag[0], listTagClone[0]);
            Assert.AreEqual(listTag[0].ByteValue, listTagClone[0].ByteValue);
            Assert.Throws<ArgumentNullException>(() => new ListTag((ListTag)null));

            LongTag longTag = new LongTag("longTag", 1);
            LongTag longTagClone = (LongTag)longTag.Clone();
            Assert.AreNotSame(longTag, longTagClone);
            Assert.AreEqual(longTag.Name, longTagClone.Name);
            Assert.AreEqual(longTag.Value, longTagClone.Value);
            Assert.Throws<ArgumentNullException>(() => new LongTag((LongTag)null));

            ShortTag shortTag = new ShortTag("shortTag", 1);
            ShortTag shortTagClone = (ShortTag)shortTag.Clone();
            Assert.AreNotSame(shortTag, shortTagClone);
            Assert.AreEqual(shortTag.Name, shortTagClone.Name);
            Assert.AreEqual(shortTag.Value, shortTagClone.Value);
            Assert.Throws<ArgumentNullException>(() => new ShortTag((ShortTag)null));

            StringTag stringTag = new StringTag("stringTag", "foo");
            StringTag stringTagClone = (StringTag)stringTag.Clone();
            Assert.AreNotSame(stringTag, stringTagClone);
            Assert.AreEqual(stringTag.Name, stringTagClone.Name);
            Assert.AreEqual(stringTag.Value, stringTagClone.Value);
            Assert.Throws<ArgumentNullException>(() => new StringTag((StringTag)null));
        }


        [TestMethod]
        public void ByteArrayIndexerTest() {
            // test getting/settings values of byte array tag via indexer
            var byteArray = new ByteArrayTag("Test");
            CollectionAssert.AreEqual(new byte[0], byteArray.Value);
            byteArray.Value = new byte[] {
                1, 2, 3
            };
            Assert.AreEqual(1, byteArray[0]);
            Assert.AreEqual(2, byteArray[1]);
            Assert.AreEqual(3, byteArray[2]);
            byteArray[0] = 4;
            Assert.AreEqual(4, byteArray[0]);
        }


        [TestMethod]
        public void IntArrayIndexerTest() {
            // test getting/settings values of int array tag via indexer
            var intArray = new IntArrayTag("Test");
            CollectionAssert.AreEqual(new int[0], intArray.Value);
            intArray.Value = new[] {
                1, 2000, -3000000
            };
            Assert.AreEqual(1, intArray[0]);
            Assert.AreEqual(2000, intArray[1]);
            Assert.AreEqual(-3000000, intArray[2]);
            intArray[0] = 4;
            Assert.AreEqual(4, intArray[0]);
        }


        [TestMethod]
        public void LongArrayIndexerTest() {
            var longArray = new LongArrayTag("Test");
            CollectionAssert.AreEqual(new long[0], longArray.Value);
            longArray.Value = new[] {
                1,
                Int64.MaxValue,
                Int64.MinValue
            };
            Assert.AreEqual(1, longArray[0]);
            Assert.AreEqual(Int64.MaxValue, longArray[1]);
            Assert.AreEqual(Int64.MinValue, longArray[2]);
            longArray[0] = 4;
            Assert.AreEqual(4, longArray[0]);
        }


        [TestMethod]
        public void DefaultValueTest() {
            // test default values of all value tags
            Assert.AreEqual(0, new ByteTag("test").Value);
            CollectionAssert.AreEqual(new byte[0], new ByteArrayTag("test").Value);
            Assert.AreEqual(0d, new DoubleTag("test").Value);
            Assert.AreEqual(0f, new FloatTag("test").Value);
            Assert.AreEqual(0, new IntTag("test").Value);
            CollectionAssert.AreEqual(new int[0], new IntArrayTag("test").Value);
            CollectionAssert.AreEqual(new long[0], new LongArrayTag("test").Value);
            Assert.AreEqual(0L, new LongTag("test").Value);
            Assert.AreEqual(0, new ShortTag("test").Value);
            Assert.AreEqual("", new StringTag().Value);
        }


        [TestMethod]
        public void NullValueTest() {
            Assert.Throws<ArgumentNullException>(() => new ByteArrayTag().Value = null);
            Assert.Throws<ArgumentNullException>(() => new IntArrayTag().Value = null);
            Assert.Throws<ArgumentNullException>(() => new LongArrayTag().Value = null);
            Assert.Throws<ArgumentNullException>(() => new StringTag().Value = null);
        }


        [TestMethod]
        public void NbtTagNameTest() {
            Assert.AreEqual("TAG_End", Tag.GetCanonicalTagName(TagType.End));
            Assert.IsNull(Tag.GetCanonicalTagName((TagType)255));
        }


        [TestMethod]
        public void PathTest() {
            // test Tag.Path property
            var testComp = new CompoundTag {
                new CompoundTag("Compound") {
                    new CompoundTag("InsideCompound")
                },
                new ListTag("List") {
                    new CompoundTag {
                        new IntTag("InsideCompoundAndList")
                    }
                }
            };

            // parent-less tag with no name has empty string for a path
            Assert.AreEqual("", testComp.Path);
            Assert.AreEqual(".Compound", testComp["Compound"].Path);
            Assert.AreEqual(".Compound.InsideCompound", testComp["Compound"]["InsideCompound"].Path);
            Assert.AreEqual(".List", testComp["List"].Path);

            // tags inside lists have no name, but they do have an index
            Assert.AreEqual(".List[0]", testComp["List"][0].Path);
            Assert.AreEqual(".List[0].InsideCompoundAndList", testComp["List"][0]["InsideCompoundAndList"].Path);
        }


        [TestMethod]
        public void BadParamsTest() {
            Assert.Throws<ArgumentNullException>(() => new ByteArrayTag((byte[])null));
            Assert.Throws<ArgumentNullException>(() => new IntArrayTag((int[])null));
            Assert.Throws<ArgumentNullException>(() => new LongArrayTag((long[])null));
            Assert.Throws<ArgumentNullException>(() => new StringTag((string)null));
        }
    }
}


