# Guide virtual dispatch correction

Recomputed the ARM64 Il2CppClass layout directly from the current exported il2cpp.h using fixed-width ctypes fields and natural 8-byte pointer alignment. The vtable begins at 0x138. Each VirtualInvokeData entry is 16 bytes. This corrects the previously assumed 0x128 base.

The native instruction at 0x9E4054 loads a method pair at class offset 0x1E8: slot 11, SuccessPanel.GetCallback. The visual target in ShowGuide remains MoreGetBtn at field offset 0x48; the original deliberately invokes GetCallback, not MoreGetCallback. The new OriginalGuideSuccessBinding explicitly names these different roles so production wiring cannot infer the wrong action from the button name.

The instruction at 0x9E3E80 uses offset 0x1A8: slot 7, MainPanel.RefreshLssPanel. Renamed the UI contract to RefreshMain. MainPanel's source RefreshLssPanel calls Top.Refresh then Bottom.Refresh. Its inherited TweenEndRefreshPanel is empty and must not be substituted.

Offset 0x188 maps to slot 5. Existing gold/coin component refresh boundaries retain their refresh purpose. This correction changes interface semantics and audit conclusions before production UI binding; earlier observer-only tests could verify sequence but did not prove native method identity. Compile/regression verification checks current callers remain consistent; the native layout and method schema provide the method-identity evidence.

Full production UI binding and whole-game 1:1 verification remain incomplete. Native header and assembly remain local, not included in the commit.

Unity 2022.3.62f3 regression after correction: 88 PASS markers in Library/unity-guide-dispatch-correction-validation.log; no compiler-error or exception markers; preferences restored.
