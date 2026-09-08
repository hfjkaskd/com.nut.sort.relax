using System;
using Newtonsoft.Json.Linq;

namespace NutSort.Content
{
    public sealed class OriginalPlayerGoldHintSchedule
    {
        private readonly Func<JObject> configuration;
        private readonly Func<long> seconds;
        private readonly Func<bool> hasPanel;
        private readonly Func<OriginalMarqueeItem> select;
        private readonly Action<OriginalMarqueeItem> present;
        private readonly Func<int,int,int> range;
        public long LastShowTime { get; private set; } = -1;

        public OriginalPlayerGoldHintSchedule(Func<JObject> configuration,Func<bool> hasPanel,
            Func<OriginalMarqueeItem> select,Action<OriginalMarqueeItem> present,
            Func<long> seconds=null,Func<int,int,int> range=null)
        {
            this.configuration=configuration ?? throw new ArgumentNullException(nameof(configuration));
            this.hasPanel=hasPanel ?? throw new ArgumentNullException(nameof(hasPanel));
            this.select=select ?? throw new ArgumentNullException(nameof(select));
            this.present=present ?? throw new ArgumentNullException(nameof(present));
            this.seconds=seconds ?? UtcSeconds;
            this.range=range ?? UnityEngine.Random.Range;
        }
        public static long UtcSeconds() => SecondsFromUtc(DateTime.UtcNow);
        public static long SecondsFromUtc(DateTime utc)
        {
            // TimeLSSUtil.Time truncates ticks to milliseconds, then TimeSeconds divides again.
            return (utc.Ticks-new DateTime(1970,1,1).Ticks)/10000/1000;
        }
        public void Init() { LastShowTime=-1; }
        public void Push() { LastShowTime=seconds(); }
        public void NextPush()
        {
            JObject document=configuration() ?? throw new NullReferenceException("ServerConfigData");
            JToken interval=null;
            foreach(JProperty field in document.Properties())
                if(string.Equals(field.Name,"LSSUPT",StringComparison.OrdinalIgnoreCase))interval=field.Value;
            long now=seconds();
            if(interval==null || interval.Type==JTokenType.Null)throw new NullReferenceException("LSSUPT");
            var values=(JArray)interval;
            LastShowTime=unchecked(now+range((int)values[0],(int)values[1]));
        }
        public void Update()
        {
            if(LastShowTime==-1)return;
            if(seconds()>=LastShowTime)Show();
        }
        public void Show()
        {
            if(hasPanel()) { LastShowTime=unchecked(LastShowTime+3);return; }
            OriginalMarqueeItem data=select();
            if(data==null) { LastShowTime=unchecked(LastShowTime+3);return; }
            NextPush();
            present(data);
        }
    }
}
