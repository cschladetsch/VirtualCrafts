
using UnityEngine;

namespace App
{
    public abstract class ForceProviderBehaviour : MonoBehaviour 
	{
		public virtual AppliedForce Force { get; }

		public AnimationCurve RpmRelative;
	}
}

