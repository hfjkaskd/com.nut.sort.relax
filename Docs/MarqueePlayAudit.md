# Marquee main-owner coroutine and Play validation

OriginalGameScene.ScheduleDelay restores the TimeLSSUtil.DelayCallback ownership rule: the main LuoSiSort-equivalent MonoBehaviour starts a coroutine, yields WaitForSeconds(seconds), and invokes a non-null callback afterward. There is no item-owned coroutine, Editor fallback, unscaled delay or synthetic response. PMDItem's existing injected scheduling boundary can bind directly to this method.

OriginalMarqueePlayValidation runs the actual LuoSiSortGame scene at 480x1040. It instantiates the recovered PMDBullet under the existing Top hierarchy and uses deterministic validation-only marquee data, name and amount. It captures level three before advancing the live-level provider to four, waits for real Unity Update to launch, and uses the scene scheduler for the item's width callback. It never manually calls AdvanceLaunch or AdvanceTravel.

The Play check passed: timeScale zero holds both initial spawning and the main-owner callback; resuming launches and updates text-driven width after the coroutine; hiding the launcher retains moving tracks and main-owner callbacks; disabling the launcher stops further spawning while the existing item completes at negative half-width and remains active/ready. PlayerPrefs content is unchanged by the UI, and the preference fixture is restored afterward.

Evidence: reverse-workspace reconstruction-nut/logs/unity-marquee-play-validation.log contains NUT_MARQUEE_PLAY_VALIDATION_PASS without validation exceptions. The current target Library/ValidationCaptures/marquee-play.png was inspected: circular PayPal icon and original text/background render over the real board, partly entering from the right as expected. This is a current reconstruction capture, not an original-device comparison.

Production MainPanelTop still needs initialization and provider wiring. The validation provider is not a reward configuration or SDK implementation. Complete lifecycle, region AB/GM and original-device visual parity remain incomplete. SDK behavior is unchanged.

Full regression after the host change: all 38 PASS markers in reconstruction-nut/logs/unity-marquee-host-validation.log, with no compiler/validation exceptions.
