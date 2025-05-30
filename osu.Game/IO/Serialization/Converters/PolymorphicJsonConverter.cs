// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace osu.Game.IO.Serialization.Converters
{
    /// <summary>
    /// A type of <see cref="JsonConverter"/> that serializes an object alongside it's type name.
    /// The type name is used in deserialization to reconstruct the object with their original types.
    /// </summary>
    /// <typeparam name="T">The type of the property this attribute is attached to.</typeparam>
    public class PolymorphicJsonConverter<T> : JsonConverter<T>
    {
        private const string type_key = "$type";
        private const string value_key = "$value";

        public override T? ReadJson(JsonReader reader, Type objectType, T? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JObject obj = JObject.Load(reader);

            string? typeName = obj[type_key]?.Value<string>();
            var value = obj[value_key];

            if (typeName == null || value == null)
                return fallbackToDefaultSerializer(reader, serializer);

            var type = Type.GetType(typeName);
            if (type == null)
                return fallbackToDefaultSerializer(reader, serializer);

            if (!type.IsAssignableTo(objectType))
                return fallbackToDefaultSerializer(reader, serializer);

            var instance = (T)Activator.CreateInstance(type)!;
            serializer.Populate(value.CreateReader(), instance);

            return instance;
        }

        private static T? fallbackToDefaultSerializer(JsonReader reader, JsonSerializer serializer) => serializer.Deserialize<T>(reader);

        public override void WriteJson(JsonWriter writer, T? value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            var type = value?.GetType();
            var assemblyName = type!.Assembly.GetName();

            string typeString = $"{type.FullName}, {assemblyName.Name}";

            writer.WriteStartObject();

            writer.WritePropertyName(type_key);
            serializer.Serialize(writer, typeString);

            writer.WritePropertyName(value_key);
            serializer.Serialize(writer, value);

            writer.WriteEndObject();
        }
    }
}
