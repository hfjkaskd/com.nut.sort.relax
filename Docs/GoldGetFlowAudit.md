# UserMgr GoldGet routing

Restored UserMgr.GoldGet (0x9c2bec) and its response consumer (0x9c3da0) as OriginalGoldGetFlow. Transport and response initialization remain external boundaries; no payment, successful ad, balance grant or SDK implementation was added.

## Native branches

For captured level <= 2 (including zero/negative values), the entry boolean chooses between ShowPanel(28, new object[]{level}) and the current MainPanel.Top.GoldItem.GoldHintText. The text branch replaces LF with a space and applies the existing source ColorToWhite transformation, retaining CR and named-color behavior. Neither branch invokes a request, refreshes currency, saves, or invokes the guide callback.

For level > 2, entry boolean is ignored and RequestMgr.GoldGet receives the reusable response callback. The consumer does not invent arguments or payload fields. Existing request/response initialization is the caller's responsibility.

A non-null response succeeds only when kinetic_gap equals NO-0, using the source S2CLSSBase.IsSuccess getter (0x9ec2b4). JSON field names follow existing typed-binding case-insensitive lookup, while the status value comparison is exact/case-sensitive. message_status is not the success field. A successful consumer first calls GoldItem.RefreshLssGold(showTip=true, addGold=0), then opens panel 29 with Array.Empty<object>(). This is a display refresh, not setting the user's balance to zero. A null/failed response shows localized tip 2 without those actions. Exceptions prevent subsequent actions; repeated callbacks are not suppressed.

## Verification

Library/gold-get-validation.log: 129 PASS markers including final content PASS, with no C# errors or exceptions. Boundary tests cover <=2 values, fresh early argument arrays, lazy HUD reads, original tip conversion, >2 request regardless of boolean, cached callback identity, null/missing/wrong status, response display ordering, shared empty args and failed HUD refresh stopping panel creation. Responses in these tests are explicitly synthetic fixtures and are not production payment results.

The existing SuccessGuideHost Play chain now passes the real confirmation Button's arguments into OriginalGoldGetFlow. Its early path must dispatch panel 28 with captured level 1 while the confirmation is closing, and must not enter the request or response-refresh boundary. The final withdrawal-guide callback is still pending rather than being completed artificially.

## Remaining scope

TXLevelPanel 28 is still a dispatch boundary and must be restored next along the primary early withdrawal branch. Later request transport/response-initialization semantics, TXSuccessPanel 29, panel 22, production startup composition and full country/AB lifecycle coverage remain incomplete. SDK behavior remains unchanged. These branch tests do not establish full lifecycle or original-device visual equivalence.

Play result: Library/success-guide-gold-get-play.log contains NUT_SUCCESS_GUIDE_HOST_PLAY_PASS with no C# errors or exceptions. The actual Confirm Button enters the native early branch and dispatches panel 28 before animated confirmation removal. The intentional duplicate TXPanel diagnostic is an existing ownership assertion. Validation processes exited and preference backup was removed after restoration.
