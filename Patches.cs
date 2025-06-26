using static DominantGene.Extensions;
using HarmonyLib;
using RimWorld;
using Verse;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;





namespace DominantGene
{

    [StaticConstructorOnStartup, Harmony]
    public static class Patches
    {
        private static int parentID;
        static Patches() { Mod.TryPerformPatches(); }

        [HarmonyPatch]
        public static class GetInheritedGenes
        {

            public static MethodBase TargetMethod() => typeof(PregnancyUtility).GetMethods().Where(x => x.Name == nameof(GetInheritedGenes)).MaxBy(x => x.GetParameters().Length);
            public static void Postfix(ref List<GeneDef> __result, Pawn father, Pawn mother)
            {
                InheritGenes inherit = null;
                var random = new System.Random();
                if (CanInheritParentDominantGenes(father, ref inherit) & CanInheritParentDominantGenes(mother, ref inherit))
                {
                    __result.Clear();
                    inherit = null;
                    int cid;
                    cid = random.Next(0, 2);
                    Log.Message("GENE varID: " + cid);
                    parentID = cid;

                    if (parentID == 0)
                    {
                        CanInheritParentDominantGenes(mother, ref inherit);
                    }
                    else
                    {
                        CanInheritParentDominantGenes(father, ref inherit);
                    }
                }
                if (inherit is null) return;
                __result.Clear();
                inherit?.Invoke(__result);
            }
        }

        [HarmonyPatch(typeof(PregnancyUtility), nameof(TryGetInheritedXenotype))]
        [HarmonyPostfix]
        public static void TryGetInheritedXenotype(ref bool __result, Pawn mother, Pawn father, ref XenotypeDef xenotype)
        {
            dominantParent = null;
            InheritXenotype inherit = null;
            if (CanInheritParentDominantXenotype(mother, ref inherit) & CanInheritParentDominantXenotype(father, ref inherit))
            {
                inherit = null;
                Log.Message("XENO varID: " + parentID);
                if (parentID == 0)
                {

                    CanInheritParentDominantXenotype(mother, ref inherit);
                }
                else
                {
                    CanInheritParentDominantXenotype(father, ref inherit);
                }
            }
            if (inherit is null) return;
            inherit?.Invoke(ref xenotype);
            __result = true;
        }

        [HarmonyPatch(typeof(Pawn_GeneTracker), nameof(SetXenotypeDirect))]
        [HarmonyPostfix]
        public static void SetXenotypeDirect(Pawn_GeneTracker __instance, ref XenotypeDef xenotype)
        {
            if (dominantParent is null) return;
            __instance.iconDef = dominantParent.genes.iconDef;
            __instance.xenotypeName = dominantParent.genes.xenotypeName;
        }
    }
}
