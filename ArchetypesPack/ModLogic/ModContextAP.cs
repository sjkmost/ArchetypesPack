using TabletopTweaks.Core.ModLogic;
using static UnityModManagerNet.UnityModManager;

namespace ArchetypesPack.ModLogic {
    internal class ModContextAP : ModContextBase {
        public ModContextAP(ModEntry ModEntry) : base(ModEntry) {
            LoadAllSettings();
#if DEBUG
            Debug = true;
#endif
        }

        public override void LoadAllSettings() {
            LoadSettings("Blueprints.json", "ArchetypesPack.Config", ref Blueprints);
            LoadLocalization("ArchetypesPack.Localization");
        }
        public override void AfterBlueprintCachePatches() {
            base.AfterBlueprintCachePatches();
            if (Debug) {
                Blueprints.RemoveUnused();
                SaveSettings(BlueprintsFile, Blueprints);
                ModLocalizationPack.RemoveUnused();
                SaveLocalization(ModLocalizationPack);
            }
        }

        public virtual void Load() {
            Blueprints.Debug = Debug;
            Blueprints.Context = this;
        }
    }
}
