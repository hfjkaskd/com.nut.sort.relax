using System;
using UnityEngine;
namespace NutSort.Content
{
    [Serializable]
    public sealed class OriginalMarqueeResponse
    {
        public string message_status,kinetic_gap;
        public long Time;
        public OriginalMarqueeData kinetic_data;
        public bool IsSuccess=>kinetic_gap=="NO-0";
        public void Init(){if(IsSuccess)kinetic_data.Init();}
    }

    public sealed class OriginalMarqueeRequests
    {
        // PMDInfo shares this envelope across every request instance, even on failure.
        public static OriginalMarqueeResponse Cached;
        private readonly Action<string,Action<string,byte[]>> request;
        private readonly OriginalMarqueeSelector selector;
        public OriginalMarqueeRequests(Action<string,Action<string,byte[]>> request,Func<float,float,float> range=null)
        {
            this.request=request??throw new ArgumentNullException(nameof(request));
            selector=new OriginalMarqueeSelector(ReadSelectionData,()=>PMD(),range);
        }
        private static OriginalMarqueeData ReadSelectionData()
        {
            if(Cached==null)return null;
            return Cached.kinetic_data??throw new NullReferenceException();
        }
        public OriginalMarqueeItem Next()=>selector.Next();
        // RequestMgr.Init's PMD call; native SDK syncNotifyCode remains excluded.
        public void Initialize()=>PMD();
        public void PMD(Action<OriginalMarqueeItem> callback=null)
        {
            Request(value=>
            {
                // Preserve the source manager's cast, including its incompatible
                // successful envelope path when a non-null callback is supplied.
                if(value!=null&&callback!=null)callback((OriginalMarqueeItem)value);
            });
        }
        public void Request(Action<object> callback=null)
        {
            if(Cached!=null){callback?.Invoke(Cached);return;}
            request("-1",(json,bytes)=>Receive(json,callback));
        }
        private static void Receive(string json,Action<object> callback)
        {
            if(!string.IsNullOrEmpty(json))
            {
                Cached=JsonUtility.FromJson<OriginalMarqueeResponse>(json);
                if(Cached.IsSuccess)
                {
                    Cached.Init();
                    callback?.Invoke(Cached);
                    return;
                }
            }
            callback?.Invoke(null);
        }
    }
}
