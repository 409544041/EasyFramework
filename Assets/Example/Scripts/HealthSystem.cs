using UniEasy.ECS;
using System;
using UniRx;

public class HealthSystem : SystemBehaviour
{
	public override void Setup ()
	{
		base.Setup ();

		var group = GroupFactory.Create (typeof(HealthComponent));
		group.Entities.ObserveAdd ().Select (x => x.Value).StartWith (group.Entities).Subscribe (entity => {
			var healthComponent = entity.GetComponent<HealthComponent> ();
			
			healthComponent.CurrentHealth = healthComponent.StartingHealth;
		}).AddTo (this.Disposer);
	}
}
