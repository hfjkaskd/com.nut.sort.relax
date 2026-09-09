using System;
namespace NutSort.Content
{
    // Country portion of UserMgr.Init, after local data load and before Config/Register.
    public sealed class OriginalUserCountryState
    {
        public string Area,CountryCode;
        public OriginalCountryInfo CountryInfo;
        private readonly OriginalCountryInfos countries;
        private readonly Action<string> error;
        public OriginalUserCountryState(OriginalCountryInfos countries,Action<string> error){this.countries=countries;this.error=error;}
        public void Initialize()
        {
            CountryInfo=countries.Get(CountryCode);
            if(Area!=CountryInfo.Area)
            {
                error(string.Concat("Area",Area," != CountryInfo.Area:",CountryInfo.Area));
                // Source rereads both fields after logging; retain the shared table-row identity.
                CountryInfo.Area=Area;
            }
        }
    }
}
