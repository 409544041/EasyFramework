using UniEasy.Console;
using UnityEngine;
using UniEasy.ECS;
using UniEasy;
using System;
using UniRx;

public class TestSystemBehaviour : SystemBehaviour
{
	public GameObject prefab;

	public override void Setup ()
	{
		base.Setup ();

		var group = GroupFactory.Create (new Type[] {
			typeof(EntityBehaviour),
			typeof(BoxCollider),
			typeof(Animator),
		});
			
		var isActive = new ReactiveProperty<bool> ();
		group.Entities.ObserveAdd ().Select (x => x.Value).StartWith (group.Entities).Subscribe (entity => {
			var animator = entity.GetComponent<Animator> ();
			animator.gameObject.ObserveEveryValueChanged (go => go.activeSelf).Subscribe (b => {
				isActive.Value = b;
			}).AddTo (this.Disposer);
			Debugger.Log (animator.name);
		}).AddTo (this.Disposer);

		var activeGroup = GroupFactory.AddTypes (
			                  typeof(BoxCollider), 
			                  typeof(Animator), 
			                  typeof(EntityBehaviour))
			.WithPredicate (entity => {
			return isActive;
		}).Create ();

		activeGroup.Entities.ObserveAdd ().Select (x => x.Value).StartWith (group.Entities).Subscribe (entity => {
			Debugger.Log ("added : " + entity.GetComponent<EntityBehaviour> ().name, "UniEasy");
		}).AddTo (this.Disposer);

		activeGroup.Entities.ObserveRemove ().Select (x => x.Value).Subscribe (entity => {
			Debugger.Log ("remove : " + entity.GetComponent<EntityBehaviour> ().name, "UniEasy");
		}).AddTo (this.Disposer);

		CommandLibrary.RegisterCommand (HelpCommand.name, HelpCommand.description, HelpCommand.usage, HelpCommand.Execute).AddTo (this.Disposer);
	
		PrefabFactory.Instantiate (prefab);
//		Observable.Timer (TimeSpan.FromSeconds (3)).Subscribe (_ => {
//			PrefabFactory.Instantiate (prefab);
//		}).AddTo (this.Disposer);
	}

	void Start ()
	{

	}
}
