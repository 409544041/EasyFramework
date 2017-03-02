using UnityEngine;

namespace UniEasy.ECS
{
	public class SystemBehaviour : MonoBehaviour, IBehaviour
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
