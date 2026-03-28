using System;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;
using MonoMod.Utils;

namespace Celeste.Mod.SlipperiestNook;

/// <summary>
///   A temporary <see cref="SpriteBatch"/>.
/// </summary>
/// <remarks>
///   When constructed, the current <see cref="SpriteBatch"/> properties and <see cref="RenderTarget2D"/> are preserved,
///   then the <see cref="SpriteBatch"/> is ended, the <see cref="RenderTarget2D"/> is swapped if necessary,
///   and finally the <see cref="SpriteBatch"/> is restarted with the new properties.<br/>
///   When disposed, the previous <see cref="SpriteBatch"/> properties and <see cref="RenderTarget2D"/> are restored.
///   <br/>
///   Useful when interrupting a <see cref="SpriteBatch"/> mid-render to, for example, render a specific entity to a
///   temporary <see cref="RenderTarget2D"/> while applying a custom shader, all while preserving the previous
///   configuration.
/// </remarks>
public ref struct TemporarySpriteBatch : IDisposable
{
    /// <summary>
    ///   The <see cref="SpriteSortMode"/> of this <see cref="TemporarySpriteBatch"/>.
    /// </summary>
    public readonly SpriteSortMode CurrentSortMode;

    /// <summary>
    ///   The <see cref="BlendState"/> of this <see cref="TemporarySpriteBatch"/>.
    /// </summary>
    public readonly BlendState CurrentBlendState;

    /// <summary>
    ///   The <see cref="SamplerState"/> of this <see cref="TemporarySpriteBatch"/>.
    /// </summary>
    public readonly SamplerState CurrentSamplerState;

    /// <summary>
    ///   The <see cref="DepthStencilState"/> of this <see cref="TemporarySpriteBatch"/>.
    /// </summary>
    public readonly DepthStencilState CurrentDepthStencilState;

    /// <summary>
    ///   The <see cref="RasterizerState"/> of this <see cref="TemporarySpriteBatch"/>.
    /// </summary>
    public readonly RasterizerState CurrentRasterizerState;

    /// <summary>
    ///   The custom <see cref="Effect"/> of this <see cref="TemporarySpriteBatch"/>.
    /// </summary>
    public readonly Effect CurrentCustomEffect;

    /// <summary>
    ///   The transformation <see cref="Matrix"/> of this <see cref="TemporarySpriteBatch"/>.
    /// </summary>
    public readonly Matrix CurrentTransformMatrix;

    /// <summary>
    ///   The <see cref="RenderTarget2D"/> swapped in for the duration of this <see cref="TemporarySpriteBatch"/>,
    ///   if <see cref="HasRenderTarget"/> is <c>true</c>; else <c>null</c>.
    /// </summary>
    public readonly RenderTarget2D? CurrentRenderTarget;

    /// <summary>
    ///   The <see cref="SpriteSortMode"/> that was used prior to the start of this
    ///   <see cref="TemporarySpriteBatch"/>.
    /// </summary>
    public readonly SpriteSortMode PreviousSortMode;

    /// <summary>
    ///   The <see cref="BlendState"/> that was used prior to the start of this
    ///   <see cref="TemporarySpriteBatch"/>.
    /// </summary>
    public readonly BlendState PreviousBlendState;

    /// <summary>
    ///   The <see cref="SamplerState"/> that was used prior to the start of this
    ///   <see cref="TemporarySpriteBatch"/>.
    /// </summary>
    public readonly SamplerState PreviousSamplerState;

    /// <summary>
    ///   The <see cref="DepthStencilState"/> that was used prior to the start of this
    ///   <see cref="TemporarySpriteBatch"/>.
    /// </summary>
    public readonly DepthStencilState PreviousDepthStencilState;

    /// <summary>
    ///   The <see cref="RasterizerState"/> that was used prior to the start of this
    ///   <see cref="TemporarySpriteBatch"/>.
    /// </summary>
    public readonly RasterizerState PreviousRasterizerState;

    /// <summary>
    ///   The custom <see cref="Effect"/> that was used prior to the start of this
    ///   <see cref="TemporarySpriteBatch"/>.
    /// </summary>
    public readonly Effect PreviousCustomEffect;

    /// <summary>
    ///   The transformation <see cref="Matrix"/> that was used prior to the start of this
    ///   <see cref="TemporarySpriteBatch"/>.
    /// </summary>
    public readonly Matrix PreviousTransformMatrix;

    /// <summary>
    ///   The <see cref="RenderTargetBinding"/>s that were used prior to the start of this
    ///   <see cref="TemporarySpriteBatch"/> if <see cref="HasRenderTarget"/> is <c>true</c>; else <c>null</c>.
    /// </summary>
    public readonly RenderTargetBinding[]? PreviousRenderTargets;

    /// <summary>
    ///   Whether a <see cref="RenderTarget2D"/> was swapped in for the duration of this
    ///   <see cref="TemporarySpriteBatch"/>
    /// </summary>
    public readonly bool HasRenderTarget;

    /// <summary>
    ///   Whether this <see cref="TemporarySpriteBatch"/> is still active.
    /// </summary>
    public bool Active { get; private set; }

    internal TemporarySpriteBatch(
        bool hasSortMode, SpriteSortMode? sortMode,
        bool hasBlendState, BlendState? blendState,
        bool hasSamplerState, SamplerState? samplerState,
        bool hasDepthStencilState, DepthStencilState? depthStencilState,
        bool hasRasterizerState, RasterizerState? rasterizerState,
        bool hasCustomEffect, Effect? customEffect,
        bool hasTransformMatrix, Matrix? transformMatrix,
        bool hasRenderTarget, RenderTarget2D? renderTarget)
    {
        GetSpriteBatchFields(
            out PreviousSortMode,
            out PreviousBlendState,
            out PreviousSamplerState,
            out PreviousDepthStencilState,
            out PreviousRasterizerState,
            out PreviousCustomEffect,
            out PreviousTransformMatrix);

        CurrentSortMode = hasSortMode ? sortMode!.Value : PreviousSortMode;
        CurrentBlendState = hasBlendState ? blendState! : PreviousBlendState;
        CurrentSamplerState = hasSamplerState ? samplerState! : PreviousSamplerState;
        CurrentDepthStencilState = hasDepthStencilState ? depthStencilState! : PreviousDepthStencilState;
        CurrentRasterizerState = hasRasterizerState ? rasterizerState! : PreviousRasterizerState;
        CurrentCustomEffect = hasCustomEffect ? customEffect! : PreviousCustomEffect;
        CurrentTransformMatrix = hasTransformMatrix ? transformMatrix!.Value : PreviousTransformMatrix;

        HasRenderTarget = hasRenderTarget;
        if (hasRenderTarget)
        {
            PreviousRenderTargets = Engine.Graphics.GraphicsDevice.GetRenderTargets();
            CurrentRenderTarget = renderTarget;
        }

        Active = true;
        Draw.SpriteBatch.End();
        if (hasRenderTarget)
            Engine.Graphics.GraphicsDevice.SetRenderTarget(CurrentRenderTarget);
        Draw.SpriteBatch.Begin(
            CurrentSortMode,
            CurrentBlendState,
            CurrentSamplerState,
            CurrentDepthStencilState,
            CurrentRasterizerState,
            CurrentCustomEffect,
            CurrentTransformMatrix);
    }

    /// <summary>
    ///   End this <see cref="TemporarySpriteBatch"/>, restore the previous render targets if necessary, and restore
    ///   the previous <see cref="SpriteBatch"/> properties.
    /// </summary>
    public void Dispose()
    {
        ObjectDisposedException.ThrowIf(!Active, typeof(TemporarySpriteBatch));

        Active = false;
        Draw.SpriteBatch.End();
        if (HasRenderTarget)
            Engine.Graphics.GraphicsDevice.SetRenderTargets(PreviousRenderTargets);
        Draw.SpriteBatch.Begin(
            PreviousSortMode,
            PreviousBlendState,
            PreviousSamplerState,
            PreviousDepthStencilState,
            PreviousRasterizerState,
            PreviousCustomEffect,
            PreviousTransformMatrix);
    }

    #region ew. reflection.

    private static void GetSpriteBatchFields(
        out SpriteSortMode sortMode,
        out BlendState blendState,
        out SamplerState samplerState,
        out DepthStencilState depthStencilState,
        out RasterizerState rasterizerState,
        out Effect customEffect,
        out Matrix transformMatrix)
    {
        DynamicData dynData = DynamicData.For(Draw.SpriteBatch);
        sortMode = dynData.Get<SpriteSortMode>("sortMode");
        blendState = dynData.Get<BlendState>("blendState")!;
        samplerState = dynData.Get<SamplerState>("samplerState")!;
        depthStencilState = dynData.Get<DepthStencilState>("depthStencilState")!;
        rasterizerState = dynData.Get<RasterizerState>("rasterizerState")!;
        customEffect = dynData.Get<Effect>("customEffect")!;
        transformMatrix = dynData.Get<Matrix>("transformMatrix");
    }

    #endregion
}
