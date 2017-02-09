using UnityEngine;

namespace App.Utils
{
	// I got so sick of forgetting to numerically shift the layer numbers...
    public static class LayersMask
	{
		public static int Make(params string[] names)
		{
			int mask = 0;
			foreach (var name in names)
			{
				mask |= 1 << LayerMask.NameToLayer(name);
			}
			return mask;
		}
	}
}

