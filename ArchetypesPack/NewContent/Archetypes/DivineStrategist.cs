using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Classes.Selection;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.Designers.Mechanics.Facts;
using Kingmaker.ElementsSystem;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.Enums;
using Kingmaker.Localization;
using Kingmaker.ResourceLinks;
using Kingmaker.UnitLogic.Abilities;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Abilities.Components;
using Kingmaker.UnitLogic.Abilities.Components.AreaEffects;
using Kingmaker.UnitLogic.Abilities.Components.Base;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using Kingmaker.UnitLogic.Buffs.Components;
using Kingmaker.UnitLogic.Commands.Base;
using Kingmaker.UnitLogic.FactLogic;
using Kingmaker.UnitLogic.Mechanics;
using Kingmaker.UnitLogic.Mechanics.Actions;
using Kingmaker.UnitLogic.Mechanics.Components;
using Kingmaker.UnitLogic.Mechanics.Conditions;
using Kingmaker.Utility;
using Kingmaker.View.Animation;
using Kingmaker.Visual.Animation.Kingmaker.Actions;
using ArchetypesPack.NewComponents;
using TabletopTweaks.Core.Utilities;
using static ArchetypesPack.Main;
using static TabletopTweaks.Core.MechanicsChanges.AdditionalModifierDescriptors;
using Kingmaker.Designers.Mechanics.Buffs;
using TabletopTweaks.Core.NewComponents;
using Kingmaker.RuleSystem.Rules;
using Kingmaker.Blueprints.Items.Equipment;
using Kingmaker.Designers.EventConditionActionSystem.Actions;

namespace ArchetypesPack.NewContent.Archetypes {
    static class DivineStrategist {

        private static readonly BlueprintAbility Aid = BlueprintTools.GetBlueprint<BlueprintAbility>("03a9630394d10164a9410882d31572f0");
        private static readonly BlueprintAbility LuckDomainBaseAbility = BlueprintTools.GetBlueprint<BlueprintAbility>("9af0b584f6f754045a0a79293d100ab3");
        private static readonly BlueprintAbility TrueStrike = BlueprintTools.GetBlueprint<BlueprintAbility>("2c38da66e5a599347ac95b3294acbe00");

        private static readonly BlueprintBuff LuckDomainBaseBuff = BlueprintTools.GetBlueprint<BlueprintBuff>("3bc40c9cbf9a0db4b8b43d8eedf2e6ec");

        private static readonly BlueprintCharacterClass ClericClass = BlueprintTools.GetBlueprint<BlueprintCharacterClass>("67819271767a9dd4fbfd4ae700befea0");

        private static readonly BlueprintBuff HeroismBuff = BlueprintTools.GetBlueprint<BlueprintBuff>("87ab2fed7feaaff47b62a3320a57ad8d");
        private static readonly BlueprintAbilityAreaEffect GloryDomainGreaterArea = BlueprintTools.GetBlueprint<BlueprintAbilityAreaEffect>("dc623fb49e4658f43b32bed21dafc38c");
        private static readonly BlueprintAbilityAreaEffect AeonAura = BlueprintTools.GetBlueprint<BlueprintAbilityAreaEffect>("2b6a9f8c38c03b441a0473b61016473b");
        private static readonly BlueprintFeature WeaponBondAdditionalUse = BlueprintTools.GetBlueprint<BlueprintFeature>("5a64de5435667da4eae2e4c95ec87917");

        private static readonly BlueprintFeatureSelection ChannelEnergySelection = BlueprintTools.GetBlueprint<BlueprintFeatureSelection>("d332c1748445e8f4f9e92763123e31bd");
        private static readonly BlueprintFeatureSelection SecondDomainsSelection = BlueprintTools.GetBlueprint<BlueprintFeatureSelection>("43281c3d7fe18cc4d91928395837cd1e");

        public static void AddDivineStrategist() {
            var MasterTacticianBuff = Helpers.CreateBlueprint<BlueprintBuff>(APContext, "MasterTacticianBuff", bp => {
                bp.SetName(APContext, "Master Tactician");
                bp.AddComponent<AddContextStatBonus>(c => {
                    c.Descriptor = ModifierDescriptor.UntypedStackable;
                    c.Stat = StatType.Initiative;        
                    c.Value = new ContextValue() {
                        ValueType = ContextValueType.Shared,
                        ValueShared = AbilitySharedValue.DamageBonus
                    };
                });
                bp.m_Flags = BlueprintBuff.Flags.HiddenInUi;
            });

            var MasterTacticianArea = Helpers.CreateBlueprint<BlueprintAbilityAreaEffect>(APContext, "MasterTacticianArea", bp => {
                bp.AddComponent<AbilityAreaEffectBuff>(c => {
                    c.Condition = new ConditionsChecker() {
                        Conditions = new Condition[] {
                            new ContextConditionIsAlly(), 
                            new ContextConditionIsCaster() {
                                Not = true
                            }
                        }
                    };
                    c.m_Buff = MasterTacticianBuff.ToReference<BlueprintBuffReference>();
                });
                bp.AggroEnemies = false;
                bp.Shape = AreaEffectShape.Cylinder;
                bp.Size = 50.Feet();
                bp.Fx = new PrefabLink();
            });

            var MasterTacticianAreaBuff = Helpers.CreateBlueprint<BlueprintBuff>(APContext, "MasterTacticianAreaBuff", bp => {
                bp.AddComponent<AddAreaEffect>(c => {
                    c.m_AreaEffect = MasterTacticianArea.ToReference<BlueprintAbilityAreaEffectReference>();
                });
                bp.m_Flags = BlueprintBuff.Flags.HiddenInUi;
            });

            var MasterTacticianFeature = Helpers.CreateBlueprint<BlueprintFeature>(APContext, "MasterTacticianFeature", bp => {
                bp.SetName(APContext, "Master Tactician");
                bp.SetDescription(APContext, "A divine strategist gains a bonus on initiative checks equal to 1/2 her cleric level." +
                    " At 20th level, a divine strategist’s initiative roll is automatically a natural 20." +
                    " Allies able to see and hear the divine strategist gain a bonus on initiative checks equal to 1/4 the divine strategist’s level.");
                bp.AddContextRankConfig(c => {
                    c.m_Type = AbilityRankType.DamageBonus;
                    c.m_BaseValueType = ContextRankBaseValueType.ClassLevel;
                    c.m_Class = new BlueprintCharacterClassReference[] {
                        ClericClass.ToReference<BlueprintCharacterClassReference>()
                    };
                    c.m_Progression = ContextRankProgression.Div2;
                });
                bp.AddComponent<ContextCalculateSharedValue>(c => {
                    c.ValueType = AbilitySharedValue.DamageBonus;
                    c.Value = new ContextDiceValue() {
                        DiceCountValue = 0, 
                        BonusValue = new ContextValue() {
                            ValueType = ContextValueType.Rank,
                            ValueRank = AbilityRankType.DamageBonus,
                            ValueShared = AbilitySharedValue.DamageBonus
                        }
                    };
                    c.Modifier = 0.5;
                });
                bp.AddComponent<AddContextStatBonus>(c => {
                    c.Descriptor = ModifierDescriptor.UntypedStackable;
                    c.Stat = StatType.Initiative;
                    c.Value = new ContextValue() {
                        ValueType = ContextValueType.Rank,
                        ValueRank = AbilityRankType.DamageBonus
                    };
                });
                bp.AddComponent<AuraFeatureComponent>(c => {
                    c.m_Buff = MasterTacticianAreaBuff.ToReference<BlueprintBuffReference>();
                });
                bp.ReapplyOnLevelUp = true;
            });

            var MasterTacticianCapstone = Helpers.CreateBlueprint<BlueprintFeature>(APContext, "MasterTacticianCapstone", bp => {
                bp.SetName(APContext, "Master Tactician");
                bp.SetDescription(APContext, "At 20th level, a divine strategist’s initiative roll is automatically a natural 20.");
                bp.AddComponent<ModifyD20>(c => {
                    c.Rule = RuleType.Initiative;
                    c.Replace = true;
                    c.RollResult = 20;
                });
            });

            var TacticalExpertiseBuff = Helpers.CreateBlueprint<BlueprintBuff>(APContext, "TacticalExpertiseBuff", bp => {
                bp.SetName(APContext, "Tactical Expertise");
                bp.SetDescription(APContext, "At 8th level, a divine strategist knows how to take best advantage of tactical opportunities." +
                    " Once per day as a swift action she may add her Intelligence modifier as a bonus on any single d20 roll." +
                    " She can use this ability one additional time per day for every two levels after 8th.");
                bp.AddComponent<AddContextStatBonus>(c => {
                    c.Descriptor = ModifierDescriptor.UntypedStackable;
                    c.Stat = StatType.AdditionalAttackBonus;
                    c.Value = new ContextValue() {
                        ValueType = ContextValueType.Shared,
                        ValueShared = AbilitySharedValue.StatBonus
                    };
                });
                bp.AddComponent<BuffAllSkillsBonus>(c => {
                    c.Descriptor = ModifierDescriptor.UntypedStackable;
                    c.Value = 1;
                    c.Multiplier = new ContextValue() {
                        ValueType = ContextValueType.Shared,
                        ValueShared = AbilitySharedValue.StatBonus
                    };
                });
                bp.AddComponent<AddContextStatBonus>(c => {
                    c.Descriptor = ModifierDescriptor.UntypedStackable;
                    c.Stat = StatType.SaveFortitude;
                    c.Value = new ContextValue() {
                        ValueType = ContextValueType.Shared,
                        ValueShared = AbilitySharedValue.StatBonus
                    };
                });
                bp.AddComponent<AddContextStatBonus>(c => {
                    c.Descriptor = ModifierDescriptor.UntypedStackable;
                    c.Stat = StatType.SaveReflex;
                    c.Value = new ContextValue() {
                        ValueType = ContextValueType.Shared,
                        ValueShared = AbilitySharedValue.StatBonus
                    };
                });
                bp.AddComponent<AddContextStatBonus>(c => {
                    c.Descriptor = ModifierDescriptor.UntypedStackable;
                    c.Stat = StatType.SaveWill;
                    c.Value = new ContextValue() {
                        ValueType = ContextValueType.Shared,
                        ValueShared = AbilitySharedValue.StatBonus
                    };
                });
                bp.AddComponent<AddContextStatBonus>(c => {
                    c.Descriptor = ModifierDescriptor.UntypedStackable;
                    c.Stat = StatType.Initiative;
                    c.Value = new ContextValue() {
                        ValueType = ContextValueType.Shared,
                        ValueShared = AbilitySharedValue.StatBonus
                    };
                });
                bp.AddComponent<AddContextStatBonus>(c => {
                    c.Descriptor = ModifierDescriptor.UntypedStackable;
                    c.Stat = StatType.AdditionalCMB;
                    c.Value = new ContextValue() {
                        ValueType = ContextValueType.Shared,
                        ValueShared = AbilitySharedValue.StatBonus
                    };
                });
                bp.AddComponent<SpellPenetrationBonus>(c => {                    
                    c.Value = new ContextValue() {
                        ValueType = ContextValueType.Shared,
                        ValueShared = AbilitySharedValue.StatBonus
                    };
                });
                bp.AddComponent<ContextDispelBonusOnType>(c => {
                    c.Bonus = new ContextValue() {
                        ValueType = ContextValueType.Shared,
                        ValueShared = AbilitySharedValue.StatBonus
                    };
                    c.Type = RuleDispelMagic.CheckType.CasterLevel;
                });
                bp.AddComponent<ConcentrationBonus>(c => {
                    c.Value = new ContextValue() {
                        ValueType = ContextValueType.Shared,
                        ValueShared = AbilitySharedValue.StatBonus
                    };
                });
                bp.AddComponent<AddRollD20Trigger>(c => {
                    c.Action = Helpers.CreateActionList(
                        new ContextActionRemoveSelf()
                    );
                    c.FromOwner = true;
                });
                bp.m_Icon = TrueStrike.m_Icon;
                bp.FxOnStart = LuckDomainBaseBuff.FxOnStart;
                bp.ResourceAssetIds = LuckDomainBaseBuff.ResourceAssetIds;
            });

            var TacticalExpertiseResource = Helpers.CreateBlueprint<BlueprintAbilityResource>(APContext, "TacticalExpertiseResource", bp => {
                bp.m_MaxAmount = new BlueprintAbilityResource.Amount() {
                    IncreasedByLevelStartPlusDivStep = true,
                    StartingLevel = 8,
                    StartingIncrease = 1,
                    LevelStep = 2,
                    PerStepIncrease = 1,
                    m_ClassDiv = new BlueprintCharacterClassReference[] {
                        ClericClass.ToReference<BlueprintCharacterClassReference>()
                    }, 
                    ResourceBonusStat = StatType.Intelligence
                };
            });

            var TacticalExpertiseAbility = Helpers.CreateBlueprint<BlueprintAbility>(APContext, "TacticalExpertiseAbility", bp => {
                bp.SetName(APContext, "Tactical Expertise");
                bp.SetDescription(APContext, "At 8th level, a divine strategist knows how to take best advantage of tactical opportunities." +
                    " Once per day as a swift action she may add her Intelligence modifier as a bonus on any single d20 roll." +
                    " She can use this ability one additional time per day for every two levels after 8th.");
                bp.AddComponent<AbilityEffectRunAction>(c => {
                    c.Actions = Helpers.CreateActionList(
                        new ContextActionApplyBuff() {
                            m_Buff = TacticalExpertiseBuff.ToReference<BlueprintBuffReference>(),
                            DurationValue = new ContextDurationValue() {
                                DiceCountValue = 0,
                                BonusValue = 1
                            }
                        }
                    );
                });
                bp.AddContextRankConfig(c => {
                    c.m_Type = AbilityRankType.StatBonus;
                    c.m_BaseValueType = ContextRankBaseValueType.StatBonus;
                    c.m_Stat = StatType.Intelligence;
                    c.m_Progression = ContextRankProgression.AsIs;
                    c.m_UseMin = true;
                    c.m_Min = 0;
                });
                bp.AddComponent<ContextCalculateSharedValue>(c => {
                    c.ValueType = AbilitySharedValue.StatBonus;
                    c.Value = new ContextDiceValue() {
                        DiceCountValue = 0,
                        BonusValue = new ContextValue() {
                            ValueType = ContextValueType.Rank,
                            ValueRank = AbilityRankType.StatBonus,
                            ValueShared = AbilitySharedValue.StatBonus
                        }
                    };            
                });
                bp.AddComponent<AbilityResourceLogic>(c => {
                    c.m_RequiredResource = TacticalExpertiseResource.ToReference<BlueprintAbilityResourceReference>();
                    c.m_IsSpendResource = true;
                });
                bp.m_Icon = TrueStrike.m_Icon;
                bp.Type = AbilityType.Supernatural;
                bp.Range = AbilityRange.Personal;
                bp.Animation = UnitAnimationActionCastSpell.CastAnimationStyle.Self;
                bp.ActionType = UnitCommand.CommandType.Swift;
                bp.AvailableMetamagic = Metamagic.Heighten;
                bp.LocalizedDuration = LuckDomainBaseAbility.LocalizedDuration;
                bp.LocalizedSavingThrow = new LocalizedString();
                bp.ResourceAssetIds = TacticalExpertiseBuff.ResourceAssetIds;
            });

            var TacticalExpertiseFeature = Helpers.CreateBlueprint<BlueprintFeature>(APContext, "TacticalExpertiseFeature", bp => {
                bp.SetName(APContext, "Tactical Expertise");
                bp.SetDescription(APContext, "At 8th level, a divine strategist knows how to take best advantage of tactical opportunities." + 
                    " Whenever she is flanking or makes an attack of opportunity, she may add her Intelligence bonus (if any) as a bonus on the attack roll." +
                    " In addition, once per day as a swift action she may add her Intelligence modifier as a bonus on any single d20 roll." +
                    " She can use this ability one additional time per day for every two levels after 8th.");
                bp.AddContextRankConfig(c => {
                    c.m_Type = AbilityRankType.StatBonus;
                    c.m_BaseValueType = ContextRankBaseValueType.StatBonus;
                    c.m_Stat = StatType.Intelligence;
                    c.m_Progression = ContextRankProgression.AsIs;
                    c.m_UseMin = true;
                    c.m_Min = 0;
                });
                bp.AddComponent<AttackBonusConditional>(c => {
                    c.Descriptor = (ModifierDescriptor)Untyped.Intelligence;
                    c.Bonus = new ContextValue() {
                        ValueType = ContextValueType.Rank,
                        ValueRank = AbilityRankType.StatBonus
                    };
                    c.Conditions = new ConditionsChecker() {
                        Conditions = new Condition[] {
                            new ContextConditionIsFlanked()           
                        }
                    };
                });
                bp.AddComponent<AttackOfOpportunityAttackBonus>(c => {
                    c.Descriptor = (ModifierDescriptor)Untyped.Intelligence;
                    c.Bonus = new ContextValue() {
                        ValueType = ContextValueType.Rank,
                        ValueRank = AbilityRankType.StatBonus
                    };
                });
                bp.AddComponent<AddFacts>(c => {
                    c.m_Facts = new BlueprintUnitFactReference[] {
                        TacticalExpertiseAbility.ToReference<BlueprintUnitFactReference>()
                    };
                });
                bp.AddComponent<AddAbilityResources>(c => {
                    c.m_Resource = TacticalExpertiseResource.ToReference<BlueprintAbilityResourceReference>();
                    c.RestoreAmount = true;
                });
                bp.AddComponent<RecalculateOnStatChange>(c => {
                    c.Stat = StatType.Intelligence;
                });               
            });

            var CasterSupportBuffDivine = Helpers.CreateBlueprint<BlueprintBuff>(APContext, "CasterSupportBuffDivine", bp => {
                bp.SetName(APContext, "Caster Support");
                bp.SetDescription(APContext, "A divine strategist can use a standard action to assist another divine spellcaster, granting a +2 circumstance bonus on caster level checks and concentration checks until the beginning of the divine strategist’s next turn." +
                    " This bonus increases by +1 at 4th level and every four levels thereafter (to a maximum of +7 at 20th level)." +
                    " Caster support can be used to assist arcane spellcasters or characters using magical items, but they gain only half the normal bonus.");
                bp.AddComponent<IncreaseCasterLevel>(c => {
                    c.Value = new ContextValue() {
                        ValueType = ContextValueType.Shared,
                        ValueShared = AbilitySharedValue.StatBonus
                    };
                    c.Descriptor = ModifierDescriptor.Circumstance;
                });
                bp.AddComponent<ContextItemCasterLevelBonus>(c => {
                    c.Bonus = new ContextValue() {
                        ValueType = ContextValueType.Shared,
                        ValueShared = AbilitySharedValue.DamageBonus
                    };
                    c.ItemType = UsableItemType.Wand;
                });
                bp.AddComponent<ContextItemCasterLevelBonus>(c => {
                    c.Bonus = new ContextValue() {
                        ValueType = ContextValueType.Shared,
                        ValueShared = AbilitySharedValue.DamageBonus
                    };
                    c.ItemType = UsableItemType.Scroll;
                });
                bp.m_Icon = Aid.m_Icon;
            });

            var CasterSupportBuffArcane = Helpers.CreateBlueprint<BlueprintBuff>(APContext, "CasterSupportBuffArcane", bp => {
                bp.SetName(APContext, "Caster Support");
                bp.SetDescription(APContext, "A divine strategist can use a standard action to assist another divine spellcaster, granting a +2 circumstance bonus on caster level checks and concentration checks until the beginning of the divine strategist’s next turn." +
                    " This bonus increases by +1 at 4th level and every four levels thereafter (to a maximum of +7 at 20th level)." +
                    " Caster support can be used to assist arcane spellcasters or characters using magical items, but they gain only half the normal bonus.");
                bp.AddComponent<IncreaseCasterLevel>(c => {
                    c.Value = new ContextValue() {
                        ValueType = ContextValueType.Shared,
                        ValueShared = AbilitySharedValue.DamageBonus
                    };
                    c.Descriptor = ModifierDescriptor.Circumstance;
                });
                bp.AddComponent<ContextItemCasterLevelBonus>(c => {
                    c.Bonus = new ContextValue() {
                        ValueType = ContextValueType.Shared,
                        ValueShared = AbilitySharedValue.DamageBonus
                    };
                    c.ItemType = UsableItemType.Wand;
                });
                bp.AddComponent<ContextItemCasterLevelBonus>(c => {
                    c.Bonus = new ContextValue() {
                        ValueType = ContextValueType.Shared,
                        ValueShared = AbilitySharedValue.DamageBonus
                    };
                    c.ItemType = UsableItemType.Scroll;
                });
                bp.m_Icon = Aid.m_Icon;
            });

            var CasterSupportAbility = Helpers.CreateBlueprint<BlueprintAbility>(APContext, "CasterSupportAbility", bp => {
                bp.SetName(APContext, "Caster Support");
                bp.SetDescription(APContext, "A divine strategist can use a standard action to assist another divine spellcaster, granting a +2 circumstance bonus on caster level checks and concentration checks until the beginning of the divine strategist’s next turn." +
                    " This bonus increases by +1 at 4th level and every four levels thereafter (to a maximum of +7 at 20th level)." +
                    " Caster support can be used to assist arcane spellcasters or characters using magical items, but they gain only half the normal bonus.");
                bp.AddComponent<AbilityEffectRunAction>(c => {
                    c.Actions = Helpers.CreateActionList(
                        new Conditional() {
                            ConditionsChecker = new ConditionsChecker() {
                                Conditions = new Condition[] {
                                    new ContextConditionTargetIsDivineCaster()
                                }
                            }, 
                            IfTrue = Helpers.CreateActionList(
                                new ContextActionApplyBuff() {
                                    m_Buff = CasterSupportBuffDivine.ToReference<BlueprintBuffReference>(),
                                    DurationValue = new ContextDurationValue() {
                                        DiceCountValue = 0,
                                        BonusValue = 1
                                    }
                                }
                            ), 
                            IfFalse = Helpers.CreateActionList(
                                new ContextActionApplyBuff() {
                                    m_Buff = CasterSupportBuffArcane.ToReference<BlueprintBuffReference>(),
                                    DurationValue = new ContextDurationValue() {
                                        DiceCountValue = 0,
                                        BonusValue = 1
                                    }
                                }
                            ),
                        }
                    );
                });
                bp.AddContextRankConfig(c => {
                    c.m_Type = AbilityRankType.StatBonus;
                    c.m_BaseValueType = ContextRankBaseValueType.ClassLevel;
                    c.m_Class = new BlueprintCharacterClassReference[] {
                        ClericClass.ToReference<BlueprintCharacterClassReference>()
                    };
                    c.m_Progression = ContextRankProgression.StartPlusDivStep;
                    c.m_StartLevel = -4;
                    c.m_StepLevel = 4;
                });
                bp.AddComponent<ContextCalculateSharedValue>(c => {
                    c.ValueType = AbilitySharedValue.StatBonus;
                    c.Value = new ContextDiceValue() {
                        DiceCountValue = 0,
                        BonusValue = new ContextValue() {
                            ValueType = ContextValueType.Rank,
                            ValueRank = AbilityRankType.StatBonus,
                            ValueShared = AbilitySharedValue.StatBonus
                        }
                    };
                });
                bp.AddComponent<ContextCalculateSharedValue>(c => {
                    c.ValueType = AbilitySharedValue.StatBonus;
                    c.Value = new ContextDiceValue() {
                        DiceCountValue = 0,
                        BonusValue = new ContextValue() {
                            ValueType = ContextValueType.Rank,
                            ValueRank = AbilityRankType.StatBonus,
                            ValueShared = AbilitySharedValue.DamageBonus
                        }
                    };
                    c.Modifier = 0.5;
                });
                bp.AddComponent<AbilitySpawnFx>(c => {
                    c.PrefabLink = new PrefabLink() {
                        AssetId = "34306172e3b0f33478ac715ca9ac7b4a"
                    };
                    c.Anchor = AbilitySpawnFxAnchor.SelectedTarget;
                });
                bp.m_Icon = Aid.m_Icon;
                bp.Type = AbilityType.Supernatural;
                bp.Range = AbilityRange.Touch;
                bp.CanTargetFriends = true;
                bp.CanTargetSelf = false;
                bp.Animation = UnitAnimationActionCastSpell.CastAnimationStyle.Touch;
                bp.ActionType = UnitCommand.CommandType.Standard;
                bp.AvailableMetamagic = Metamagic.Quicken | Metamagic.Extend | Metamagic.Heighten | Metamagic.Reach | Metamagic.CompletelyNormal;
                bp.LocalizedDuration = LuckDomainBaseAbility.LocalizedDuration;
                bp.LocalizedSavingThrow = new LocalizedString();
                bp.ResourceAssetIds = new string[] {
                    "34306172e3b0f33478ac715ca9ac7b4a"
                };
            });

            var CasterSupportFeature = Helpers.CreateBlueprint<BlueprintFeature>(APContext, "CasterSupportFeature", bp => {
                bp.SetName(APContext, "Caster Support");
                bp.SetDescription(APContext, "A divine strategist can use a standard action to assist another divine spellcaster, granting a +2 circumstance bonus on caster level checks and concentration checks until the beginning of the divine strategist’s next turn." +
                    " This bonus increases by +1 at 4th level and every four levels thereafter (to a maximum of +7 at 20th level)." +
                    " Caster support can be used to assist arcane spellcasters or characters using magical items, but they gain only half the normal bonus.");
                bp.AddComponent<AddFacts>(c => {
                    c.m_Facts = new BlueprintUnitFactReference[] {
                        CasterSupportAbility.ToReference<BlueprintUnitFactReference>()
                    };
                });
            });

            var DivineStrategistArchetype = Helpers.CreateBlueprint<BlueprintArchetype>(APContext, "DivineStrategistArchetype", bp => {
                bp.SetName(APContext, "Divine Strategist");
                bp.SetDescription(APContext, "The divine strategist leads the armies of the faithful, not from the front lines but through her clever strategy and tactical acumen.");
                bp.RemoveFeatures = new LevelEntry[] {
                    Helpers.CreateLevelEntry(1, ChannelEnergySelection),
                    Helpers.CreateLevelEntry(1, SecondDomainsSelection)                
                };
                bp.AddFeatures = new LevelEntry[] {
                    Helpers.CreateLevelEntry(1, MasterTacticianFeature),
                    Helpers.CreateLevelEntry(1, CasterSupportFeature), 
                    Helpers.CreateLevelEntry(8, TacticalExpertiseFeature),
                    Helpers.CreateLevelEntry(20, MasterTacticianCapstone)
                };
            });

            ClericClass.m_Archetypes = ClericClass.m_Archetypes.AppendToArray(DivineStrategistArchetype.ToReference<BlueprintArchetypeReference>());

            APContext.Logger.LogPatch("Added", DivineStrategistArchetype);
        }
    }
}
