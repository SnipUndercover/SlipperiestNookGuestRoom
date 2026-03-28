using System;
using System.Diagnostics;
using System.Reflection.Emit;
using Monocle;
using MonoMod.RuntimeDetour;

namespace Celeste.Mod.SlipperiestNook;

public sealed class YoinkySploinkyModule : EverestModule
{
    private const string MapSid = "Spooooky/SlipperiestNook/SlipperiestNook";

    public static YoinkySploinkyModule Instance { get; private set; } = default!;

    public override Type SessionType { get; } = typeof(YoinkySploinkySession);
    public static YoinkySploinkySession Session => (YoinkySploinkySession)Instance._Session;

    public YoinkySploinkyModule()
    {
        Instance = this;
    #if DEBUG
        // debug builds use verbose logging
        Logger.SetLogLevel(nameof(SlipperiestNook), LogLevel.Verbose);
    #else
        // release builds use info logging to reduce spam in log files
        Logger.SetLogLevel(nameof(SlipperiestNook), LogLevel.Info);
    #endif
    }

    public override void Load()
    {
        ExtendedVariantInterop.Load();

        // Level.OnEnter is not triggered when invoking the load command from the debug console,
        // but LevelLoader.OnLoadingThread is
        Everest.Events.LevelLoader.OnLoadingThread += TryApplyCrumpleHooks;
        Everest.Events.Level.OnExit += TryUnapplyCrumpleHooks;

        if (Engine.Scene is AssetReloadHelper { OrigScene: Level { Session.Area.SID: MapSid } })
            // we're being hot reloaded in the map, load immediately
            CrumpleBoostComponent.ApplyHooks();
    }

    public override void Unload()
    {
        Everest.Events.LevelLoader.OnLoadingThread -= TryApplyCrumpleHooks;
        Everest.Events.Level.OnExit -= TryUnapplyCrumpleHooks;

        CrumpleBoostComponent.UnapplyHooks();
    }

    private static void TryApplyCrumpleHooks(Level level)
    {
        if (level.Session.Area.SID == MapSid)
            CrumpleBoostComponent.ApplyHooks();
    }

    private static void TryUnapplyCrumpleHooks(
        Level level,
        LevelExit exit,
        LevelExit.Mode mode,
        Session session,
        HiresSnow snow)
    {
        if (session.Area.SID == MapSid)
            CrumpleBoostComponent.UnapplyHooks();
    }
}
