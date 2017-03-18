using UnityEngine;
using UniEasy.ECS;
using UniEasy;
using System;
using UniRx;

public class TestSystemBehaviour : SystemBehaviour
{
	protected override void Awake ()
	{
		base.Awake ();
	}

	void Start ()
	{
		var group = GroupFactory.Create (new Type[] {
			typeof(BoxCollider),
			typeof(Animator),
		});

		group.Entities.ObserveAdd ().Select (x => x.Value).StartWith (group.Entities).Subscribe (entity => {
			var animator = entity.GetComponent<Animator> ();
			Debugger.Log (animator.name, "UniEasy");
		}).AddTo (this.Disposer);
	}
}