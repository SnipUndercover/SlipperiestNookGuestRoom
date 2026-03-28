using ModInteropImportGenerator;

namespace Celeste.Mod.SlipperiestNook;

[GenerateImports("ExtendedVariantMode", RequiredDependency = true)]
public static partial class ExtendedVariantInterop
{
    public static partial object GetCurrentVariantValue(string variantString);
    public static partial void TriggerFloatVariant(string variant, float newValue, bool revertOnDeath);
}
