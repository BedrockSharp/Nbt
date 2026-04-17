using System;
using System.Globalization;
using System.Text;

namespace Nbt {
    /// <summary> A tag containing a single-precision floating point number. </summary>
    public sealed class FloatTag : Tag {
        /// <summary> Type of this tag (Float). </summary>
        public override TagType TagType {
            get { return TagType.Float; }
        }

        /// <summary> Value/payload of this tag (a single-precision floating point number). </summary>
        public float Value { get; set; }


        /// <summary> Creates an unnamed FloatTag tag with the default value of 0f. </summary>
        public FloatTag() { }


        /// <summary> Creates an unnamed FloatTag tag with the given value. </summary>
        /// <param name="value"> Value to assign to this tag. </param>
        public FloatTag(float value)
            : this(null, value) { }


        /// <summary> Creates an FloatTag tag with the given name and the default value of 0f. </summary>
        /// <param name="tagName"> Name to assign to this tag. May be <c>null</c>. </param>
        public FloatTag(string? tagName)
            : this(tagName, 0) { }


        /// <summary> Creates an FloatTag tag with the given name and value. </summary>
        /// <param name="tagName"> Name to assign to this tag. May be <c>null</c>. </param>
        /// <param name="value"> Value to assign to this tag. </param>
        public FloatTag(string? tagName, float value) {
            name = tagName;
            Value = value;
        }


        /// <summary> Creates a copy of given FloatTag tag. </summary>
        /// <param name="other"> Tag to copy. May not be <c>null</c>. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="other"/> is <c>null</c>. </exception>
        public FloatTag(FloatTag other) {
            if (other == null) throw new ArgumentNullException(nameof(other));
            name = other.name;
            Value = other.Value;
        }


        internal override bool ReadTag(NbtBinaryReader readStream) {
            if (readStream.Selector != null && !readStream.Selector(this)) {
                readStream.ReadSingle();
                return false;
            }
            Value = readSingle(readStream);
            return true;
        }

        private float readSingle(NbtBinaryReader readStream) {
            return readStream.ReadSingle();
        }


        internal override void SkipTag(NbtBinaryReader readStream) {
            readStream.ReadSingle();
        }


        public override void WriteTag(NbtBinaryWriter writeStream) {
            writeStream.Write(TagType.Float);
            if (Name == null) throw new NbtFormatException("Name is null");
            writeStream.Write(Name);
            writeStream.Write(Value);
        }


        internal override void WriteData(NbtBinaryWriter writeStream) {
            writeStream.Write(Value);
        }


        /// <inheritdoc />
        public override object Clone() {
            return new FloatTag(this);
        }


        internal override void PrettyPrint(StringBuilder sb, string indentString, int indentLevel) {
            for (int i = 0; i < indentLevel; i++) {
                sb.Append(indentString);
            }
            sb.Append("TAG_Float");
            if (!String.IsNullOrEmpty(Name)) {
                sb.AppendFormat(CultureInfo.InvariantCulture, "(\"{0}\")", Name);
            }
            sb.Append(": ");
            sb.Append(Value);
        }
    }
}
