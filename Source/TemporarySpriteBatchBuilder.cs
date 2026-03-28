using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Celeste.Mod.SlipperiestNook;

/// <summary>
///   A <see cref="TemporarySpriteBatch"/> builder.
/// </summary>
/// <remarks>
///   This class lets users prepare to restart the <see cref="SpriteBatch"/> with custom properties or
///   <see cref="RenderTarget2D"/>.
/// </remarks>
public sealed class TemporarySpriteBatchBuilder
{
    /// <summary>
    ///   Whether the <see cref="SpriteBatch"/>'s
    ///   <see cref="Microsoft.Xna.Framework.Graphics.SpriteSortMode"/> should be overridden.
    /// </summary>
    /// <seealso cref="SortMode"/>
    public bool HasSortMode { get; private set; }

    /// <summary>
    ///   Whether the <see cref="SpriteBatch"/>'s
    ///   <see cref="Microsoft.Xna.Framework.Graphics.BlendState"/> should be overridden.
    /// </summary>
    /// <seealso cref="BlendState"/>
    public bool HasBlendState { get; private set; }

    /// <summary>
    ///   Whether the <see cref="SpriteBatch"/>'s
    ///   <see cref="Microsoft.Xna.Framework.Graphics.SamplerState"/> should be overridden.
    /// </summary>
    /// <seealso cref="SamplerState"/>
    public bool HasSamplerState { get; private set; }

    /// <summary>
    ///   Whether the <see cref="SpriteBatch"/>'s
    ///   <see cref="Microsoft.Xna.Framework.Graphics.DepthStencilState"/> should be overridden.
    /// </summary>
    /// <seealso cref="DepthStencilState"/>
    public bool HasDepthStencilState { get; private set; }

    /// <summary>
    ///   Whether the <see cref="SpriteBatch"/>'s
    ///   <see cref="Microsoft.Xna.Framework.Graphics.RasterizerState"/> should be overridden.
    /// </summary>
    /// <seealso cref="RasterizerState"/>
    public bool HasRasterizerState { get; private set; }

    /// <summary>
    ///   Whether the <see cref="SpriteBatch"/>'s custom
    ///   <see cref="Microsoft.Xna.Framework.Graphics.Effect"/> should be overridden.
    /// </summary>
    /// <seealso cref="CustomEffect"/>
    public bool HasCustomEffect { get; private set; }

    /// <summary>
    ///   Whether the <see cref="SpriteBatch"/>'s transformation
    ///   <see cref="Microsoft.Xna.Framework.Matrix"/> should be overridden.
    /// </summary>
    /// <seealso cref="TransformMatrix"/>
    public bool HasTransformMatrix { get; private set; }

    /// <summary>
    ///   Whether to swap the current <see cref="Microsoft.Xna.Framework.Graphics.RenderTarget2D"/>
    ///   in-between <see cref="SpriteBatch"/>es.
    /// </summary>
    /// <seealso cref="RenderTarget"/>
    public bool HasRenderTarget { get; private set; }

    /// <summary>
    ///   The <see cref="Microsoft.Xna.Framework.Graphics.SpriteSortMode"/> that the new
    ///   <see cref="SpriteBatch"/> should use.
    /// </summary>
    /// <remarks>
    ///    Contains a value when <see cref="HasSortMode"/> is <c>true</c>; <c>null</c> otherwise.
    /// </remarks>
    public SpriteSortMode? SortMode { get; private set; }

    /// <summary>
    ///   The <see cref="Microsoft.Xna.Framework.Graphics.BlendState"/> that the new
    ///   <see cref="SpriteBatch"/> should use.
    /// </summary>
    /// <remarks>
    ///    Contains a value when <see cref="HasBlendState"/> is <c>true</c>; <c>null</c> otherwise.
    /// </remarks>
    public BlendState? BlendState { get; private set; }

    /// <summary>
    ///   The <see cref="Microsoft.Xna.Framework.Graphics.SamplerState"/> that the new
    ///   <see cref="SpriteBatch"/> should use.
    /// </summary>
    /// <remarks>
    ///    Contains a value when <see cref="HasSamplerState"/> is <c>true</c>; <c>null</c> otherwise.
    /// </remarks>
    public SamplerState? SamplerState { get; private set; }

    /// <summary>
    ///   The <see cref="Microsoft.Xna.Framework.Graphics.DepthStencilState"/> that the new
    ///   <see cref="SpriteBatch"/> should use.
    /// </summary>
    /// <remarks>
    ///    Contains a value when <see cref="HasDepthStencilState"/> is <c>true</c>; <c>null</c> otherwise.
    /// </remarks>
    public DepthStencilState? DepthStencilState { get; private set; }

    /// <summary>
    ///   The <see cref="Microsoft.Xna.Framework.Graphics.RasterizerState"/> that the new
    ///   <see cref="SpriteBatch"/> should use.
    /// </summary>
    /// <remarks>
    ///    Contains a value when <see cref="HasRasterizerState"/> is <c>true</c>; <c>null</c> otherwise.
    /// </remarks>
    public RasterizerState? RasterizerState { get; private set; }

    /// <summary>
    ///   The custom <see cref="Microsoft.Xna.Framework.Graphics.Effect"/> that the new
    ///   <see cref="SpriteBatch"/> should use.
    /// </summary>
    /// <remarks>
    ///    Contains a value when <see cref="HasCustomEffect"/> is <c>true</c>; <c>null</c> otherwise.
    /// </remarks>
    public Effect? CustomEffect { get; private set; }

    /// <summary>
    ///   The transformation <see cref="Microsoft.Xna.Framework.Matrix"/> that the new
    ///   <see cref="SpriteBatch"/> should use.
    /// </summary>
    /// <remarks>
    ///    Contains a value when <see cref="HasTransformMatrix"/> is <c>true</c>; <c>null</c> otherwise.
    /// </remarks>
    public Matrix? TransformMatrix { get; private set; }

    /// <summary>
    ///   The <see cref="Microsoft.Xna.Framework.Graphics.RenderTarget2D"/> that should be swapped to
    ///   in-between <see cref="SpriteBatch"/>es.
    /// </summary>
    /// <remarks>
    ///    Contains a value when <see cref="HasRenderTarget"/> is <c>true</c>; <c>null</c> otherwise.
    /// </remarks>
    public RenderTarget2D? RenderTarget { get; private set; }

    /// <summary>
    ///   Override the new <see cref="SpriteBatch"/>'s <see cref="Microsoft.Xna.Framework.Graphics.SpriteSortMode"/>.
    /// </summary>
    /// <param name="sortMode">
    ///   The new sort mode.
    /// </param>
    public TemporarySpriteBatchBuilder WithSortMode(SpriteSortMode sortMode)
    {
        HasSortMode = true;
        SortMode = sortMode;
        return this;
    }

    /// <summary>
    ///   Override the new <see cref="SpriteBatch"/>'s <see cref="Microsoft.Xna.Framework.Graphics.BlendState"/>.
    /// </summary>
    /// <param name="blendState">
    ///   The new blend state.
    /// </param>
    public TemporarySpriteBatchBuilder WithBlendState(BlendState blendState)
    {
        HasBlendState = true;
        BlendState = blendState;
        return this;
    }

    /// <summary>
    ///   Override the new <see cref="SpriteBatch"/>'s <see cref="Microsoft.Xna.Framework.Graphics.SamplerState"/>.
    /// </summary>
    /// <param name="samplerState">
    ///   The new sampler state.
    /// </param>
    public TemporarySpriteBatchBuilder WithSamplerState(SamplerState samplerState)
    {
        HasSamplerState = true;
        SamplerState = samplerState;
        return this;
    }

    /// <summary>
    ///   Override the new <see cref="SpriteBatch"/>'s <see cref="Microsoft.Xna.Framework.Graphics.DepthStencilState"/>.
    /// </summary>
    /// <param name="depthStencilState">
    ///   The new depth stencil state.
    /// </param>
    public TemporarySpriteBatchBuilder WithDepthStencilState(DepthStencilState depthStencilState)
    {
        HasDepthStencilState = true;
        DepthStencilState = depthStencilState;
        return this;
    }

    /// <summary>
    ///   Override the new <see cref="SpriteBatch"/>'s <see cref="Microsoft.Xna.Framework.Graphics.RasterizerState"/>.
    /// </summary>
    /// <param name="rasterizerState">
    ///   The new rasterizer state.
    /// </param>
    public TemporarySpriteBatchBuilder WithRasterizerState(RasterizerState rasterizerState)
    {
        HasRasterizerState = true;
        RasterizerState = rasterizerState;
        return this;
    }

    /// <summary>
    ///   Override the new <see cref="SpriteBatch"/>'s custom <see cref="Microsoft.Xna.Framework.Graphics.Effect"/>.
    /// </summary>
    /// <param name="customEffect">
    ///   The new custom effect.
    /// </param>
    public TemporarySpriteBatchBuilder WithCustomEffect(Effect customEffect)
    {
        HasCustomEffect = true;
        CustomEffect = customEffect;
        return this;
    }

    /// <summary>
    ///   Override the new <see cref="SpriteBatch"/>'s transformation <see cref="Microsoft.Xna.Framework.Matrix"/>.
    /// </summary>
    /// <param name="transformMatrix">
    ///   The new transformation matrix.
    /// </param>
    public TemporarySpriteBatchBuilder WithTransformMatrix(Matrix transformMatrix)
    {
        HasTransformMatrix = true;
        TransformMatrix = transformMatrix;
        return this;
    }

    /// <summary>
    ///   Override the <see cref="RenderTarget2D"/> in-between <see cref="SpriteBatch"/>es.
    /// </summary>
    /// <param name="renderTarget">
    ///   The new render target.
    /// </param>
    public TemporarySpriteBatchBuilder WithRenderTarget(RenderTarget2D? renderTarget)
    {
        HasRenderTarget = true;
        RenderTarget = renderTarget;
        return this;
    }

    /// <summary>
    ///   Restart the <see cref="SpriteBatch"/> with the configured properties.
    /// </summary>
    /// <returns>
    ///   A <see cref="TemporarySpriteBatch"/> that will restore the previous <see cref="SpriteBatch"/> properties
    ///   when disposed. Remember to put it in a <c>using</c> block.
    /// </returns>
    public TemporarySpriteBatch Use()
        => new(
            HasSortMode, SortMode,
            HasBlendState, BlendState,
            HasSamplerState, SamplerState,
            HasDepthStencilState, DepthStencilState,
            HasRasterizerState, RasterizerState,
            HasCustomEffect, CustomEffect,
            HasTransformMatrix, TransformMatrix,
            HasRenderTarget, RenderTarget
        );
}
