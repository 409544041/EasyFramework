using UnityEngine;
using UniEasy;

public class DIExample : MonoBehaviour
{
	[Inject]
	public IInjectTest injectTest;

	void Start ()
	{
		DiContainer container = new DiContainer ();
		container.Inject (this);
		container.Bind<IInjectTest> ().To<InjectTest> ().FromInstance (new InjectTest ());

		Debug.Log ("injectTest isEmpty ? " + injectTest);
	}
}

public interface IInjectTest
{
	
}

public class InjectTest : IInjectTest
{
	
}
