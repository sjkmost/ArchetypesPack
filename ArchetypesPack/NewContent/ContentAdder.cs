using HarmonyLib;
using Kingmaker.Blueprints.JsonSystem;
using static ArchetypesPack.Main;

namespace ArchetypesPack.NewContent {
    static class ContentAdder {
        [HarmonyPatch(typeof(BlueprintsCache), "Init")]
        static class BlueprintsCache_Init_Patch {
            static bool Initialized;
            
            [HarmonyPriority(Priority.First)]
            static void Postfix() {
                if (Initialized) {
                    return;
                }

                Initialized = true;
                APContext.Logger.LogHeader("Loading New Content");

                AddArchetypes();
            }

            static void AddArchetypes() {
                Archetypes.DivineStrategist.AddDivineStrategist();
                // Archetypes.HolyTactician.AddHolyTactician();
            }
        }
    }
}
