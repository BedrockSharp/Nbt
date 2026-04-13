using System;
using System.Globalization;
using System.Text;

namespace Nbt {
    /// <summary> Base class for different kinds of named binary tags. </summary>
    public abstract class Tag : ICloneable {
        /// <summary> Parent compound tag, either ListTag or CompoundTag, if any.
        /// May be <c>null</c> for detached tags. </summary>
        public Tag? Parent { get; internal set; }

        /// <summary> Type of this tag. </summary>
        public abstract TagType TagType { get; }

        /// <summary> Returns true if tags of this type have a value attached.
        /// All tags except Compound, List, and End have values. </summary>
        public bool HasValue {
            get {
                switch (TagType) {
                    case TagType.Compound:
                    case TagType.End:
                    case TagType.List:
                    case TagType.Unknown:
                        return false;
                    default:
                        return true;
                }
            }
        }

        /// <summary> Name of this tag. Immutable, and set by the constructor. May be <c>null</c>. </summary>
        /// <exception cref="ArgumentNullException"> If <paramref name="value"/> is <c>null</c>, and <c>Parent</c> tag is an CompoundTag.
        /// Name of tags inside an <c>CompoundTag</c> may not be null. </exception>
        /// <exception cref="ArgumentException"> If this tag resides in an <c>CompoundTag</c>, and a sibling tag with the name already exists. </exception>
        public string? Name {
            get { return name; }
            set {
                if (name == value) {
                    return;
                }

                if (Parent is CompoundTag parentAsCompound) {
                    if (value == null) {
                        throw new ArgumentNullException(nameof(value),
                                                        "Name of tags inside an CompoundTag may not be null.");
                    } else if (name != null) {
                        parentAsCompound.RenameTag(name, value);
                    }
                }

                name = value;
            }
        }

        // Used by impls to bypass setter checks (and avoid side effects) when initializing state
        internal string? name;

        /// <summary> Gets the full name of this tag, including all parent tag names, separated by dots. 
        /// Unnamed tags show up as empty strings. </summary>
        public string Path {
            get {
                if (Parent == null) {
                    return Name ?? "";
                }
                if (Parent is ListTag parentAsList) {
                    return parentAsList.Path + '[' + parentAsList.IndexOf(this) + ']';
                } else {
                    return Parent.Path + '.' + Name;
                }
            }
        }

        internal abstract bool ReadTag(NbtBinaryReader readStream);

        internal abstract void SkipTag(NbtBinaryReader readStream);

        internal abstract void WriteTag(NbtBinaryWriter writeReader);

        // WriteData does not write the tag's ID byte or the name
        internal abstract void WriteData(NbtBinaryWriter writeStream);


        #region Shortcuts
#pragma warning disable CA1065 // Do not raise exceptions in unexpected locations

        /// <summary> Gets or sets the tag with the specified name. May return <c>null</c>. </summary>
        /// <returns> The tag with the specified key. Null if tag with the given name was not found. </returns>
        /// <param name="tagName"> The name of the tag to get or set. Must match tag's actual name. </param>
        /// <exception cref="InvalidOperationException"> If used on a tag that is not CompoundTag. </exception>
        /// <remarks> ONLY APPLICABLE TO CompoundTag OBJECTS!
        /// Included in Tag base class for programmers' convenience, to avoid extra type casts. </remarks>
        public virtual Tag? this[string tagName] {
            get { throw new InvalidOperationException("String indexers only work on CompoundTag tags."); }
            set { throw new InvalidOperationException("String indexers only work on CompoundTag tags."); }
        }

        /// <summary> Gets or sets the tag at the specified index. </summary>
        /// <returns> The tag at the specified index. </returns>
        /// <param name="tagIndex"> The zero-based index of the tag to get or set. </param>
        /// <exception cref="ArgumentOutOfRangeException"> tagIndex is not a valid index in this tag. </exception>
        /// <exception cref="ArgumentNullException"> Given tag is <c>null</c>. </exception>
        /// <exception cref="ArgumentException"> Given tag's type does not match ListType. </exception>
        /// <exception cref="InvalidOperationException"> If used on a tag that is not ListTag, ByteArrayTag, or IntArrayTag. </exception>
        /// <remarks> ONLY APPLICABLE TO ListTag, ByteArrayTag, and IntArrayTag OBJECTS!
        /// Included in Tag base class for programmers' convenience, to avoid extra type casts. </remarks>
        public virtual Tag this[int tagIndex] {
            get { throw new InvalidOperationException("Integer indexers only work on ListTag tags."); }
            set { throw new InvalidOperationException("Integer indexers only work on ListTag tags."); }
        }

        /// <summary> Returns the value of this tag, cast as a byte.
        /// Only supported by ByteTag tags. </summary>
        /// <exception cref="InvalidCastException"> When used on a tag other than ByteTag. </exception>
        public byte ByteValue {
            get {
                if (TagType == TagType.Byte) {
                    return ((ByteTag)this).Value;
                } else {
                    throw new InvalidCastException("Cannot get ByteValue from " + GetCanonicalTagName(TagType));
                }
            }
        }

        /// <summary> Returns the value of this tag, cast as a short (16-bit signed integer).
        /// Only supported by ByteTag and ShortTag. </summary>
        /// <exception cref="InvalidCastException"> When used on an unsupported tag. </exception>
        public short ShortValue {
            get {
                switch (TagType) {
                    case TagType.Byte:
                        return ((ByteTag)this).Value;
                    case TagType.Short:
                        return ((ShortTag)this).Value;
                    default:
                        throw new InvalidCastException("Cannot get ShortValue from " + GetCanonicalTagName(TagType));
                }
            }
        }

        /// <summary> Returns the value of this tag, cast as an int (32-bit signed integer).
        /// Only supported by ByteTag, ShortTag, and IntTag. </summary>
        /// <exception cref="InvalidCastException"> When used on an unsupported tag. </exception>
        public int IntValue {
            get {
                switch (TagType) {
                    case TagType.Byte:
                        return ((ByteTag)this).Value;
                    case TagType.Short:
                        return ((ShortTag)this).Value;
                    case TagType.Int:
                        return ((IntTag)this).Value;
                    default:
                        throw new InvalidCastException("Cannot get IntValue from " + GetCanonicalTagName(TagType));
                }
            }
        }

        /// <summary> Returns the value of this tag, cast as a long (64-bit signed integer).
        /// Only supported by ByteTag, ShortTag, IntTag, and LongTag. </summary>
        /// <exception cref="InvalidCastException"> When used on an unsupported tag. </exception>
        public long LongValue {
            get {
                switch (TagType) {
                    case TagType.Byte:
                        return ((ByteTag)this).Value;
                    case TagType.Short:
                        return ((ShortTag)this).Value;
                    case TagType.Int:
                        return ((IntTag)this).Value;
                    case TagType.Long:
                        return ((LongTag)this).Value;
                    default:
                        throw new InvalidCastException("Cannot get LongValue from " + GetCanonicalTagName(TagType));
                }
            }
        }

        /// <summary> Returns the value of this tag, cast as a long (64-bit signed integer).
        /// Only supported by FloatTag and, with loss of precision, by DoubleTag, ByteTag, ShortTag, IntTag, and LongTag. </summary>
        /// <exception cref="InvalidCastException"> When used on an unsupported tag. </exception>
        public float FloatValue {
            get {
                switch (TagType) {
                    case TagType.Byte:
                        return ((ByteTag)this).Value;
                    case TagType.Short:
                        return ((ShortTag)this).Value;
                    case TagType.Int:
                        return ((IntTag)this).Value;
                    case TagType.Long:
                        return ((LongTag)this).Value;
                    case TagType.Float:
                        return ((FloatTag)this).Value;
                    case TagType.Double:
                        return (float)((DoubleTag)this).Value;
                    default:
                        throw new InvalidCastException("Cannot get FloatValue from " + GetCanonicalTagName(TagType));
                }
            }
        }

        /// <summary> Returns the value of this tag, cast as a long (64-bit signed integer).
        /// Only supported by FloatTag, DoubleTag, and, with loss of precision, by ByteTag, ShortTag, IntTag, and LongTag. </summary>
        /// <exception cref="InvalidCastException"> When used on an unsupported tag. </exception>
        public double DoubleValue {
            get {
                switch (TagType) {
                    case TagType.Byte:
                        return ((ByteTag)this).Value;
                    case TagType.Short:
                        return ((ShortTag)this).Value;
                    case TagType.Int:
                        return ((IntTag)this).Value;
                    case TagType.Long:
                        return ((LongTag)this).Value;
                    case TagType.Float:
                        return ((FloatTag)this).Value;
                    case TagType.Double:
                        return ((DoubleTag)this).Value;
                    default:
                        throw new InvalidCastException("Cannot get DoubleValue from " + GetCanonicalTagName(TagType));
                }
            }
        }

        /// <summary> Returns the value of this tag, cast as a byte array.
        /// Only supported by ByteArrayTag tags. </summary>
        /// <exception cref="InvalidCastException"> When used on a tag other than ByteArrayTag. </exception>
        public byte[] ByteArrayValue {
            get {
                if (TagType == TagType.ByteArray) {
                    return ((ByteArrayTag)this).Value;
                } else {
                    throw new InvalidCastException("Cannot get ByteArrayValue from " + GetCanonicalTagName(TagType));
                }
            }
        }

        /// <summary> Returns the value of this tag, cast as an int array.
        /// Only supported by IntArrayTag tags. </summary>
        /// <exception cref="InvalidCastException"> When used on a tag other than IntArrayTag. </exception>
        public int[] IntArrayValue {
            get {
                if (TagType == TagType.IntArray) {
                    return ((IntArrayTag)this).Value;
                } else {
                    throw new InvalidCastException("Cannot get IntArrayValue from " + GetCanonicalTagName(TagType));
                }
            }
        }

        /// <summary> Returns the value of this tag, cast as a long array.
        /// Only supported by LongArrayTag tags. </summary>
        /// <exception cref="InvalidCastException"> When used on a tag other than LongArrayTag. </exception>
        public long[] LongArrayValue {
            get {
                if (TagType == TagType.LongArray) {
                    return ((LongArrayTag)this).Value;
                } else {
                    throw new InvalidCastException("Cannot get LongArrayValue from " + GetCanonicalTagName(TagType));
                }
            }
        }

        /// <summary> Returns the value of this tag, cast as a string.
        /// Returns exact value for StringTag, and stringified (using InvariantCulture) value for ByteTag, DoubleTag, FloatTag, IntTag, LongTag, and ShortTag.
        /// Not supported by CompoundTag, ListTag, ByteArrayTag, IntArrayTag, or LongArrayTag. </summary>
        /// <exception cref="InvalidCastException"> When used on an unsupported tag. </exception>
        public string StringValue {
            get {
                switch (TagType) {
                    case TagType.String:
                        return ((StringTag)this).Value;
                    case TagType.Byte:
                        return ((ByteTag)this).Value.ToString(CultureInfo.InvariantCulture);
                    case TagType.Double:
                        return ((DoubleTag)this).Value.ToString(CultureInfo.InvariantCulture);
                    case TagType.Float:
                        return ((FloatTag)this).Value.ToString(CultureInfo.InvariantCulture);
                    case TagType.Int:
                        return ((IntTag)this).Value.ToString(CultureInfo.InvariantCulture);
                    case TagType.Long:
                        return ((LongTag)this).Value.ToString(CultureInfo.InvariantCulture);
                    case TagType.Short:
                        return ((ShortTag)this).Value.ToString(CultureInfo.InvariantCulture);
                    default:
                        throw new InvalidCastException("Cannot get StringValue from " + GetCanonicalTagName(TagType));
                }
            }
        }
#pragma warning restore CA1065 // Do not raise exceptions in unexpected locations
        #endregion


        /// <summary> Returns a canonical (Notchy) name for the given TagType,
        /// e.g. "TAG_Byte_Array" for TagType.ByteArray </summary>
        /// <param name="type"> TagType to name. </param>
        /// <returns> String representing the canonical name of a tag,
        /// or null of given TagType does not have a canonical name (e.g. Unknown). </returns>
        public static string? GetCanonicalTagName(TagType type) {
            return type switch {
                TagType.Byte => "TAG_Byte",
                TagType.ByteArray => "TAG_Byte_Array",
                TagType.Compound => "TAG_Compound",
                TagType.Double => "TAG_Double",
                TagType.End => "TAG_End",
                TagType.Float => "TAG_Float",
                TagType.Int => "TAG_Int",
                TagType.IntArray => "TAG_Int_Array",
                TagType.LongArray => "TAG_Long_Array",
                TagType.List => "TAG_List",
                TagType.Long => "TAG_Long",
                TagType.Short => "TAG_Short",
                TagType.String => "TAG_String",
                _ => null,
            };
        }


        /// <summary> Prints contents of this tag, and any child tags, to a string.
        /// Indents the string using multiples of the given indentation string. </summary>
        /// <returns> A string representing contents of this tag, and all child tags (if any). </returns>
        public override string ToString() {
            return ToString(DefaultIndentString);
        }


        /// <summary> Creates a deep copy of this tag. </summary>
        /// <returns> A new Tag object that is a deep copy of this instance. </returns>
        public abstract object Clone();


        /// <summary> Prints contents of this tag, and any child tags, to a string.
        /// Indents the string using multiples of the given indentation string. </summary>
        /// <param name="indentString"> String to be used for indentation. </param>
        /// <returns> A string representing contents of this tag, and all child tags (if any). </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="indentString"/> is <c>null</c>. </exception>
        public string ToString(string indentString) {
            if (indentString == null) throw new ArgumentNullException(nameof(indentString));
            var sb = new StringBuilder();
            PrettyPrint(sb, indentString, 0);
            return sb.ToString();
        }


        internal abstract void PrettyPrint(StringBuilder sb, string indentString, int indentLevel);

        /// <summary> String to use for indentation in Tag's and NbtFile's ToString() methods by default. </summary>
        /// <exception cref="ArgumentNullException"> <paramref name="value"/> is <c>null</c>. </exception>
        public static string DefaultIndentString {
            get { return defaultIndentString; }
            set {
                if (value == null) throw new ArgumentNullException(nameof(value));
                defaultIndentString = value;
            }
        }

        static string defaultIndentString = "  ";
    }
}


