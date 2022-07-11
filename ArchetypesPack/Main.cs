using HarmonyLib;
using ArchetypesPack.ModLogic;
using TabletopTweaks.Core.Utilities;
using UnityModManagerNet;

namespace ArchetypesPack {
    static class Main {
        public static bool Enabled;
        public static ModContextAP APContext;

        static bool Load(UnityModManager.ModEntry modEntry) {
            var harmony = new Harmony(modEntry.Info.Id);

            APContext = new ModContextAP(modEntry);
            APContext.Load();
            APContext.ModEntry.OnSaveGUI = OnSaveGUI;
            harmony.PatchAll();
            PostPatchInitializer.Initialize(APContext);
            return true;
        }

        private static void OnSaveGUI(UnityModManager.ModEntry modEntry) {
        }
    }
}