using HarmonyLib;
using Kingmaker.Blueprints.JsonSystem;
using static ArchetypesPack.Main;

namespace ArchetypesPack.Fixes {
    static class FixesAdder {
        [HarmonyPatch(typeof(BlueprintsCache), "Init")]
        static class BlueprintsCache_Init_Patch {
            static bool Initialized;
         
            static void Postfix() {
                if (Initialized) {
                    return;
                }

                Initialized = true;
                APContext.Logger.LogHeader("Loading Fixes");

                Classes.Paladin.FixAll();
            }
        }
    }
}
