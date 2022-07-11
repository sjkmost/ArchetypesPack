using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Classes.Prerequisites;
using Kingmaker.Blueprints.Classes.Selection;
using Kingmaker.Designers.EventConditionActionSystem.Actions;
using Kingmaker.Designers.Mechanics.Facts;
using Kingmaker.ElementsSystem;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.Enums;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Abilities.Components;
using Kingmaker.UnitLogic.Alignments;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using Kingmaker.UnitLogic.Commands.Base;
using Kingmaker.UnitLogic.FactLogic;
using Kingmaker.UnitLogic.Mechanics;
using Kingmaker.UnitLogic.Mechanics.Actions;
using Kingmaker.UnitLogic.Mechanics.Components;
using Kingmaker.UnitLogic.Mechanics.Conditions;
using Kingmaker.UnitLogic.Mechanics.Properties;
using Kingmaker.Utility;
using Kingmaker.Visual.Animation.Kingmaker.Actions;
using System.Collections.Generic;
using System.Linq;
using TabletopTweaks.Core.NewComponents;
using TabletopTweaks.Core.Utilities;
using static ArchetypesPack.Main;

namespace ArchetypesPack.NewContent.Archetypes {
    static class HolyTactician {
        private static readonly BlueprintCharacterClass PaladinClass = BlueprintTools.GetBlueprint<BlueprintCharacterClass>("bfa11238e7ae3544bbeb4d0b92e897ec");

        public static void AddHolyTactician() {
            var HolyTacticianWealsChampionAbility = Helpers.CreateBlueprint<BlueprintAbility>(APContext, "HolyTacticianWealsChampionAbility", bp => {
                bp.AddContextRankConfig(c => {
                    c.m_Type = AbilityRankType.Default;
                    c.m_BaseValueType = ContextRankBaseValueType.ClassLevel;
                    c.m_Class = new BlueprintCharacterClassReference[] { 
                        PaladinClass.ToReference<BlueprintCharacterClassReference>() 
                    };
                    c.m_Progression = ContextRankProgression.Div2;
                    c.m_Min = 1;
                    c.m_UseMin = true;
                });
                bp.AddComponent<AbilityEffectRunAction>(c => {
                    c.Actions = Helpers.CreateActionList(
                        new Conditional() {
                            ConditionsChecker = new ConditionsChecker() {
                                Conditions = new Condition[] {
                                    new ContextConditionAlignment() {
                                        Alignment = AlignmentComponent.Evil
                                    }
                                }
                            }
                        }               
                    );
                });
            });
        }
    }
}
