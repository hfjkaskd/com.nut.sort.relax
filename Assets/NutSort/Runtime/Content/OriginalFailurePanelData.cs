using System;
using Newtonsoft.Json.Linq;

namespace NutSort.Content
{
    // FailPanel.RefreshLssPanel (0x9D6AD8). Presentation receives the original
    // server string verbatim, followed by visual-only revive gray state.
    public sealed class OriginalFailurePanelData
    {
        private readonly OriginalUserLocalData user;
        private readonly Action<string> setValue;
        private readonly Action<bool> setGray;
        private readonly int defaultMaximum;

        public OriginalFailurePanelData(OriginalUserLocalData user, Action<string> setValue,
            Action<bool> setGray, int defaultMaximum)
        {
            this.user = user ?? throw new ArgumentNullException(nameof(user));
            this.setValue = setValue ?? throw new ArgumentNullException(nameof(setValue));
            this.setGray = setGray ?? throw new ArgumentNullException(nameof(setGray));
            this.defaultMaximum = defaultMaximum;
        }

        public void Refresh()
        {
            JObject reward = user.GoldRewardTargetS2CData ?? throw new NullReferenceException("GoldRewardTargetS2CData");
            setValue((string)Field(reward, "bear_zs_show"));
            int used = user.CurrentLevelAddScrewCount;
            JObject config = user.ServerConfigData ?? throw new NullReferenceException("ServerConfigData");
            JToken maximum = Field(config, "LSSLSMAC");
            setGray(used >= (maximum == null ? defaultMaximum : (int)maximum));
        }

        private static JToken Field(JObject value, string name)
        {
            JToken result = null;
            foreach (JProperty field in value.Properties())
                if (string.Equals(field.Name, name, StringComparison.OrdinalIgnoreCase)) result = field.Value;
            return result;
        }
    }
}
