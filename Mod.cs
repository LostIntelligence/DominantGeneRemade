using Mod = DominantGene.Mod;
using HarmonyLib;
using System;
using Verse;








namespace DominantGene
{

    public sealed class Mod : Verse.Mod
    {
        public static Mod Instance { get; private set; }
        public static readonly Harmony Harmony = new(nameof(DominantGene));
        public static bool Patched { get; private set; }
        public Mod(ModContentPack content) : base(content)
        {
#if DEBUG
            Harmony.DEBUG = true;
#endif
            Instance = this;
        }
        public static void TryPerformPatches()
        {
            if (Patched) return;
            try
            {
                Harmony.PatchAll();
                Patched = true;
                Log.Message($"{nameof(DominantGene)} patched successfully...");
            }
            catch (Exception ex) { Log.Error(ex + ""); }
        }
    }
}