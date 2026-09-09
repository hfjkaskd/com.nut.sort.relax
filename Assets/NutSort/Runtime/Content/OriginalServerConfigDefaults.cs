using Newtonsoft.Json.Linq;
using UnityEngine;
namespace NutSort.Content
{
    [CreateAssetMenu(menuName="Nut Sort/Original server config defaults")]
    public sealed class OriginalServerConfigDefaults:ScriptableObject
    {
        public int LSSLR1,LSSLR2,LSSLRFP,LSSLRFV,LSSLD1,LSSLD2;
        public int[] LSSLDR;
        public int LSSEDGC,LSSR1,LSSR2;
        public int[] LSSUPT;
        public bool LSSNTY;
        public int[] LSSGPUL;
        public bool LSS260820;
        public int LSSSHSLV,LSSLSMAC;
        public bool LSSAB;
        public string ksCountry;
        public JObject CreateDocument()=>new JObject
        {
            ["LSSLR1"]=LSSLR1,["LSSLR2"]=LSSLR2,["LSSLRFP"]=LSSLRFP,["LSSLRFV"]=LSSLRFV,
            ["LSSLD1"]=LSSLD1,["LSSLD2"]=LSSLD2,["LSSLDR"]=Array(LSSLDR),
            ["LSSEDGC"]=LSSEDGC,["LSSR1"]=LSSR1,["LSSR2"]=LSSR2,["LSSUPT"]=Array(LSSUPT),
            ["LSSNTY"]=LSSNTY,["LSSGPUL"]=Array(LSSGPUL),["LSS260820"]=LSS260820,
            ["LSSSHSLV"]=LSSSHSLV,["LSSLSMAC"]=LSSLSMAC,["LSSAB"]=LSSAB,["ksCountry"]=ksCountry==null?JValue.CreateNull():new JValue(ksCountry)
        };
        private static JToken Array(int[] values)
        {if(values==null)return JValue.CreateNull();var result=new JArray();foreach(int value in values)result.Add(value);return result;}
    }
}
