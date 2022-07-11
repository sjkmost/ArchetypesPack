using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Facts;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.ElementsSystem;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.Enums;
using Kingmaker.PubSubSystem;
using Kingmaker.RuleSystem;
using Kingmaker.RuleSystem.Rules;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Mechanics;

namespace ArchetypesPack.NewComponents {
    [AllowedOn(typeof(BlueprintUnitFact), false)]
    [TypeId("8f3b046cc1194125a4da8eb35f9d967e")]
    public class AddRollD20Trigger : UnitFactComponentDelegate,
        IInitiatorRulebookHandler<RuleRollD20>,
        IRulebookHandler<RuleRollD20>,
        ISubscriber, IInitiatorRulebookSubscriber {
        
        public void OnEventAboutToTrigger(RuleRollD20 evt) {
        }

        public void OnEventDidTrigger(RuleRollD20 evt) {
            if (Fact != null && Fact.MaybeContext != null && (!FromOwner || evt.Initiator == Owner)) {
                Fact.RunActionInContext(Action, Owner);
            }
        }
       
        public ActionList Action;
        public bool FromOwner;
    }
}
