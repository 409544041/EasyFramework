using UnityEngine;
using System.Collections;
using UniEasy;

public class DIExample : MonoBehaviour
{
	[Inject]
	public string primitiveTestFirst;
	[Inject]
	public string primitiveTestSecond;
	[Inject]
	public IInjectTest injectTest;

	IEnumerator Start ()
	{
		DiContainer container = new DiContainer ();
		container.Inject (this);
		container.Bind<string> ().FromInstance ("first output!").AsSingle ();
		container.Bind<string> ().FromInstance ("second output!").AsSingle ();
		container.Bind<IInjectTest> ().To<InjectTest> ().FromInstance (new InjectTest ());

		Debug.Log ("primitiveTestFirst isEmpty ? " + primitiveTestFirst);
		Debug.Log ("primitiveTestSecond isEmpty ? " + primitiveTestSecond);
		Debug.Log ("injectTest isEmpty ? " + injectTest);

		yield return null;
	}
}

public interface IInjectTest
{
	
}

public class InjectTest : IInjectTest
{
	
}
