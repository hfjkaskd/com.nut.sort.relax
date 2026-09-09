using System;
using System.Globalization;
namespace NutSort.UI
{
    // TimeLSSUtil.LocalTimeSeconds and UILSSUtil.TimeFormat, used by withdrawal steps.
    public sealed class OriginalWithdrawalTime
    {
        private readonly Func<string> countryLanguageCode;
        public OriginalWithdrawalTime(Func<string> countryLanguageCode){this.countryLanguageCode=countryLanguageCode;}
        public static long LocalSeconds()=>SecondsFromLocal(DateTime.Now);
        public static long SecondsFromLocal(DateTime local)
        {
            // Source compares raw wall-clock ticks against an unspecified-kind epoch.
            return (local.Ticks-new DateTime(1970,1,1).Ticks)/10000/1000;
        }
        public static string CountryLanguageCode(string language,string country)=>string.Concat(language,"-",country);
        public string Format(long seconds,string format)
        {
            DateTime date=DateTimeOffset.FromUnixTimeSeconds(seconds).DateTime;
            // Resolve the live country only after the timestamp conversion succeeds.
            var culture=new CultureInfo(countryLanguageCode());
            return date.ToString(format,culture);
        }
    }
}
