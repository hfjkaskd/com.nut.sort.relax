# Reward progress recovery

The authored runtime recovers TXMgr.GetTXStage (0x9C0930), IsGuidePassStage2Level (0x9C1BB0), GetTXStateDes (0x9C0BB4), StringUtil.ToLssFloat (0x9B522C), and UILSSUtil.GoldLSSFormat (0x9B7BE8). Branches were checked against the current APK's ARM64 implementation and typed fields. Raw native listings are kept outside this repository.

The reward document stays intact in UserLocalData. An explicit, reflection-free projection reads cal_cfg and the third bear_list record's RealLevel, Stage2RealLevel, caliper_logs and caliper_rank. No server values or derived level thresholds are invented. Missing records fail instead of silently substituting a US fixture. This projection consumes already-initialized saved values; the server document's Init derivation has not yet been restored.

| Stage | First matching condition |
| --- | --- |
| 1 | Requested level below 3; no reward document needed |
| 2 | Requested level at or below RealLevel |
| 3 | Requested level at or below Stage2RealLevel |
| 0 | Empty target gold, with the source diagnostic |
| 4 | Gold below the parsed target, or IsGuideGold |
| 7 | LoginDay at or above caliper_logs |
| 6 | TodayPassLevelCount at or above cal_cfg |
| 5 | Remaining case |

The stage-two guide checks its completed flag before accessing the document, then compares Level strictly above RealLevel with no upper bound. Stage descriptions use original table IDs 6, 7, 9, 24, 10, 11 and 12. Stage three supplies the last displayed level, not a remaining level count. Stage seven subtracts UserLevel, not gameplay Level, without clamping negative results.

Currency formatting uses the source culture cache behavior, C currency format, ASCII-space removal, and country-specific suffix handling. ja-JP/id-ID/ur-PK truncate fractions toward zero. th-TH replaces THB with the baht symbol. Explicit culture arguments invalidate the cache; a changed default-country provider alone does not. Target parsing follows current culture; format failures log and return invariant numeric text as in the source. These are runtime rules shared by Editor and player, not Editor fallbacks.

Validation: all 25 Unity regression groups pass using Unity 2022.3.62f3. Added cases cover all seven stages, inclusive boundaries, guide priority, text arguments, NaN comparisons, missing reward records, source constructor defaults, case-varied fields, JSON preservation, current-culture parsing and regional currency formatting. Test reward values are isolated synthetic data and are never installed as production configuration. PlayerPrefs were restored by the existing validation fixture.

**The production reward banner and initialization action host remain unbound.** This checkpoint supplies the actual rules and formatting they require, not a completed reward lifecycle. Native banner entry/hold/fly animation, top-gold hint, TX progress view, source country initialization, AB/GM routing and device comparison still require work. SDK calls and configuration treatment remain unchanged. Unity/Mono culture data and extreme floating-point conversion have not been compared across original Android and target players; these tests do not establish complete visual or cross-device equivalence.
