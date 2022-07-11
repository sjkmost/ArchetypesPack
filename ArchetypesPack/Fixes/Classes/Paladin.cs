using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Classes.Prerequisites;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using Kingmaker.UnitLogic.FactLogic;
using Kingmaker.UnitLogic.ActivatableAbilities;
using Kingmaker.UnitLogic.Mechanics.Components;
using TabletopTweaks.Core.Utilities;
using static ArchetypesPack.Main;

namespace ArchetypesPack.Fixes.Classes {
    static class Paladin {
        public static void FixAll() {
            FixDivineHunter();
        }

        private static readonly BlueprintArchetype DivineHunterArchetype = BlueprintTools.GetBlueprint<BlueprintArchetype>("fec08c1a3187da549abd6b85f27e4432");

        private static readonly BlueprintFeature DivineHunterBondPlus2 = BlueprintTools.GetBlueprint<BlueprintFeature>("c5b169a910918e540a5494a1f24bd6ee");
        private static readonly BlueprintFeature WeaponBondAdditionalUse = BlueprintTools.GetBlueprint<BlueprintFeature>("5a64de5435667da4eae2e4c95ec87917");

        private static readonly BlueprintProgression PaladinProgression = BlueprintTools.GetBlueprint<BlueprintProgression>("fd325cbba872e5f40b618970678db002");

        private static readonly BlueprintActivatableAbility WeaponBondFlamingBurstChoice = BlueprintTools.GetBlueprint<BlueprintActivatableAbility>("3af19bdbd6215434f8421a860cc98363");
        private static readonly BlueprintActivatableAbility WeaponBondSpeedChoice = BlueprintTools.GetBlueprint<BlueprintActivatableAbility>("ed1ef581af9d9014fa1386216b31cdae");
        private static readonly BlueprintActivatableAbility WeaponBondBrillantEnergyChoice = BlueprintTools.GetBlueprint<BlueprintActivatableAbility>("f1eec5cc68099384cbfc6964049b24fa");

        public static void FixDivineHunter() {
            var DivineHunterBondPlus3 = Helpers.CreateBlueprint<BlueprintFeature>(APContext, "DivineHunterBondPlus3", bp => {
                bp.m_DisplayName = DivineHunterBondPlus2.m_DisplayName;
                bp.m_Description = DivineHunterBondPlus2.m_Description;
                bp.m_Icon = DivineHunterBondPlus2.m_Icon;
                bp.IsClassFeature = true;
                bp.AddComponent<IncreaseActivatableAbilityGroupSize>(c => {
                    c.Group = ActivatableAbilityGroup.DivineWeaponProperty;
                });
                bp.AddComponent<AddFacts>(c => {
                    c.m_Facts = new BlueprintUnitFactReference[] {
                        WeaponBondSpeedChoice.ToReference<BlueprintUnitFactReference>()
                    };
                });      
            });

            var DivineHunterBondPlus4 = Helpers.CreateBlueprint<BlueprintFeature>(APContext, "DivineHunterBondPlus4", bp => {
                bp.m_DisplayName = DivineHunterBondPlus2.m_DisplayName;
                bp.m_Description = DivineHunterBondPlus2.m_Description;
                bp.m_Icon = DivineHunterBondPlus2.m_Icon;
                bp.IsClassFeature = true;
                bp.AddComponent<IncreaseActivatableAbilityGroupSize>(c => {
                    c.Group = ActivatableAbilityGroup.DivineWeaponProperty;
                });
                bp.AddComponent<AddFacts>(c => {
                    c.m_Facts = new BlueprintUnitFactReference[] {
                        WeaponBondBrillantEnergyChoice.ToReference<BlueprintUnitFactReference>()
                    };
                });
            });

            var DivineHunterBondPlus5 = Helpers.CreateBlueprint<BlueprintFeature>(APContext, "DivineHunterBondPlus5", bp => {
                bp.m_DisplayName = DivineHunterBondPlus2.m_DisplayName;
                bp.m_Description = DivineHunterBondPlus2.m_Description;
                bp.m_Icon = DivineHunterBondPlus2.m_Icon;
                bp.IsClassFeature = true;
                bp.AddComponent<IncreaseActivatableAbilityGroupSize>(c => {
                    c.Group = ActivatableAbilityGroup.DivineWeaponProperty;
                });
            });

            var DivineHunterBondPlus6 = Helpers.CreateBlueprint<BlueprintFeature>(APContext, "DivineHunterBondPlus6", bp => {
                bp.m_DisplayName = DivineHunterBondPlus2.m_DisplayName;
                bp.m_Description = DivineHunterBondPlus2.m_Description;
                bp.m_Icon = DivineHunterBondPlus2.m_Icon;
                bp.IsClassFeature = true;
                bp.AddComponent<IncreaseActivatableAbilityGroupSize>(c => {
                    c.Group = ActivatableAbilityGroup.DivineWeaponProperty;
                });
            });

            DivineHunterArchetype.AddFeatures = DivineHunterArchetype.AddFeatures.AppendToArray(new LevelEntry[] {
                Helpers.CreateLevelEntry(9, WeaponBondAdditionalUse),
                Helpers.CreateLevelEntry(11, DivineHunterBondPlus3),
                Helpers.CreateLevelEntry(13, WeaponBondAdditionalUse),
                Helpers.CreateLevelEntry(14, DivineHunterBondPlus4), 
                Helpers.CreateLevelEntry(17, DivineHunterBondPlus5, WeaponBondAdditionalUse), 
                Helpers.CreateLevelEntry(20, DivineHunterBondPlus6)
            });
            PaladinProgression.UIGroups[0].m_Features.AddRange(new BlueprintFeatureBaseReference[] {
                DivineHunterBondPlus3.ToReference<BlueprintFeatureBaseReference>(),
                DivineHunterBondPlus4.ToReference<BlueprintFeatureBaseReference>(), 
                DivineHunterBondPlus5.ToReference<BlueprintFeatureBaseReference>(),
                DivineHunterBondPlus6.ToReference<BlueprintFeatureBaseReference>()
            });

            var DivineHunterBondPlus2AddFacts = DivineHunterBondPlus2.GetComponent<AddFacts>();

            DivineHunterBondPlus2AddFacts.m_Facts = DivineHunterBondPlus2AddFacts.m_Facts.AppendToArray(WeaponBondFlamingBurstChoice.ToReference<BlueprintUnitFactReference>());

            APContext.Logger.LogPatch("Patched", DivineHunterArchetype);
        }
    }
}
