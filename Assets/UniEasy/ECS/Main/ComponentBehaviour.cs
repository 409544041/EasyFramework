using UnityEngine;

namespace UniEasy.ECS
{
	public class ComponentBehaviour : MonoBehaviour, IBehaviour
	{
		protected virtual void Awake ()
		{
			this.InjectSelf ();
		}

		void Start ()
		{

		}
	}
}