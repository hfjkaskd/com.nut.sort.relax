using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NutSort.Gameplay
{
    // Explicit field access is stable under Obfuz. No JsonConvert object
    // contracts, enum names, reflection, or Unity object references are used.
    public static class OriginalBoardSnapshotJson
    {
        public static OriginalBoardSnapshot Read(string json)
        {
            using (var input = new StringReader(json ?? string.Empty))
            using (var reader = new JsonTextReader(input) { DateParseHandling = DateParseHandling.None })
            {
                if (!Next(reader)) return null;
                OriginalBoardSnapshot result = ReadOriginalBoardSnapshot(reader);
                if (Next(reader)) throw new JsonSerializationException("Trailing board snapshot content.");
                return result;
            }
        }

        public static string Write(OriginalBoardSnapshot snapshot)
        {
            using (var output = new StringWriter(CultureInfo.InvariantCulture))
            using (var writer = new JsonTextWriter(output))
            {
                WriteOriginalBoardSnapshot(writer, snapshot);
                writer.Flush();
                return output.ToString();
            }
        }

        private static bool Next(JsonReader reader)
        {
            while (reader.Read()) if (reader.TokenType != JsonToken.Comment) return true;
            return false;
        }

        private static void Fields(JsonReader reader, Action<string, JsonReader> read)
        {
            if (reader.TokenType != JsonToken.StartObject) throw new JsonSerializationException("Expected snapshot object.");
            while (Next(reader))
            {
                if (reader.TokenType == JsonToken.EndObject) return;
                if (reader.TokenType != JsonToken.PropertyName) throw new JsonSerializationException("Expected snapshot property.");
                string name = (string)reader.Value;
                if (!Next(reader)) break;
                read(name.ToUpperInvariant(), reader);
            }
            throw new JsonSerializationException("Unterminated snapshot object.");
        }

        private static List<T> ReadList<T>(JsonReader reader, Func<JsonReader, T> read)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            if (reader.TokenType != JsonToken.StartArray) throw new JsonSerializationException("Expected snapshot array.");
            var list = new List<T>();
            while (Next(reader))
            {
                if (reader.TokenType == JsonToken.EndArray) return list;
                list.Add(read(reader));
            }
            throw new JsonSerializationException("Unterminated snapshot array.");
        }

        private static void WriteList<T>(JsonWriter writer, List<T> list, Action<JsonWriter, T> write)
        {
            if (list == null) { writer.WriteNull(); return; }
            writer.WriteStartArray();
            for (int i = 0; i < list.Count; i++) write(writer, list[i]);
            writer.WriteEndArray();
        }

        private static OriginalBoardSnapshot ReadOriginalBoardSnapshot(JsonReader reader)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = new OriginalBoardSnapshot();
            Fields(reader, (key, r) =>
            {
                switch (key)
                {
                    case "SCREWINFOS": value.ScrewInfos = ReadList(r, ReadSavedScrew); break;
                    case "OPERATORINFOS": value.OperatorInfos = ReadList(r, ReadOriginalMoveRecord); break;
                    default: r.Skip(); break;
                }
            });
            return value;
        }

        private static void WriteOriginalBoardSnapshot(JsonWriter writer, OriginalBoardSnapshot value)
        {
            if (value == null) { writer.WriteNull(); return; }
            writer.WriteStartObject();
            writer.WritePropertyName("ScrewInfos"); WriteList(writer, value.ScrewInfos, WriteSavedScrew);
            writer.WritePropertyName("OperatorInfos"); WriteList(writer, value.OperatorInfos, WriteOriginalMoveRecord);
            writer.WriteEndObject();
        }

        private static SavedScrew ReadSavedScrew(JsonReader reader)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = new SavedScrew();
            Fields(reader, (key, r) =>
            {
                switch (key)
                {
                    case "ID": value.Id = (int)JToken.ReadFrom(r); break;
                    case "INDEX": value.Index = (int)JToken.ReadFrom(r); break;
                    case "NUTMAXCOUNT": value.NutMaxCount = (int)JToken.ReadFrom(r); break;
                    case "COORDINATE": value.Coordinate = ReadSavedCoordinate(r); break;
                    case "NUTINFOS": value.NutInfos = ReadList(r, ReadSavedNut); break;
                    case "SCREWMASKDATAS": value.ScrewMaskDatas = ReadList(r, ReadSavedMask); break;
                    case "ISLOCKED": value.IsLocked = (bool)JToken.ReadFrom(r); break;
                    default: r.Skip(); break;
                }
            });
            return value;
        }

        private static void WriteSavedScrew(JsonWriter writer, SavedScrew value)
        {
            if (value == null) { writer.WriteNull(); return; }
            writer.WriteStartObject();
            writer.WritePropertyName("Id"); writer.WriteValue(value.Id);
            writer.WritePropertyName("Index"); writer.WriteValue(value.Index);
            writer.WritePropertyName("NutMaxCount"); writer.WriteValue(value.NutMaxCount);
            writer.WritePropertyName("Coordinate"); WriteSavedCoordinate(writer, value.Coordinate);
            writer.WritePropertyName("NutInfos"); WriteList(writer, value.NutInfos, WriteSavedNut);
            writer.WritePropertyName("ScrewMaskDatas"); WriteList(writer, value.ScrewMaskDatas, WriteSavedMask);
            writer.WritePropertyName("IsLocked"); writer.WriteValue(value.IsLocked);
            writer.WriteEndObject();
        }

        private static SavedCoordinate ReadSavedCoordinate(JsonReader reader)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = new SavedCoordinate();
            Fields(reader, (key, r) =>
            {
                switch (key)
                {
                    case "X": value.x = (int)JToken.ReadFrom(r); break;
                    case "Y": value.y = (int)JToken.ReadFrom(r); break;
                    default: r.Skip(); break;
                }
            });
            return value;
        }

        private static void WriteSavedCoordinate(JsonWriter writer, SavedCoordinate value)
        {
            if (value == null) { writer.WriteNull(); return; }
            writer.WriteStartObject();
            writer.WritePropertyName("x"); writer.WriteValue(value.x);
            writer.WritePropertyName("y"); writer.WriteValue(value.y);
            writer.WriteEndObject();
        }

        private static SavedNut ReadSavedNut(JsonReader reader)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = new SavedNut();
            Fields(reader, (key, r) =>
            {
                switch (key)
                {
                    case "NUTPOS": value.NutPos = ReadSavedNutPosition(r); break;
                    case "NUTDATA": value.NutData = ReadSavedNutData(r); break;
                    default: r.Skip(); break;
                }
            });
            return value;
        }

        private static void WriteSavedNut(JsonWriter writer, SavedNut value)
        {
            if (value == null) { writer.WriteNull(); return; }
            writer.WriteStartObject();
            writer.WritePropertyName("NutPos"); WriteSavedNutPosition(writer, value.NutPos);
            writer.WritePropertyName("NutData"); WriteSavedNutData(writer, value.NutData);
            writer.WriteEndObject();
        }

        private static SavedNutPosition ReadSavedNutPosition(JsonReader reader)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = new SavedNutPosition();
            Fields(reader, (key, r) =>
            {
                switch (key)
                {
                    case "X": value.x = (int)JToken.ReadFrom(r); break;
                    case "Y": value.y = (int)JToken.ReadFrom(r); break;
                    case "Z": value.z = (int)JToken.ReadFrom(r); break;
                    default: r.Skip(); break;
                }
            });
            return value;
        }

        private static void WriteSavedNutPosition(JsonWriter writer, SavedNutPosition value)
        {
            if (value == null) { writer.WriteNull(); return; }
            writer.WriteStartObject();
            writer.WritePropertyName("x"); writer.WriteValue(value.x);
            writer.WritePropertyName("y"); writer.WriteValue(value.y);
            writer.WritePropertyName("z"); writer.WriteValue(value.z);
            writer.WriteEndObject();
        }

        private static SavedNutData ReadSavedNutData(JsonReader reader)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = new SavedNutData();
            Fields(reader, (key, r) =>
            {
                switch (key)
                {
                    case "NUTTYPE": value.NutType = (int)JToken.ReadFrom(r); break;
                    case "NUTCOLOR": value.NutColor = (int)JToken.ReadFrom(r); break;
                    case "V": value.V = (bool)JToken.ReadFrom(r); break;
                    default: r.Skip(); break;
                }
            });
            return value;
        }

        private static void WriteSavedNutData(JsonWriter writer, SavedNutData value)
        {
            if (value == null) { writer.WriteNull(); return; }
            writer.WriteStartObject();
            writer.WritePropertyName("NutType"); writer.WriteValue(value.NutType);
            writer.WritePropertyName("NutColor"); writer.WriteValue(value.NutColor);
            writer.WritePropertyName("V"); writer.WriteValue(value.V);
            writer.WriteEndObject();
        }

        private static SavedMask ReadSavedMask(JsonReader reader)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = new SavedMask();
            Fields(reader, (key, r) =>
            {
                switch (key)
                {
                    case "SCREWTYPE": value.ScrewType = (int)JToken.ReadFrom(r); break;
                    case "NUTCOLOR": value.NutColor = (int)JToken.ReadFrom(r); break;
                    case "OBJ": value.Obj = ReadSavedMaskObject(r); break;
                    case "ISSHOW": value.IsShow = (bool)JToken.ReadFrom(r); break;
                    default: r.Skip(); break;
                }
            });
            return value;
        }

        private static void WriteSavedMask(JsonWriter writer, SavedMask value)
        {
            if (value == null) { writer.WriteNull(); return; }
            writer.WriteStartObject();
            writer.WritePropertyName("ScrewType"); writer.WriteValue(value.ScrewType);
            writer.WritePropertyName("NutColor"); writer.WriteValue(value.NutColor);
            writer.WritePropertyName("Obj"); WriteSavedMaskObject(writer, value.Obj);
            writer.WritePropertyName("IsShow"); writer.WriteValue(value.IsShow);
            writer.WriteEndObject();
        }

        private static SavedMaskObject ReadSavedMaskObject(JsonReader reader)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = new SavedMaskObject();
            Fields(reader, (key, r) =>
            {
                switch (key)
                {
                    case "ID": value.Id = (int)JToken.ReadFrom(r); break;
                    case "TA": value.TA = (int)JToken.ReadFrom(r); break;
                    case "NUTCOLOR": value.NutColor = (int)JToken.ReadFrom(r); break;
                    case "ISSHOW": value.IsShow = (bool)JToken.ReadFrom(r); break;
                    default: r.Skip(); break;
                }
            });
            return value;
        }

        private static void WriteSavedMaskObject(JsonWriter writer, SavedMaskObject value)
        {
            if (value == null) { writer.WriteNull(); return; }
            writer.WriteStartObject();
            writer.WritePropertyName("Id"); writer.WriteValue(value.Id);
            writer.WritePropertyName("TA"); writer.WriteValue(value.TA);
            writer.WritePropertyName("NutColor"); writer.WriteValue(value.NutColor);
            writer.WritePropertyName("IsShow"); writer.WriteValue(value.IsShow);
            writer.WriteEndObject();
        }

        private static OriginalMoveRecord ReadOriginalMoveRecord(JsonReader reader)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = new OriginalMoveRecord();
            Fields(reader, (key, r) =>
            {
                switch (key)
                {
                    case "FROMSCREWINDEX": value.FromScrewIndex = (int)JToken.ReadFrom(r); break;
                    case "TOSCREWINDEX": value.ToScrewIndex = (int)JToken.ReadFrom(r); break;
                    case "NUTCOUNT": value.NutCount = (int)JToken.ReadFrom(r); break;
                    default: r.Skip(); break;
                }
            });
            return value;
        }

        private static void WriteOriginalMoveRecord(JsonWriter writer, OriginalMoveRecord value)
        {
            if (value == null) { writer.WriteNull(); return; }
            writer.WriteStartObject();
            writer.WritePropertyName("FromScrewIndex"); writer.WriteValue(value.FromScrewIndex);
            writer.WritePropertyName("ToScrewIndex"); writer.WriteValue(value.ToScrewIndex);
            writer.WritePropertyName("NutCount"); writer.WriteValue(value.NutCount);
            writer.WriteEndObject();
        }
    }
}
