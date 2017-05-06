using UnityEngine;
using UniEasy.ECS;
using UniEasy;
using System;
using UniRx;

public class TestSystemBehaviour : SystemBehaviour
{
	public override void Setup ()
	{
		base.Setup ();

		var group = GroupFactory.CreateAsSingle (new Type[] {
			typeof(BoxCollider),
			typeof(Animator),
		});

		group.Entities.ObserveAdd ().Select (x => x.Value).StartWith (group.Entities).Subscribe (entity => {
			var animator = entity.GetComponent<Animator> ();
			Debugger.Log (animator.name);
		}).AddTo (this.Disposer);

		group.GetEntities (true).ObserveAdd ().Select (x => x.Value).StartWith (group.Entities).Subscribe (entity => {
			Debugger.Log ("added : " + entity.GetComponent<EntityBehaviour> ().name, "UniEasy");
		}).AddTo (this.Disposer);

		group.GetEntities (true).ObserveRemove ().Select (x => x.Value).Subscribe (entity => {
			Debugger.Log ("remove : " + entity.GetComponent<EntityBehaviour> ().name, "UniEasy");
		}).AddTo (this.Disposer);
	}

	void Start ()
	{

	}
}
