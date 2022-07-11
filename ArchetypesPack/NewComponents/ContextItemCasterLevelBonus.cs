using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Facts;
using Kingmaker.Blueprints.Items.Equipment;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Mechanics;
using Kingmaker.UnitLogic.Parts;

namespace ArchetypesPack.NewComponents {
    [AllowedOn(typeof(BlueprintUnitFact), false)]
	[AllowMultipleComponents]
	[TypeId("31323ecc02de4392bb2bde43d3c3baaf")]
    public class ContextItemCasterLevelBonus : UnitFactComponentDelegate {
		public override void OnTurnOn() {
			Owner.Ensure<UnitPartItemCasterLevelBonus>().AddEntry(Bonus.Calculate(Context), ItemType, Fact);
		}

		public override void OnTurnOff() {
			UnitPartItemCasterLevelBonus unitPartItemCasterLevelBonus = Owner.Get<UnitPartItemCasterLevelBonus>();
			if (unitPartItemCasterLevelBonus == null) {
				return;
			}
			unitPartItemCasterLevelBonus.RemoveEntry(Fact);
		}

		public ContextValue Bonus;
		public UsableItemType ItemType;
	}
}
