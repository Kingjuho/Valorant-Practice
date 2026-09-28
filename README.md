# Valorant-Practice
VALORANT-like tactical fps practice game

## Basic movement prototype

Open `Assets/Scenes/SampleScene.unity`, enter Play mode, then click the Game view.

- Mouse: look around; WASD: move relative to the player's horizontal facing.
- Escape: release the cursor and suspend controls; click again to resume.
- 1: gun movement speed (5.4 m/s for Vandal, Phantom and Sheriff).
- 3: knife movement speed (6.75 m/s).
- These keys only change the movement speed profile; weapons are not rendered yet.

Tune `Mouse Sensitivity` on Player's `FirstPersonController` component using
VALORANT's base `Sensitivity: Aim` value. The default is 1. Both look axes use
`mouse delta * sensitivity * 0.07` degrees, with no delta-time multiplier.
DPI is reflected in the mouse input already and is not multiplied again in code.

Using the supplied hipfire reference:
`cm/360 = (360 * 2.54) / (DPI * sensitivity * 0.07)`.
At 800 DPI, sensitivity 1 gives approximately 16.33 cm/360, and 0.35 gives
46.65 cm/360 (280 eDPI). These distances assume one input delta unit per mouse
count; physical travel still needs verification in the running application.
ADS and scoped sensitivity multipliers are not implemented.

One Unity unit represents one metre. Diagonal movement is normalized.
The existing camera FOV and initial eye height are preserved.

This prototype uses immediate acceleration/stopping. Braking, walk/crouch,
jumping and shooting are not implemented yet. Gravity and capsule dimensions
are prototype settings, not verified VALORANT physics values.

Movement references:
- https://liquipedia.net/valorant/Vandal
- https://liquipedia.net/valorant/Phantom
- https://liquipedia.net/valorant/Sheriff
- https://valorant.fandom.com/wiki/Melee
