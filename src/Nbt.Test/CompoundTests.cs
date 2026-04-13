using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Nbt.Test {
    [TestClass]
    public sealed class CompoundTests {
        [TestMethod]
        public void InitializingCompoundFromCollectionTest() {
            Tag[] allNamed = {
                new ShortTag("allNamed1", 1),
                new LongTag("allNamed2", 2),
                new IntTag("allNamed3", 3)
            };

            Tag[] someUnnamed = {
                new IntTag("someUnnamed1", 1),
                new IntTag(2),
                new IntTag("someUnnamed3", 3)
            };

            Tag[] someNull = {
                new IntTag("someNull1", 1),
                null,
                new IntTag("someNull3", 3)
            };

            Tag[] dupeNames = {
                new IntTag("dupeNames1", 1),
                new IntTag("dupeNames2", 2),
                new IntTag("dupeNames1", 3)
            };

            // null collection, should throw
            Assert.Throws<ArgumentNullException>(() => new CompoundTag("nullTest", null));

            // proper initialization
            CompoundTag allNamedTest = new CompoundTag("allNamedTest", allNamed);
            CollectionAssert.AreEquivalent(allNamed, allNamedTest);

            // some tags are unnamed, should throw
            Assert.Throws<ArgumentException>(() => new CompoundTag("someUnnamedTest", someUnnamed));

            // some tags are null, should throw
            Assert.Throws<ArgumentNullException>(() => new CompoundTag("someNullTest", someNull));

            // some tags have same names, should throw
            Assert.Throws<ArgumentException>(() => new CompoundTag("dupeNamesTest", dupeNames));
        }


        [TestMethod]
        public void GettersAndSetters() {
            // construct a document for us to test.
            var nestedChild = new CompoundTag("NestedChild");
            var nestedInt = new IntTag(1);
            var nestedChildList = new ListTag("NestedChildList") {
                nestedInt
            };
            var child = new CompoundTag("Child") {
                nestedChild,
                nestedChildList
            };
            var childList = new ListTag("ChildList") {
                new IntTag(1)
            };
            var parent = new CompoundTag("Parent") {
                child,
                childList
            };

            // Accessing nested compound tags using indexers
            Assert.AreEqual(nestedChild, parent["Child"]["NestedChild"]);
            Assert.AreEqual(nestedChildList, parent["Child"]["NestedChildList"]);
            Assert.AreEqual(nestedInt, parent["Child"]["NestedChildList"][0]);

            // Accessing nested compound tags using Get and Get<T>
            Assert.Throws<ArgumentNullException>(() => parent.Get<CompoundTag>(null));
            Assert.IsNull(parent.Get<CompoundTag>("NonExistingChild"));
            Assert.AreEqual(nestedChild, parent.Get<CompoundTag>("Child").Get<CompoundTag>("NestedChild"));
            Assert.AreEqual(nestedChildList, parent.Get<CompoundTag>("Child").Get<ListTag>("NestedChildList"));
            Assert.AreEqual(nestedInt, parent.Get<CompoundTag>("Child").Get<ListTag>("NestedChildList")[0]);
            Assert.Throws<ArgumentNullException>(() => parent.Get(null));
            Assert.IsNull(parent.Get("NonExistingChild"));
            Assert.AreEqual(nestedChild, (parent.Get("Child") as CompoundTag).Get("NestedChild"));
            Assert.AreEqual(nestedChildList, (parent.Get("Child") as CompoundTag).Get("NestedChildList"));
            Assert.AreEqual(nestedInt, (parent.Get("Child") as CompoundTag).Get("NestedChildList")[0]);

            // Accessing with Get<T> and an invalid given type
            Assert.Throws<InvalidCastException>(() => parent.Get<IntTag>("Child"));

            // Using TryGet and TryGet<T>
            Tag dummyTag;
            Assert.Throws<ArgumentNullException>(() => parent.TryGet(null, out dummyTag));
            Assert.IsFalse(parent.TryGet("NonExistingChild", out dummyTag));
            Assert.IsTrue(parent.TryGet("Child", out dummyTag));
            CompoundTag dummyCompoundTag;
            Assert.Throws<ArgumentNullException>(() => parent.TryGet(null, out dummyCompoundTag));
            Assert.IsFalse(parent.TryGet("NonExistingChild", out dummyCompoundTag));
            Assert.IsTrue(parent.TryGet("Child", out dummyCompoundTag));

            // Trying to use integer indexers on non-ListTag tags
            Assert.Throws<InvalidOperationException>(() => parent[0] = nestedInt);
            Assert.Throws<InvalidOperationException>(() => nestedInt[0] = nestedInt);

            // Trying to use string indexers on non-CompoundTag tags
            Assert.Throws<InvalidOperationException>(() => dummyTag = childList["test"]);
            Assert.Throws<InvalidOperationException>(() => childList["test"] = nestedInt);
            Assert.Throws<InvalidOperationException>(() => nestedInt["test"] = nestedInt);

            // Trying to get a non-existent element by name
            Assert.IsNull(parent.Get<Tag>("NonExistentTag"));
            Assert.IsNull(parent["NonExistentTag"]);

            // Null indices on CompoundTag
            Assert.Throws<ArgumentNullException>(() => parent.Get<Tag>(null));
            Assert.Throws<ArgumentNullException>(() => parent[null] = new IntTag(1));
            Assert.Throws<ArgumentNullException>(() => nestedInt = (IntTag)parent[null]);

            // Out-of-range indices on ListTag
            Assert.Throws<ArgumentOutOfRangeException>(() => nestedInt = (IntTag)childList[-1]);
            Assert.Throws<ArgumentOutOfRangeException>(() => childList[-1] = new IntTag(1));
            Assert.Throws<ArgumentOutOfRangeException>(() => nestedInt = childList.Get<IntTag>(-1));
            Assert.Throws<ArgumentOutOfRangeException>(() => nestedInt = (IntTag)childList[childList.Count]);
            Assert.Throws<ArgumentOutOfRangeException>(() => nestedInt = childList.Get<IntTag>(childList.Count));

            // Using setter correctly
            parent["NewChild"] = new ByteTag("NewChild");

            // Using setter incorrectly
            object dummyObject;
            Assert.Throws<ArgumentNullException>(() => parent["Child"] = null);
            Assert.IsNotNull(parent["Child"]);
            Assert.Throws<ArgumentException>(() => parent["Child"] = new ByteTag("NotChild"));
            Assert.Throws<InvalidOperationException>(() => dummyObject = parent[0]);
            Assert.Throws<InvalidOperationException>(() => parent[0] = new ByteTag("NewerChild"));

            // Try adding tag to self
            var selfTest = new CompoundTag("SelfTest");
            Assert.Throws<ArgumentException>(() => selfTest["SelfTest"] = selfTest);

            // Try adding a tag that already has a parent
            Assert.Throws<ArgumentException>(() => selfTest[child.Name] = child);
        }


        [TestMethod]
        public void Renaming() {
            var tagToRename = new IntTag("DifferentName", 1);
            var compound = new CompoundTag {
                new IntTag("SameName", 1),
                tagToRename
            };

            // proper renaming, should not throw
            tagToRename.Name = "SomeOtherName";

            // attempting to use a duplicate name
            Assert.Throws<ArgumentException>(() => tagToRename.Name = "SameName");

            // assigning a null name to a tag inside a compound; should throw
            Assert.Throws<ArgumentNullException>(() => tagToRename.Name = null);

            // assigning a null name to a tag that's been removed; should not throw
            compound.Remove(tagToRename);
            tagToRename.Name = null;
        }


        [TestMethod]
        public void AddingAndRemoving() {
            var foo = new IntTag("Foo");
            var test = new CompoundTag {
                foo
            };

            // adding duplicate object
            Assert.Throws<ArgumentException>(() => test.Add(foo));

            // adding duplicate name
            Assert.Throws<ArgumentException>(() => test.Add(new ByteTag("Foo")));

            // adding unnamed tag
            Assert.Throws<ArgumentException>(() => test.Add(new IntTag()));

            // adding null
            Assert.Throws<ArgumentNullException>(() => test.Add(null));

            // adding tag to self
            Assert.Throws<ArgumentException>(() => test.Add(test));

            // contains existing name/object
            Assert.IsTrue(test.Contains("Foo"));
            Assert.IsTrue(test.Contains(foo));
            Assert.Throws<ArgumentNullException>(() => test.Contains((string)null));
            Assert.Throws<ArgumentNullException>(() => test.Contains((Tag)null));

            // contains non-existent name
            Assert.IsFalse(test.Contains("Bar"));

            // contains existing name / different object
            Assert.IsFalse(test.Contains(new IntTag("Foo")));

            // removing non-existent name
            Assert.Throws<ArgumentNullException>(() => test.Remove((string)null));
            Assert.IsFalse(test.Remove("Bar"));

            // removing existing name
            Assert.IsTrue(test.Remove("Foo"));

            // removing non-existent name
            Assert.IsFalse(test.Remove("Foo"));

            // re-adding object
            test.Add(foo);

            // removing existing object
            Assert.Throws<ArgumentNullException>(() => test.Remove((Tag)null));
            Assert.IsTrue(test.Remove(foo));
            Assert.IsFalse(test.Remove(foo));

            // clearing an empty CompoundTag
            Assert.AreEqual(0, test.Count);
            test.Clear();

            // re-adding after clearing
            test.Add(foo);
            Assert.AreEqual(1, test.Count);

            // clearing a non-empty CompoundTag
            test.Clear();
            Assert.AreEqual(0, test.Count);
        }


        [TestMethod]
        public void UtilityMethods() {
            Tag[] testThings = {
                new ShortTag("Name1", 1),
                new IntTag("Name2", 2),
                new LongTag("Name3", 3)
            };
            var compound = new CompoundTag();

            // add range
            compound.AddRange(testThings);

            // add range with duplicates
            Assert.Throws<ArgumentException>(() => compound.AddRange(testThings));
        }


        [TestMethod]
        public void InterfaceImplementations() {
            Tag[] tagList = {
                new ByteTag("First", 1), new ShortTag("Second", 2), new IntTag("Third", 3),
                new LongTag("Fourth", 4L)
            };

            // test CompoundTag(IEnumerable<Tag>) constructor
            var comp = new CompoundTag(tagList);

            // test .Names and .Tags collections
            CollectionAssert.AreEquivalent(new[] {
                "First", "Second", "Third", "Fourth"
            }, comp.Names.ToList());
            CollectionAssert.AreEquivalent(tagList, comp.Tags.ToList());

            // test ICollection and ICollection<Tag> boilerplate properties
            ICollection<Tag> iGenCollection = comp;
            Assert.IsFalse(iGenCollection.IsReadOnly);
            ICollection iCollection = comp;
            Assert.IsNotNull(iCollection.SyncRoot);
            Assert.IsFalse(iCollection.IsSynchronized);

            // test CopyTo()
            var tags = new Tag[iCollection.Count];
            iCollection.CopyTo(tags, 0);
            CollectionAssert.AreEquivalent(comp, tags);

            // test non-generic GetEnumerator()
            var enumeratedTags = comp.ToList();
            CollectionAssert.AreEquivalent(tagList, enumeratedTags);

            // test generic GetEnumerator()
            List<Tag> enumeratedTags2 = new List<Tag>();
            var enumerator = comp.GetEnumerator();
            while (enumerator.MoveNext()) {
                enumeratedTags2.Add(enumerator.Current);
            }
            CollectionAssert.AreEquivalent(tagList, enumeratedTags2);
        }
    }
}


