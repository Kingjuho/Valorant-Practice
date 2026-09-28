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

Movement uses a linear acceleration/braking approximation, adjustable on Player:

| Inspector setting | Default | Meaning |
| --- | --- | --- |
| Acceleration Time | 0.29 s | Rest to full speed, for both profiles |
| Gun Stop Time | 0.125 s | Full gun speed to rest |
| Knife Stop Time | 0.145 s | Full knife speed to rest |

Lower starting speeds stop sooner with the same deceleration rate. Opposing
input brakes to zero first, then accelerates in the new direction using any
time left in that frame. There is no additional counter-strafe braking bonus.
Perpendicular direction changes approach the new velocity at the acceleration
rate. Switching speed profiles also approaches the new speed gradually.
Cursor release, focus loss and disabling controls clear stored momentum.

Both velocity and displacement are integrated over each ramp, including frames
that cross a stop or reach full speed. These provisional targets come from a
2022 community measurement, not verified current VALORANT engine constants:
https://docs.google.com/document/u/2/d/e/2PACX-1vTsGYzGYiO78fcr_frhhBmzDaMvEqgBRd3b5u7TNzXRAhKflvFOovleWmavebIfFywh3DHppT75xHAk/pub

Walk/crouch, jumping and shooting are not implemented yet. Gravity and capsule
dimensions are also prototype settings. Stopping times here describe movement,
not the separate threshold at which shooting loses its movement penalty.

Movement references:
- https://liquipedia.net/valorant/Vandal
- https://liquipedia.net/valorant/Phantom
- https://liquipedia.net/valorant/Sheriff
- https://valorant.fandom.com/wiki/Melee
