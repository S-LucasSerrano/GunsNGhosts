using UnityEngine;

/// <summary> Extensions of the LayerMask class. </summary>
public static class LayerMaskExtensions
{
	/// <summary> Check if this LayerMask contains the given layer. </summary>
	/// https://andreberlemont.com/2016-09-20
	public static bool ContainsLayer(this LayerMask thisLayerMask, int layer)
	{
		LayerMask layerMask = 1 << layer;
		return thisLayerMask.ContainsLayer( layerMask );
	}

	/// <summary> Check if this LayerMask has at least one layer matching with other LayerMask. </summary>
	public static bool ContainsLayer(this LayerMask thisLayerMask, LayerMask layerMask)
	{
		return (thisLayerMask & layerMask) > 0;
	}

}
