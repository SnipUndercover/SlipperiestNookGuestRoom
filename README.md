# Slipperiest Nook - Snip's Guest Room Mechanic

This repository contains the code I wrote for my guest room in Spooooky's map, [Slipperiest Nook](https://gamebanana.com/mods/666241).  
I highly suggest playing the map and my room first to see and experience it for yourself.

## Technical explanation

The way the crouch boost works is relatively simple. All of the logic behind it is executed **only when inside Slipperiest Nook** - the hooks and shader are loaded on enter and unloaded on exit.

**First, the actual gameplay mechanics.** On spawn, we add a [`CrumpleBoostComponent`](https://github.com/SnipUndercover/SlipperiestNookGuestRoom/blob/main/Source/CrumpleBoostComponent.cs) to the player on spawn, which handles the bulk of the logic.   It stays disabled until entering a trigger that enables it.

When crouching, its `WindUpStrength` approaches `MaxWindUpStrength` *(`1f`, or 100%)* over `MaxWindUpTimeSeconds` seconds *(`1.5f`)*.  
The height of the jump is modified by reading the current `JumpHeight` extended variant value, then multiplying it by `1 + WindUpStrength`.
This effectively scales it from 1&times; to 2&times; jump height depending on how long Madeline was crouching.

If `WindUpStrength` was greater than or equal to `0.25f` *(25%)*, a spring bounce sound effect is played.  
Its pitch is set to `1.25f - WindUpStrength / 2`, making it scale from `1.25f` to `0.75f`. Its volume is also set to `0.5f`, or 50%.

If `WindUpStrength` was greater than or equal to `0.5f` *(50%)*, the player is forced to hold jump via `AutoJump`. This is done to limit cheesability.  
Say you made a mistake and held crouch for too long, but you're not supposed to boost your next jump.
It's easy to still fix the situation by just tapping jump instead of holding it.  
Forcing the jump to be held via `AutoJump` enforces that the shortest jump will still give quite a bit of height, forcing the player to think about their movements.

This mechanic is implemented by setting `AutoJump` to `true` and setting `AutoJumpTimer` to `(WindUpStrength - 0.5f) / 2`.   
This maps the value of `WindUpStrength` from [`0.5f`, `1f`] to [`0f`, `0.25f`], making jump be held between 0s and 0.25s depending on how long crouch was held.

If Madeline stops crouching or becomes airborne, there are `MaxWindUpLeniencyTimer` seconds *(`10 * FrameLengthSeconds`; 10 frames or ~0.167 seconds)* of leniency where `WindUpStrength` is not reset. This makes the gameplay experience smoother and more forgiving.

---

**Next, the player flash.** It is only applied when `WindUpStrength` is greater than or equal to `FlashStrengthThreshold` *(`0.25f`, or 25%)*.

The implementation uses two things:
1. a screen-sized (320&times;180) `FlashPlayerRT` `VirtualRenderTarget` *(a wrapper for FNA's `RenderTarget2D`, which is like a texture that you can also render to)*
1. a custom `FlashShader` `CustomEffect` that interpolates the colors of a texture to white depending on the value of `flash_strength`; a `float` that ranges from `0f` to `1f`

The player sprite *(not hair!)* is first rendered to `FlashPlayerRT`. This is done by hijacking the `SpriteBatch` mid-`Player.Render`:
- restart the `SpriteBatch`, swapping in our `FlashPlayerRT` rendertarget and replacing the transformation matrix with the identity matrix
- adjust the player `RenderPosition` to capture Madeline in RT jail
- **let the sprite render with `orig(self);`**
- restore the `RenderPosition`
- restart the `SpriteBatch` with the previous parameters

Then, `FlashPlayerRT` is rendered to the screen with the custom shader:
- assign `flash_strength` with the value of `WindUpStrength`
- restart the `SpriteBatch`, replacing the shader with our `FlashShader`
- render our `FlashPlayerRT` at the camera position
- restart the `SpriteBatch` with the previous parameters

> [!NOTE]
> The reason we don't apply the shader as it draws the player to the rendertarget is because of SkinModHelper+'s skin colorgrade shader.  
> We apply our hook *after* SMH+, so if we were to render the player with our `FlashShader` and call `orig(self)`, following the hook chain, SMH+'s hook would eventually run and replace the shader with its colorgrade shader, ruining our plan.

The flash shader consists of one pixel shader; it samples the texture at the given UV position, adds white premultiplied by `flash_strength` and the sampled color's alpha and returns the result.  
By modifying `flash_strength`, we can control how much white is applied to the texture.

---

**Finally, the naming.** Consider the following: lol and lmao. Some may even say haha.  
I just picked funny non-obvious names to obfuscate their true meaning, mostly to prevent accidental leaks.