using Microsoft.ServiceFabric.Actors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;

namespace AppCommon
{
    public class ActorIdSerializer : JsonConverter<ActorId>
    {
        public override ActorId? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString();
            return ConvertToActorId(value);
        }

        public override void Write(Utf8JsonWriter writer, ActorId value, JsonSerializerOptions options)
        {
             writer.WriteStringValue(ConvertToString(value));
        }

        public static ActorId? ConvertToActorId(String? value)
        {
            if (value == null)
            {
                return null;
            }
            string[] parts = value.Split('|');
            if (parts[0] == "StringKind")
            {
                return new ActorId(parts[1]);
            }
            else if (parts[0] == "LongKind")
            {
                return new ActorId(long.Parse(parts[1]));
            }
            else
            {
                throw new ArgumentException("Unsupported ActorId in this converted!");
            }
        }

        public static string? ConvertToString(ActorId? value)
        {
            if (value == null)
            {
                return null;
            }
            else if (value.Kind == ActorIdKind.String)
            {
                return "StringKind|" + value.GetStringId();
            }
            else if (value.Kind == ActorIdKind.Long)
            {
                return "LongKind|" + value.GetLongId().ToString();
            }
            else
            {
                throw new ArgumentException("Unsupported ActorId in this converted!");
            }
        }
    }
}
