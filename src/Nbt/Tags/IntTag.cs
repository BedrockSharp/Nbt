using System;
using System.Globalization;
using System.Text;

namespace Nbt {
    /// <summary> A tag containing a single signed 32-bit integer. </summary>
    public sealed class IntTag : Tag {
        /// <summary> Type of this tag (Int). </summary>
        public override TagType TagType {
            get { return TagType.Int; }
        }

        /// <summary> Value/payload of this tag (a single signed 32-bit integer). </summary>
        public int Value { get; set; }


        /// <summary> Creates an unnamed IntTag tag with the default value of 0. </summary>
        public IntTag() { }


        /// <summary> Creates an unnamed IntTag tag with the given value. </summary>
        /// <param name="value"> Value to assign to this tag. </param>
        public IntTag(int value)
            : this(null, value) { }


        /// <summary> Creates an IntTag tag with the given name and the default value of 0. </summary>
        /// <param name="tagName"> Name to assign to this tag. May be <c>null</c>. </param>
        public IntTag(string? tagName)
            : this(tagName, 0) { }


        /// <summary> Creates an IntTag tag with the given name and value. </summary>
        /// <param name="tagName"> Name to assign to this tag. May be <c>null</c>. </param>
        /// <param name="value"> Value to assign to this tag. </param>
        public IntTag(string? tagName, int value) {
            name = tagName;
            Value = value;
        }


        /// <summary> Creates a copy of given IntTag tag. </summary>
        /// <param name="other"> Tag to copy. May not be <c>null</c>. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="other"/> is <c>null</c>. </exception>
        public IntTag(IntTag other) {
            if (other == null) throw new ArgumentNullException(nameof(other));
            name = other.name;
            Value = other.Value;
        }


        internal override bool ReadTag(NbtBinaryReader readStream) {
            if (readStream.Selector != null && !readStream.Selector(this)) {
                readStream.ReadInt32();
                return false;
            }
            Value = readStream.ReadInt32();
            return true;
        }


        internal override void SkipTag(NbtBinaryReader readStream) {
            readStream.ReadInt32();
        }


        public override void WriteTag(NbtBinaryWriter writeStream) {
            writeStream.Write(TagType.Int);
            if (Name == null) throw new NbtFormatException("Name is null");
            writeStream.Write(Name);
            writeStream.Write(Value);
        }


        internal override void WriteData(NbtBinaryWriter writeStream) {
            writeStream.Write(Value);
        }


        /// <inheritdoc />
        public override object Clone() {
            return new IntTag(this);
        }


        internal override void PrettyPrint(StringBuilder sb, string indentString, int indentLevel) {
            for (int i = 0; i < indentLevel; i++) {
                sb.Append(indentString);
            }
            sb.Append("TAG_Int");
            if (!String.IsNullOrEmpty(Name)) {
                sb.AppendFormat(CultureInfo.InvariantCulture, "(\"{0}\")", Name);
            }
            sb.Append(": ");
            sb.Append(Value);
        }
    }
}
