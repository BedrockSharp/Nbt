using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Nbt.Test {
    [TestClass]
    public sealed class ListTests {
        [TestMethod]
        public void InterfaceImplementation() {
            // prepare our test lists
            var referenceList = new List<Tag> {
                new IntTag(1),
                new IntTag(2),
                new IntTag(3)
            };
            var testTag = new IntTag(4);
            var originalList = new ListTag(referenceList);

            // check IList implementation
            IList iList = originalList;
            CollectionAssert.AreEqual(referenceList, iList);

            // check IList<Tag> implementation
            IList<Tag> iGenericList = originalList;
            ListAssert.AreEqual(referenceList, iGenericList, NbtComparer.Instance);
            Assert.IsFalse(iGenericList.IsReadOnly);

            // check IList.Add
            referenceList.Add(testTag);
            iList.Add(testTag);
            CollectionAssert.AreEqual(referenceList, iList);

            // check IList.IndexOf
            Assert.AreEqual(referenceList.IndexOf(testTag), iList.IndexOf(testTag));
            Assert.IsTrue(iList.IndexOf(null) < 0);

            // check IList<Tag>.IndexOf
            Assert.AreEqual(referenceList.IndexOf(testTag), iGenericList.IndexOf(testTag));
            Assert.IsTrue(iGenericList.IndexOf(null) < 0);

            // check IList.Contains
            Assert.IsTrue(iList.Contains(testTag));
            Assert.IsFalse(iList.Contains(null));

            // check IList.Remove
            iList.Remove(testTag);
            Assert.IsFalse(iList.Contains(testTag));

            // check IList.Insert
            iList.Insert(0, testTag);
            Assert.AreEqual(0, iList.IndexOf(testTag));

            // check IList.RemoveAt
            iList.RemoveAt(0);
            Assert.IsFalse(iList.Contains(testTag));

            // check misc IList properties
            Assert.IsFalse(iList.IsFixedSize);
            Assert.IsFalse(iList.IsReadOnly);
            Assert.IsFalse(iList.IsSynchronized);
            Assert.IsNotNull(iList.SyncRoot);

            // check IList.CopyTo
            var exportTest = new IntTag[iList.Count];
            iList.CopyTo(exportTest, 0);
            CollectionAssert.AreEqual(iList, exportTest);

            // check IList.this[int]
            for (int i = 0; i < iList.Count; i++) {
                Assert.AreEqual(originalList[i], iList[i]);
                iList[i] = new IntTag(i);
            }

            // check IList.Clear
            iList.Clear();
            Assert.AreEqual(0, iList.Count);
            Assert.AreEqual(-1, iList.IndexOf(testTag));
        }


        [TestMethod]
        public void IndexerTest() {
            ByteTag ourTag = new ByteTag(1);
            var secondList = new ListTag {
                new ByteTag()
            };

            var testList = new ListTag();
            // Trying to set an out-of-range element
            Assert.Throws<ArgumentOutOfRangeException>(() => testList[0] = new ByteTag(1));

            // Make sure that setting did not affect ListType
            Assert.AreEqual(TagType.Unknown, testList.ListType);
            Assert.AreEqual(0, testList.Count);
            testList.Add(ourTag);

            // set a tag to null
            Assert.Throws<ArgumentNullException>(() => testList[0] = null);

            // set a tag to itself
            Assert.Throws<ArgumentException>(() => testList[0] = testList);

            // give a named tag where an unnamed tag was expected
            Assert.Throws<ArgumentException>(() => testList[0] = new ByteTag("NamedTag"));

            // give a tag of wrong type
            Assert.Throws<ArgumentException>(() => testList[0] = new IntTag(0));

            // give an unnamed tag that already has a parent
            Assert.Throws<ArgumentException>(() => testList[0] = secondList[0]);

            // Make sure that none of the failed insertions went through
            Assert.AreEqual(ourTag, testList[0]);
        }


        [TestMethod]
        public void InitializingListFromCollection() {
            // auto-detecting list type
            var test1 = new ListTag("Test1", new Tag[] {
                new IntTag(1),
                new IntTag(2),
                new IntTag(3)
            });
            Assert.AreEqual(TagType.Int, test1.ListType);

            // check pre-conditions
            Assert.Throws<ArgumentNullException>(() => new ListTag((Tag[])null));
            Assert.Throws<ArgumentNullException>(() => new ListTag(null, null));
            _ = new ListTag((string)null, TagType.Unknown); // does not throw, but creates an empty list
            Assert.Throws<ArgumentNullException>(() => new ListTag((Tag[])null, TagType.Unknown));

            // correct explicitly-given list type
            _ = new ListTag("Test2", new Tag[] {
                new IntTag(1),
                new IntTag(2),
                new IntTag(3)
            }, TagType.Int);

            // wrong explicitly-given list type
            Assert.Throws<ArgumentException>(() => new ListTag("Test3", new Tag[] {
                new IntTag(1),
                new IntTag(2),
                new IntTag(3)
            }, TagType.Float));

            // auto-detecting mixed list given
            Assert.Throws<ArgumentException>(() => new ListTag("Test4", new Tag[] {
                new FloatTag(1),
                new ByteTag(2),
                new IntTag(3)
            }));

            // using AddRange
            new ListTag().AddRange(new Tag[] {
                new IntTag(1),
                new IntTag(2),
                new IntTag(3)
            });
            Assert.Throws<ArgumentNullException>(() => new ListTag().AddRange(null));
        }


        [TestMethod]
        public void ManipulatingList() {
            var sameTags = new Tag[] {
                new IntTag(0),
                new IntTag(1),
                new IntTag(2)
            };

            var list = new ListTag("Test1", sameTags);

            // testing enumerator, indexer, Contains, and IndexOf
            int j = 0;
            foreach (Tag tag in list) {
                Assert.IsTrue(list.Contains(sameTags[j]));
                Assert.AreEqual(sameTags[j], tag);
                Assert.AreEqual(j, list.IndexOf(tag));
                j++;
            }

            // adding an item of correct type
            list.Add(new IntTag(3));
            list.Insert(3, new IntTag(4));

            // adding an item of wrong type
            Assert.Throws<ArgumentException>(() => list.Add(new StringTag()));
            Assert.Throws<ArgumentException>(() => list.Insert(3, new StringTag()));
            Assert.Throws<ArgumentNullException>(() => list.Insert(3, null));

            // testing array contents
            for (int i = 0; i < sameTags.Length; i++) {
                Assert.AreSame(sameTags[i], list[i]);
                Assert.AreEqual(i, ((IntTag)list[i]).Value);
            }

            // test removal
            Assert.IsFalse(list.Remove(new IntTag(5)));
            Assert.IsTrue(list.Remove(sameTags[0]));
            Assert.Throws<ArgumentNullException>(() => list.Remove(null));
            list.RemoveAt(0);
            Assert.Throws<ArgumentOutOfRangeException>(() => list.RemoveAt(10));

            // Test some failure scenarios for Add:
            // adding a list to itself
            var loopList = new ListTag();
            Assert.AreEqual(TagType.Unknown, loopList.ListType);
            Assert.Throws<ArgumentException>(() => loopList.Add(loopList));

            // adding same tag to multiple lists
            Assert.Throws<ArgumentException>(() => loopList.Add(list[0]));
            Assert.Throws<ArgumentException>(() => loopList.Insert(0, list[0]));

            // adding null tag
            Assert.Throws<ArgumentNullException>(() => loopList.Add(null));

            // make sure that all those failed adds didn't affect the tag
            Assert.AreEqual(0, loopList.Count);
            Assert.AreEqual(TagType.Unknown, loopList.ListType);

            // try creating a list with invalid tag type
            Assert.Throws<ArgumentOutOfRangeException>(() => new ListTag((TagType)200));
        }


        [TestMethod]
        public void ChangingListTagType() {
            var list = new ListTag();

            // changing list type to an out-of-range type
            Assert.Throws<ArgumentOutOfRangeException>(() => list.ListType = (TagType)200);

            // failing to add or insert a tag should not change ListType
            Assert.Throws<ArgumentOutOfRangeException>(() => list.Insert(-1, new IntTag()));
            Assert.Throws<ArgumentException>(() => list.Add(new IntTag("namedTagWhereUnnamedIsExpected")));
            Assert.AreEqual(TagType.Unknown, list.ListType);

            // changing the type of an empty list to "End" is allowed, see https://github.com/fragmer/Nbt/issues/12
            list.ListType = TagType.End;
            Assert.AreEqual(list.ListType, TagType.End);

            // changing the type of an empty list back to "Unknown" is allowed too!
            list.ListType = TagType.Unknown;
            Assert.AreEqual(list.ListType, TagType.Unknown);

            // adding the first element should set the tag type
            list.Add(new IntTag());
            Assert.AreEqual(list.ListType, TagType.Int);

            // setting correct type for a non-empty list
            list.ListType = TagType.Int;

            // changing list type to an incorrect type
            Assert.Throws<ArgumentException>(() => list.ListType = TagType.Short);

            // after the list is cleared, we should once again be allowed to change its TagType
            list.Clear();
            list.ListType = TagType.Short;
        }


        [TestMethod]
        public void SerializingWithoutListType() {
            var root = new CompoundTag("root") {
                new ListTag("list")
            };
            var file = new NbtFile(root);

            using (var ms = new MemoryStream()) {
                // list should throw NbtFormatException, because its ListType is Unknown
                Assert.Throws<NbtFormatException>(() => file.SaveToStream(ms, NbtCompression.None));
            }
        }


        [TestMethod]
        public void Serializing1() {
            // check the basics of saving/loading
            const TagType expectedListType = TagType.Int;
            const int elements = 10;

            // construct nbt file
            var writtenFile = new NbtFile(new CompoundTag("ListTypeTest"));
            var writtenList = new ListTag("Entities", null, expectedListType);
            for (int i = 0; i < elements; i++) {
                writtenList.Add(new IntTag(i));
            }
            writtenFile.RootTag.Add(writtenList);

            // test saving
            byte[] data = writtenFile.SaveToBuffer(NbtCompression.None);

            // test loading
            var readFile = new NbtFile();
            long bytesRead = readFile.LoadFromBuffer(data, 0, data.Length, NbtCompression.None);
            Assert.AreEqual(bytesRead, data.Length);

            // check contents of loaded file
            Assert.IsNotNull(readFile.RootTag);
            Assert.IsInstanceOfType<ListTag>(readFile.RootTag["Entities"]);
            var readList = (ListTag)readFile.RootTag["Entities"];
            Assert.AreEqual(writtenList.ListType, readList.ListType);
            Assert.AreEqual(readList.Count, writtenList.Count);

            // check .ToArray
            CollectionAssert.AreEquivalent(readList, readList.ToArray());
            CollectionAssert.AreEquivalent(readList, readList.ToArray<IntTag>());

            // check contents of loaded list
            for (int i = 0; i < elements; i++) {
                Assert.AreEqual(readList.Get<IntTag>(i).Value, writtenList.Get<IntTag>(i).Value);
            }
        }


        [TestMethod]
        public void Serializing2() {
            // check saving/loading lists of all possible value types
            var testFile = new NbtFile(TestFiles.MakeListTest());
            byte[] buffer = testFile.SaveToBuffer(NbtCompression.None);
            long bytesRead = testFile.LoadFromBuffer(buffer, 0, buffer.Length, NbtCompression.None);
            Assert.AreEqual(bytesRead, buffer.Length);
        }


        [TestMethod]
        public void SerializingEmpty() {
            // check saving/loading lists of all possible value types
            var testFile = new NbtFile(new CompoundTag("root") {
                new ListTag("emptyList", TagType.End),
                new ListTag("listyList", TagType.List) {
                    new ListTag(TagType.End)
                }
            });
            byte[] buffer = testFile.SaveToBuffer(NbtCompression.None);

            testFile.LoadFromBuffer(buffer, 0, buffer.Length, NbtCompression.None);

            ListTag list1 = testFile.RootTag.Get<ListTag>("emptyList");
            Assert.AreEqual(list1.Count, 0);
            Assert.AreEqual(list1.ListType, TagType.End);

            ListTag list2 = testFile.RootTag.Get<ListTag>("listyList");
            Assert.AreEqual(list2.Count, 1);
            Assert.AreEqual(list2.ListType, TagType.List);
            Assert.AreEqual(list2.Get<ListTag>(0).Count, 0);
            Assert.AreEqual(list2.Get<ListTag>(0).ListType, TagType.End);
        }


        [TestMethod]
        public void NestedListAndCompoundTest() {
            byte[] data;
            {
                var root = new CompoundTag("Root");
                var outerList = new ListTag("OuterList", TagType.Compound);
                var outerCompound = new CompoundTag();
                var innerList = new ListTag("InnerList", TagType.Compound);
                var innerCompound = new CompoundTag();

                innerList.Add(innerCompound);
                outerCompound.Add(innerList);
                outerList.Add(outerCompound);
                root.Add(outerList);

                var file = new NbtFile(root);
                data = file.SaveToBuffer(NbtCompression.None);
            }
            {
                var file = new NbtFile();
                long bytesRead = file.LoadFromBuffer(data, 0, data.Length, NbtCompression.None);
                Assert.AreEqual(bytesRead, data.Length);
                Assert.AreEqual(1, file.RootTag.Get<ListTag>("OuterList").Count);
                Assert.AreEqual(null, file.RootTag.Get<ListTag>("OuterList").Get<CompoundTag>(0).Name);
                Assert.AreEqual(1,
                                file.RootTag.Get<ListTag>("OuterList")
                                    .Get<CompoundTag>(0)
                                    .Get<ListTag>("InnerList")
                                    .Count);
                Assert.AreEqual(null,
                                file.RootTag.Get<ListTag>("OuterList")
                                    .Get<CompoundTag>(0)
                                    .Get<ListTag>("InnerList")
                                    .Get<CompoundTag>(0)
                                    .Name);
            }
        }


        [TestMethod]
        public void FirstInsertTest() {
            ListTag list = new ListTag();
            Assert.AreEqual(TagType.Unknown, list.ListType);
            list.Insert(0, new IntTag(123));
            // Inserting a tag should set ListType
            Assert.AreEqual(TagType.Int, list.ListType);
        }
    }
}


