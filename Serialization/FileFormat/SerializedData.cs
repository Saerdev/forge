﻿// The MIT License (MIT)
//
// Copyright (c) 2013 Jacob Dufault
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and
// associated documentation files (the "Software"), to deal in the Software without restriction,
// including without limitation the rights to use, copy, modify, merge, publish, distribute,
// sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT
// NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

//#define ENABLE_IMPLICIT_CONVERSIONS

using Neon.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Neon.Serialization {
    public struct ObjectReference {
        public int Id;
        public Type Type;

        internal ObjectReference(int id, Type type) {
            Id = id;
            Type = type;
        }
    }

    /// <summary>
    /// A union type that stores a serialized value. The stored type can be one of six different
    /// types: null, boolean, Real, string, Dictionary, or List.
    /// </summary>
    public sealed class SerializedData {
        /// <summary>
        /// The raw value that this serialized data stores. It can be one of six different types; a
        /// boolean, a Real, a string, an object reference, a Dictionary, or a List.
        /// </summary>
        private object _value;

        /// <summary>
        /// If this data is an object reference definition, then this maybe will contain the id for
        /// the object reference we are a definition of.
        /// </summary>
        private Maybe<int> _objectDefinition;

        #region Constructors
        /// <summary>
        /// Creates a SerializedData instance that holds null.
        /// </summary>
        public SerializedData() {
            _value = null;
        }

        /// <summary>
        /// Creates a SerializedData instance that holds a boolean.
        /// </summary>
        public SerializedData(bool boolean) {
            _value = boolean;
        }

        /// <summary>
        /// Creates a SerializedData instance that holds a real.
        /// </summary>
        public SerializedData(Real real) {
            _value = real;
        }

        /// <summary>
        /// Creates a SerializedData instance that holds a string.
        /// </summary>
        public SerializedData(string str) {
            _value = str;
        }

        /// <summary>
        /// Creates a SerializedData instance that holds a dictionary of values.
        /// </summary>
        public SerializedData(Dictionary<string, SerializedData> dict) {
            _value = dict;
        }

        /// <summary>
        /// Creates a SerializedData instance that holds a list of values.
        /// </summary>
        public SerializedData(List<SerializedData> list) {
            _value = list;
        }

        /// <summary>
        /// Creates a new SerializedData instance that contains an object reference.
        /// </summary>
        /// <param name="objectId">The id of the object that this data references</param>
        public static SerializedData CreateObjectReference(int objectId, Type type) {
            SerializedData data = new SerializedData();
            data._value = new ObjectReference(objectId, type);
            return data;
        }

        /// <summary>
        /// Helper method to create a SerializedData instance that holds a dictionary.
        /// </summary>
        public static SerializedData CreateDictionary() {
            return new SerializedData(new Dictionary<string, SerializedData>());
        }

        /// <summary>
        /// Helper method to create a SerializedData instance that holds a list.
        /// </summary>
        public static SerializedData CreateList() {
            return new SerializedData(new List<SerializedData>());
        }

        /// <summary>
        /// Sets this SerializedData instance so that it acts as an object definition for the object
        /// reference with the given objectId.
        /// </summary>
        /// <param name="objectId">The object id to act as a definition for.</param>
        public void SetObjectDefinition(int objectId) {
            _objectDefinition = Maybe.Just(objectId);
        }
        #endregion

        #region Casting Predicates
        /// <summary>
        /// Returns true if this SerializedData instance maps back to null.
        /// </summary>
        public bool IsNull {
            get {
                return _value == null;
            }
        }

        /// <summary>
        /// Returns true if this SerializedData instance maps back to a Real.
        /// </summary>
        public bool IsReal {
            get {
                return _value is Real;
            }
        }

        /// <summary>
        /// Returns true if this SerializedData instance maps back to a boolean.
        /// </summary>
        public bool IsBool {
            get {
                return _value is bool;
            }
        }

        /// <summary>
        /// Returns true if this SerializedData instance maps back to a string.
        /// </summary>
        public bool IsString {
            get {
                return _value is string;
            }
        }

        /// <summary>
        /// Returns true if the SerializedData instance maps contains no actual data and merely is a
        /// reference to another object.
        /// </summary>
        public bool IsObjectReference {
            get {
                return _value is ObjectReference;
            }
        }

        /// <summary>
        /// Returns true if this serialized data acts as an instance that other data can reference
        /// for content.
        /// </summary>
        public bool IsObjectDefinition {
            get {
                return _objectDefinition.Exists;
            }
        }

        /// <summary>
        /// Returns true if this SerializedData instance maps back to a Dictionary.
        /// </summary>
        public bool IsDictionary {
            get {
                return _value is Dictionary<string, SerializedData>;
            }
        }

        /// <summary>
        /// Returns true if this SerializedData instance maps back to a List.
        /// </summary>
        public bool IsList {
            get {
                return _value is List<SerializedData>;
            }
        }
        #endregion

        #region Casts
        /// <summary>
        /// Casts this SerializedData to a Real. Throws an exception if it is not a Real.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public Real AsReal {
            get {
                return Cast<Real>();
            }
        }

        /// <summary>
        /// Casts this SerializedData to a boolean. Throws an exception if it is not a boolean.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public bool AsBool {
            get {
                return Cast<bool>();
            }
        }

        /// <summary>
        /// Casts this SerializedData to a string. Throws an exception if it is not a string.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public string AsString {
            get {
                return Cast<string>();
            }
        }

        /// <summary>
        /// Casts this SerializedData to an object reference. Throws an exception if is not an
        /// object reference.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public ObjectReference AsObjectReference {
            get {
                return Cast<ObjectReference>();
            }
        }

        /// <summary>
        /// Returns the object definition id that this serialized data instance maps to. Throws an
        /// exception if this data is not an object definition.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public int AsObjectDefinition {
            get {
                if (_objectDefinition.IsEmpty) {
                    throw new InvalidOperationException("The SerializedData instance is not an " +
                        "object definition (this=" + PrettyPrinted + ")");
                }

                return _objectDefinition.Value;
            }
        }

        /// <summary>
        /// Casts this SerializedData to a Dictionary. Throws an exception if it is not a
        /// Dictionary.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public Dictionary<string, SerializedData> AsDictionary {
            get {
                return Cast<Dictionary<string, SerializedData>>();
            }
        }

        /// <summary>
        /// Casts this SerializedData to a List. Throws an exception if it is not a List.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public List<SerializedData> AsList {
            get {
                return Cast<List<SerializedData>>();
            }
        }

        /// <summary>
        /// Internal helper method to cast the underlying storage to the given type or throw a
        /// pretty printed exception on failure.
        /// </summary>
        private T Cast<T>() {
            if (_value is T) {
                return (T)_value;
            }

            throw new InvalidCastException("Unable to cast <" + PrettyPrinted + "> (with type = " +
                _value.GetType() + ") to type " + typeof(T));
        }
        #endregion

#if ENABLE_IMPLICIT_CONVERSIONS
        #region Implicit Casts (if enabled)
        public static implicit operator SerializedData(bool boolean) {
            return new SerializedData(boolean);
        }

        public static implicit operator SerializedData(Real real) {
            return new SerializedData(real);
        }

        public static implicit operator SerializedData(string str) {
            return new SerializedData(str);
        }

        public static implicit operator SerializedData(List<SerializedData> list) {
            return new SerializedData(list);
        }

        public static implicit operator SerializedData(Dictionary<string, SerializedData> dict) {
            return new SerializedData(dict);
        }

        public SerializedData this[int index] {
            get {
                return AsList[index];
            }
            set {
                AsList[index] = value;
            }
        }

        public SerializedData this[string key] {
            get {
                return AsDictionary[key];
            }
            set {
                AsDictionary[key] = value;
            }
        }

        public static implicit operator Real(SerializedData value) {
            return value.Cast<Real>();
        }

        public static implicit operator string(SerializedData value) {
            return value.Cast<string>();
        }

        public static implicit operator bool(SerializedData value) {
            return value.Cast<bool>();
        }
        #endregion
#endif

        #region Pretty Printing
        /// <summary>
        /// Inserts the given number of indents into the builder.
        /// </summary>
        private void InsertSpacing(StringBuilder builder, int count) {
            for (int i = 0; i < count; ++i) {
                builder.Append("    ");
            }
        }

        /// <summary>
        /// Formats this data into the given builder.
        /// </summary>
        private void BuildString(StringBuilder builder, int depth) {
            if (IsObjectDefinition) {
                builder.Append("&d");
                builder.Append(AsObjectDefinition);
                builder.Append(' ');
            }

            if (IsNull) {
                builder.Append("null");
            }

            else if (IsBool) {
                if (AsBool) {
                    builder.Append("true");
                }
                else {
                    builder.Append("false");
                }
            }

            else if (IsReal) {
                // We can convert the real to a float and export it that way, because upon import
                // all computers will parse the same string the same way.
                builder.Append(AsReal.AsFloat);
            }

            else if (IsString) {
                // we don't support escaping
                builder.Append('"');
                builder.Append(AsString);
                builder.Append('"');
            }

            else if (IsObjectReference) {
                builder.Append("&r");
                builder.Append(AsObjectReference.Id);
                builder.Append('<');
                builder.Append(AsObjectReference.Type);
                builder.Append('>');
            }

            else if (IsDictionary) {
                builder.Append('{');
                builder.AppendLine();
                foreach (var entry in AsDictionary) {
                    InsertSpacing(builder, depth + 1);
                    builder.Append(entry.Key);
                    builder.Append(": ");
                    entry.Value.BuildString(builder, depth + 1);
                    builder.AppendLine();
                }
                InsertSpacing(builder, depth);
                builder.Append('}');
            }

            else if (IsList) {
                // special case for empty lists; we don't put an empty line between the brackets
                if (AsList.Count == 0) {
                    builder.Append("[]");
                }

                else {
                    builder.Append('[');
                    builder.AppendLine();
                    foreach (var entry in AsList) {
                        InsertSpacing(builder, depth + 1);
                        entry.BuildString(builder, depth + 1);
                        builder.AppendLine();
                    }
                    InsertSpacing(builder, depth);
                    builder.Append(']');
                }
            }

            else {
                throw new NotImplementedException("Unknown stored value type of " + _value);
            }
        }

        /// <summary>
        /// Returns this SerializedData in a pretty printed format.
        /// </summary>
        public string PrettyPrinted {
            get {
                StringBuilder builder = new StringBuilder();
                BuildString(builder, 0);
                return builder.ToString();
            }
        }
        #endregion

        #region Equality Comparisons
        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        public override bool Equals(Object obj) {
            if (obj == null) {
                return false;
            }

            SerializedData v = obj as SerializedData;
            if (v == null) {
                return false;
            }

            if (_value == null) {
                return v._value == null;
            }

            return _value.Equals(v._value);
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        public bool Equals(SerializedData v) {
            if (v == null) {
                return false;
            }

            if (_value == null) {
                return v._value == null;
            }

            return _value.Equals(v._value);
        }

        /// <summary>
        /// Returns true iff a == b.
        /// </summary>
        public static bool operator ==(SerializedData a, SerializedData b) {
            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(a, b)) {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null)) {
                return false;
            }

            return a.Equals(b);
        }

        /// <summary>
        /// Returns true iff a != b.
        /// </summary>
        public static bool operator !=(SerializedData a, SerializedData b) {
            return !(a == b);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data
        /// structures like a hash table.</returns>
        public override int GetHashCode() {
            return _value.GetHashCode();
        }
        #endregion
    }

}