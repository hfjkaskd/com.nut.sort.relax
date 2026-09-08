using Newtonsoft.Json.Linq;

namespace NutSort.Content
{
    // Explicit field access preserves the original JSON names under obfuscation.
    // No JsonSerializer, ToObject<T>, reflected member lookup, or enum string keys.
    public static class OriginalLevelJson
    {
        public static LevelDataConfig ReadIndex(string json)
        {
            JObject root = JObject.Parse(json);
            JArray entries = root["LevelDataInfos"] as JArray;
            if (entries == null) return new LevelDataConfig();
            var result = new LevelDataConfig { LevelDataInfos = new LevelDataInfo[entries.Count] };
            for (int i = 0; i < entries.Count; i++)
            {
                JToken entry = entries[i];
                JArray seeds = entry["Seeds"] as JArray;
                var data = new LevelDataInfo { Id = Number(entry, "Id") };
                if (seeds != null)
                {
                    data.Seeds = new string[seeds.Count];
                    for (int j = 0; j < seeds.Count; j++) data.Seeds[j] = (string)seeds[j];
                }
                result.LevelDataInfos[i] = data;
            }
            return result;
        }

        public static LevelData ReadBoard(string json)
        {
            JObject root = JObject.Parse(json);
            var result = new LevelData { UCCC = Number(root, "UCCC"), CC = Number(root, "CC"), LId = (string)root["LId"] };
            JArray screws = root["B"] as JArray;
            if (screws == null) return result;
            result.B = new ScrewData[screws.Count];
            for (int i = 0; i < screws.Count; i++) result.B[i] = ReadScrew(screws[i]);
            return result;
        }

        private static ScrewData ReadScrew(JToken token)
        {
            if (IsNull(token)) return null;
            var screw = new ScrewData { Id = Number(token, "Id") };
            JArray cells = token["C"] as JArray;
            if (cells != null)
            {
                screw.C = new CData[cells.Count];
                for (int i = 0; i < cells.Count; i++) screw.C[i] = ReadCell(cells[i]);
            }
            JArray outer = token["OBIM"] as JArray;
            if (outer != null)
            {
                screw.OBIM = new OBIMData[outer.Count];
                for (int i = 0; i < outer.Count; i++)
                {
                    JToken item = outer[i];
                    if (IsNull(item)) continue;
                    var data = new OBIMData { Id = Number(item, "Id"), CI = Number(item, "CI") };
                    JToken obj = item["Obj"];
                    if (!IsNull(obj)) data.Obj = new OBIMObjData { Id = Number(obj, "Id"), TA = Number(obj, "TA"), CI = Number(obj, "CI") };
                    screw.OBIM[i] = data;
                }
            }
            return screw;
        }

        private static CData ReadCell(JToken token)
        {
            if (IsNull(token)) return null;
            var result = new CData();
            JToken position = token["LP"];
            if (!IsNull(position)) result.LP = new LPData { x = Number(position, "x"), y = Number(position, "y"), z = Number(position, "z") };
            JToken nut = token["BIM"];
            if (!IsNull(nut)) result.BIM = new BIMData { Id = Number(nut, "Id"), CI = Number(nut, "CI"), V = (bool?)nut["V"] ?? false };
            return result;
        }

        private static int Number(JToken token, string name) => (int?)token[name] ?? 0;
        private static bool IsNull(JToken token) => token == null || token.Type == JTokenType.Null;
    }
}
