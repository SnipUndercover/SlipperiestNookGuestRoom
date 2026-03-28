using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.SlipperiestNook;

[CustomEntity($"{nameof(SlipperiestNook)}/{nameof(CrumpleBoostTrigger)}")]
public sealed class CrumpleBoostTrigger(EntityData data, Vector2 offset) : Trigger(data, offset)
{
    private readonly bool Enable = data.Bool("enable");

    public override void OnEnter(Player player)
    {
        base.OnEnter(player);

        YoinkySploinkyModule.Session.CrumpleBoostEnabled = Enable;
        if (!Enable)
            player.Get<CrumpleBoostComponent>()?.ConsumeWindUp();
    }
}
