using FMOD.Studio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;

namespace Celeste.Mod.SlipperiestNook;

internal class CrumpleBoostComponent() : Component(
    active: true,
#if DEBUG
    visible: true
#else
    visible: false
#endif
) {
    private static bool HooksApplied;
    private static Effect? FlashShader;
    private static VirtualRenderTarget? FlashPlayerRT;

    private const string LogId = $"{nameof(SlipperiestNook)}/{nameof(CrumpleBoostComponent)}";

    private const float FrameLengthSeconds = 1 / 60f;

    private const float MaxWindUpStrength = 1f;
    private const float MaxWindUpTimeSeconds = 1.5f;
    private float WindUpStrength;

    private const float MaxWindUpLeniencyTimer = 10 * FrameLengthSeconds;
    private float WindUpLeniencyTimer;

    private const float FlashStrengthThreshold = 0.25f;
    private const float FlashIntervalSeconds = 10 * FrameLengthSeconds;
    private float FlashTimer;

    public override void Update()
    {
        if (Entity is not Player player)
        {
            Logger.Warn(LogId, $"Attempted to add a {nameof(CrumpleBoostComponent)} to a non-{nameof(Player)} entity.");
            RemoveSelf();
            return;
        }

        if (!YoinkySploinkyModule.Session.CrumpleBoostEnabled)
            return;

        base.Update();

        if (player.Ducking)
        {
            if (player.OnGround())
                WindUpStrength = Calc.Approach(WindUpStrength, MaxWindUpStrength, Engine.DeltaTime / MaxWindUpTimeSeconds);
            WindUpLeniencyTimer = MaxWindUpLeniencyTimer;
        }
        else if (WindUpLeniencyTimer > 0f)
        {
            WindUpLeniencyTimer = Calc.Approach(WindUpLeniencyTimer, 0f, Engine.DeltaTime);
        }
        else
        {
            WindUpStrength = 0f;
            FlashTimer = 0f;
        }

        if (WindUpStrength < FlashStrengthThreshold)
            return;

        FlashTimer += Engine.DeltaTime;
        if (FlashTimer > 2 * FlashIntervalSeconds)
            FlashTimer -= 2 * FlashIntervalSeconds;
    }

    public void ConsumeWindUp()
    {
        WindUpStrength = 0f;
        WindUpLeniencyTimer = 0f;
        FlashTimer = 0f;
    }

    internal static void ApplyHooks()
    {
        if (HooksApplied)
            return;

        Everest.Events.Player.OnSpawn += AddCrumpleBoostComponent;
        On.Celeste.Player.Jump += ApplyCrumpleBoost;
        On.Celeste.PlayerSprite.Render += FlashOnCrumpleCharged;

        FlashShader = new Effect(
            Engine.Graphics.GraphicsDevice, Everest.Content.Get("Effects/Spooooky/SlipperiestNook/flash.cso").Data);

        HooksApplied = true;
    }

    internal static void UnapplyHooks()
    {
        if (!HooksApplied)
            return;

        Everest.Events.Player.OnSpawn -= AddCrumpleBoostComponent;
        On.Celeste.Player.Jump -= ApplyCrumpleBoost;
        On.Celeste.PlayerSprite.Render -= FlashOnCrumpleCharged;
        FlashShader?.Dispose();

        HooksApplied = false;
    }

    private static void AddCrumpleBoostComponent(Player player)
    {
        if (player.Get<CrumpleBoostComponent>() is null)
            player.Add(new CrumpleBoostComponent());
    }

    private const string JumpStrengthVariant = "JumpHeight";

    private static void ApplyCrumpleBoost(On.Celeste.Player.orig_Jump orig, Player self, bool particles, bool playSfx)
    {
        if (self.Get<CrumpleBoostComponent>() is not { WindUpStrength: > 0 } component)
        {
            orig(self, particles, playSfx);
            return;
        }

        float currentJumpStrength = (float)ExtendedVariantInterop.GetCurrentVariantValue(JumpStrengthVariant);
        float windUpStrength = component.WindUpStrength;

        try
        {
            ExtendedVariantInterop.TriggerFloatVariant(
                JumpStrengthVariant, currentJumpStrength * (1 + windUpStrength), revertOnDeath: false);

            orig(self, particles, playSfx);

            if (windUpStrength < 0.25f)
                return;

            EventInstance springSfx = self.Play(SFX.game_gen_spring);
            springSfx.setPitch(1.25f - windUpStrength / 2);
            springSfx.setVolume(0.5f);

            if (windUpStrength < 0.5f)
                return;

            // convert [0.5, 1] to [0, 0.25]
            self.AutoJumpTimer = (windUpStrength - 0.5f) / 2;
            self.AutoJump = true;
        }
        finally
        {
            component.ConsumeWindUp();
            ExtendedVariantInterop.TriggerFloatVariant(JumpStrengthVariant, currentJumpStrength, revertOnDeath: false);
        }
    }

    private static void FlashOnCrumpleCharged(On.Celeste.PlayerSprite.orig_Render orig, PlayerSprite self)
    {
        if (FlashShader is null)
        {
            orig(self);
            return;
        }

        if (self.Scene is not Level level)
        {
            orig(self);
            return;
        }

        if (self.Entity is not Player { Ducking: true } player)
        {
            orig(self);
            return;
        }

        if (player.Get<CrumpleBoostComponent>() is not {
            WindUpStrength: >= FlashStrengthThreshold,
            FlashTimer: < FlashIntervalSeconds,
        } component)
        {
            orig(self);
            return;
        }

        FlashPlayerRT ??= VirtualContent.CreateRenderTarget(
            "SlipperiestNook/FlashPlayer",
            GameplayBuffers.Gameplay?.Width ?? Celeste.GameWidth,
            GameplayBuffers.Gameplay?.Height ?? Celeste.GameHeight,
            preserve: false);

        // can't use the shader here as smh+ uses its colorgrade shader in orig.
        // (Sprite."Render" which is effectively Image.Render)
        // need to render the player to the rendertarget and then draw the target with the shader
        using (new TemporarySpriteBatchBuilder()
            //.WithCustomEffect(FlashShader)
            .WithRenderTarget(FlashPlayerRT)
            .WithTransformMatrix(Matrix.Identity).Use())
        {
            Engine.Graphics.GraphicsDevice.Clear(Color.Transparent);

            Vector2 renderPosition = self.RenderPosition;
            self.RenderPosition = level.WorldToScreen(self.Entity.Position) / 6;
            orig(self);
            self.RenderPosition = renderPosition;
        }

        FlashShader.Parameters["flash_strength"].SetValue(component.WindUpStrength);
        using (new TemporarySpriteBatchBuilder().WithCustomEffect(FlashShader).Use())
            Draw.SpriteBatch.Draw(
                FlashPlayerRT,
                level.Camera.Position,
                Color.White);
    }

#if DEBUG
    private static readonly Vector2 OffsetVector = new(0, -24f);

    public override void Render()
    {
        Draw.Point(Entity.Position + OffsetVector, WindUpStrength switch {
            <= 0f => Color.Transparent,
            < 0.5f => Color.LimeGreen,
            < 0.75f => Color.Yellow,
            < 1f => Color.Orange,
            _ => Color.IndianRed,
        });
    }
#endif
}
