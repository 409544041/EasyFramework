using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UniEasy;

public class DIExample : MonoBehaviour
{
	[Inject (Id = "first")]
	public string primitiveTestFirst;
	[Inject (Id = "second")]
	public string primitiveTestSecond;
	[Inject (Id = "foo")]
	public IFoo foo1;
	[Inject]
	public IFoo foo2;
	[Inject]
	public List<IFoo> foos;

	IEnumerator Start ()
	{
		DiContainer container = new DiContainer ();
		container.Inject (this);
		container.Bind<string> ().WithId ("first").FromInstance ("first output!")
			.AsSingle ().WhenInjectedInto<TestWhenInject> ();
		container.Bind<string> ().WithId ("second").FromInstance ("second output!").AsSingle ();
		container.Bind<IFoo> ().WithId ("foo").To<Foo1> ().FromInstance (new Foo1 ()).AsSingle ();
		container.Bind<IFoo> ().To<Foo2> ().FromInstance (new Foo2 ()).AsSingle ();
		var foos = new List<Foo3> () { new Foo3 (), new Foo3 (), new Foo3 () };
		container.Bind<List<IFoo>> ().To (typeof(List<Foo3>)).FromInstance (foos);

		Debug.Log ("primitiveTestFirst : " + primitiveTestFirst);
		Debug.Log ("primitiveTestSecond : " + primitiveTestSecond);
		Debug.Log ("foo1 : " + foo1);
		Debug.Log ("foo2 : " + foo2);
		Debug.Log ("foos : " + foos.Count);
		foreach (IFoo sub in foos) {
			Debug.Log ("foo sub : " + sub);
		}

		yield return null;
	}
}

public interface IFoo
{
	
}

public class Foo1 : IFoo
{
	
}

public class Foo2 : IFoo
{

}

public class Foo3 : IFoo
{

}

public class TestWhenInject
{
	
}
