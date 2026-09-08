# Registered panel refresh

OriginalPanelRegistry.Refresh implements UIMgr.RefreshLssPanel (0x9F7E54). It resolves the explicit panel name before lookup, invokes Refresh on the existing registered instance, and logs the original `panel exist :` error if absent. It does not instantiate, initialize, reactivate, hide, or replace the panel. A refresh exception propagates and leaves registration intact. Explicit name adapters preserve Obfuz compatibility without enum.ToString or reflection.

OriginalPanelRegistryValidation now checks name/refresh order, retained identity after exceptions, exact missing-panel error/type and absence of creation. The real FailPanel fixture changes current reward display data from 250 to 375 and refreshes through the manager, verifying the same instance displays the new value before its actual Restart Button closes it.

This completes another panel-manager operation; the manager and complete panels are still not fully bound into normal startup. It is not evidence of complete lifecycle or visual parity. SDK handling is unchanged.

Unity 2022.3.62f3 verification: 65 regression PASS markers including the expanded panel registry test in Library/unity-panel-refresh-validation.log. No compiler or validation exceptions; preference backup absent after restoration. The expected missing-panel error is asserted by the test.
