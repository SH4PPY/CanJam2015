using UnityEngine;
using System.Collections;

public class scrConsumable : MonoBehaviour
{
	public float Food;
	public float Water;
	public GameObject Evidence;

	public GameObject TakeEvidence()
	{
		Debug.Log ("Evidence Taken");

		GameObject e = Evidence;
		if (e != null)
		{
			e.transform.SetParent (null);
			Evidence = null;
		}

		return e;
	}
}
