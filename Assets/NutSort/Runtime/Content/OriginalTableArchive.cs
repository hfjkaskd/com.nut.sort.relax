using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;

namespace NutSort.Content
{
    // Reads the source AES -> GZip -> NRBF data container without resolving CLR
    // types, invoking constructors or using BinaryFormatter/reflection.
    public static class OriginalTableArchive
    {
        public static Dictionary<string, byte[]> Read(byte[] encrypted, OriginalTableSettings settings)
        {
            byte[] compressed;
            using (var derivation = new PasswordDeriveBytes(settings.Password, Convert.FromBase64String(settings.Salt)))
            using (var aes = Aes.Create())
            using (var decryptor = aes.CreateDecryptor(derivation.GetBytes(32), Convert.FromBase64String(settings.IV)))
                compressed = decryptor.TransformFinalBlock(encrypted, 0, encrypted.Length);
            using (var input = new MemoryStream(compressed, false))
            using (var gzip = new GZipStream(input, CompressionMode.Decompress))
            using (var output = new MemoryStream())
            {
                gzip.CopyTo(output);
                output.Position = 0;
                return new Reader(output).ReadTables();
            }
        }

        private sealed class Reader
        {
            private sealed class Schema
            {
                public string[] Names;
                public byte[] Types;
                public byte[] PrimitiveTypes;
            }
            private sealed class Reference { public int Id; }
            private readonly BinaryReader reader;
            private readonly Dictionary<int, object> objects = new Dictionary<int, object>();
            private readonly Dictionary<int, Schema> schemas = new Dictionary<int, Schema>();
            private bool ended;
            public Reader(Stream stream) { reader = new BinaryReader(stream, Encoding.UTF8, true); }
            public Dictionary<string, byte[]> ReadTables()
            {
                if (reader.ReadByte() != 0 || reader.ReadInt32() != 1 || reader.ReadInt32() != -1 ||
                    reader.ReadInt32() != 1 || reader.ReadInt32() != 0)
                    throw new InvalidDataException("Original table archive header differs.");
                while (!ended) ReadRecord();
                if (reader.BaseStream.Position != reader.BaseStream.Length) throw new InvalidDataException("Trailing table archive data.");
                var root = (Dictionary<string, object>)objects[1];
                var dictionary = (Dictionary<string, object>)Resolve(root["datas"]);
                var pairs = (object[])Resolve(dictionary["KeyValuePairs"]);
                var result = new Dictionary<string, byte[]>(StringComparer.Ordinal);
                foreach (var value in pairs)
                {
                    var pair = (Dictionary<string, object>)Resolve(value);
                    result.Add((string)Resolve(pair["key"]), (byte[])Resolve(pair["value"]));
                }
                return result;
            }
            private object Resolve(object value) => value is Reference reference ? objects[reference.Id] : value;
            private byte ReadTypeInfo(byte type)
            {
                switch (type)
                {
                    case 0: case 7: return reader.ReadByte();
                    case 3: reader.ReadString(); return 0;
                    case 4: reader.ReadString(); reader.ReadInt32(); return 0;
                    case 1: case 2: case 5: case 6: return 0;
                    default: throw new InvalidDataException("Unsupported table member type.");
                }
            }
            private object ReadValue(byte type, byte primitive)
            {
                if (type != 0) return ReadRecord();
                if (primitive == 8) return reader.ReadInt32();
                throw new InvalidDataException("Unsupported table primitive.");
            }
            private object ReadClass(int id, Schema schema)
            {
                var value = new Dictionary<string, object>(StringComparer.Ordinal);
                objects.Add(id, value);
                for (int i = 0; i < schema.Names.Length; i++)
                    value.Add(schema.Names[i], ReadValue(schema.Types[i], schema.PrimitiveTypes[i]));
                return value;
            }
            private object ReadRecord()
            {
                byte kind = reader.ReadByte();
                int id, count;
                switch (kind)
                {
                    case 12: reader.ReadInt32(); return reader.ReadString(); // Library name is data only.
                    case 4: case 5:
                        id = reader.ReadInt32(); reader.ReadString(); count = reader.ReadInt32();
                        var schema = new Schema { Names = new string[count], Types = new byte[count], PrimitiveTypes = new byte[count] };
                        for (int i = 0; i < count; i++) schema.Names[i] = reader.ReadString();
                        for (int i = 0; i < count; i++) schema.Types[i] = reader.ReadByte();
                        for (int i = 0; i < count; i++) schema.PrimitiveTypes[i] = ReadTypeInfo(schema.Types[i]);
                        if (kind == 5) reader.ReadInt32();
                        schemas.Add(id, schema);
                        return ReadClass(id, schema);
                    case 1:
                        id = reader.ReadInt32(); return ReadClass(id, schemas[reader.ReadInt32()]);
                    case 6:
                        id = reader.ReadInt32(); string text = reader.ReadString(); objects.Add(id, text); return text;
                    case 9: return new Reference { Id = reader.ReadInt32() };
                    case 7:
                        id = reader.ReadInt32();
                        if (reader.ReadByte() != 0 || reader.ReadInt32() != 1) throw new InvalidDataException("Unsupported table array shape.");
                        count = reader.ReadInt32(); byte type = reader.ReadByte(); byte primitive = ReadTypeInfo(type);
                        var array = new object[count]; objects.Add(id, array);
                        for (int i = 0; i < count; i++) array[i] = ReadValue(type, primitive);
                        return array;
                    case 15:
                        id = reader.ReadInt32(); count = reader.ReadInt32();
                        if (reader.ReadByte() != 2) throw new InvalidDataException("Expected table byte array.");
                        byte[] bytes = reader.ReadBytes(count);
                        if (bytes.Length != count) throw new EndOfStreamException("Truncated table payload.");
                        objects.Add(id, bytes); return bytes;
                    case 11: ended = true; return null;
                    default: throw new InvalidDataException("Unsupported table record: " + kind);
                }
            }
        }
    }
}
