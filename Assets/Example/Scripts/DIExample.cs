﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UniEasy.DI;

public class DIExample : MonoInstaller<DIExample>
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
	[Inject]
	public Greeter greeter;

	public override void InstallBindings ()
	{
		Container.Bind<string> ().WithId ("first").FromInstance ("first output!")
			.AsSingle ().WhenInjectedInto<DIExample> ();
		Container.Bind<string> ().WithId ("second").FromInstance ("second output!").AsSingle ();
		Container.Bind<IFoo> ().WithId ("foo").To<Foo1> ().FromInstance (new Foo1 ()).AsSingle ();
		Container.Bind<IFoo> ().To<Foo2> ().FromInstance (new Foo2 ()).AsSingle ();
		var foos = new List<Foo3> () { new Foo3 (), new Foo3 (), new Foo3 () };
		Container.Bind<List<IFoo>> ().To (typeof(List<Foo3>)).FromInstance (foos);

		Container.Bind<string> ().FromInstance ("Hello World!");
		Container.Bind<Greeter> ().AsSingle ().NonLazy ();

		Container.Inject (this);

		Debug.Log ("primitiveTestFirst : " + primitiveTestFirst);
		Debug.Log ("primitiveTestSecond : " + primitiveTestSecond);
		Debug.Log ("foo1 : " + foo1);
		Debug.Log ("foo2 : " + foo2);
		Debug.Log ("foos : " + foos.Count);
		foreach (IFoo sub in foos) {
			Debug.Log ("foo sub : " + sub);
		}
		Debug.Log ("has binding IFoo : " + Container.HasBinding<IFoo> ());

		Container.Bind<IBar> ().To<Bar1> ().FromInstance (new Bar1 ()).WhenInjectedInto<Foo> ();
		var foo = Container.Instantiate<Foo> (true);
		Debug.Log ("Test Constructor Inject Function : " + foo._bar);

		Debug.Log ("Test Nonlazy Function : " + greeter);
	}
}

public class Greeter
{
	public Greeter (string message)
	{
		Debug.Log (message);
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

public interface IBar
{
}

public class Bar1 : IBar
{
}

public class Foo
{
	public IBar _bar;

	public Foo (IBar bar)
	{
		_bar = bar;
	}

	//	[Inject]
	public void Init (IBar bar)
	{
		_bar = bar;
	}
}
