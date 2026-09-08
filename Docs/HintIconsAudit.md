# PlayerGoldGetHint channel icons

OriginalChannelInfos reads the existing channelInfos table section without reflection and restores PayTable.GetChannelInfo (0x9EA574): source-order exact string match, shared first matching row, error log and null on no match. It does not case-normalize channel names or silently choose a different channel. Channel metadata strings are retained for later consumers; email/name form rules are not implemented here.

ShowSelf (0x9E06D4) selects the current country's channel list entry at UserLssInfo.GetType, then resolves the ChannelInfo before updating the self-name label and loading the icon. GetType is an original serialized integer field that shadows Object.GetType in the exported class; it is read as explicit JSON data, never invoked through reflection. Missing integer field defaults to zero; invalid list index is not clamped. Selection returns metadata separately so the future view can preserve name/icon/info assignment order.

ResourceMgr.GetPayChannelIcon (0x9F7358) resolves Atlas/PaySimple/{channel} for false and Atlas/Pay/{channel} for true. PlayerGoldGetHint uses false. The restored icon helper preserves three distinct assignment paths: ordinary Show assigns only a non-null result; UILSSUtil.RandomPayChannel (0x9C0388), used by RefreshTip and Show(Action), assigns even a missing result; ShowSelf assigns the selected channel's result directly and makes no random draw. Existing sprite assets are reused, with no new large serialized resource references or SDK behavior.

Validation covers original channel metadata, US selected index and shared identity, missing-index zero/default and invalid-index failure, both resource paths, actual current sprite loading, and preserved-versus-cleared Image.sprite behavior with draw/load counts. The view prefab, animation, scheduling integration and production Top binding remain pending. No new visual assets or device parity claims are introduced.

Final regression: 42 complete Unity PASS markers in reverse-workspace reconstruction-nut/logs/unity-hint-icons-validation.log, no compiler/validation exceptions; gameplay preference backup absent after restoration.
