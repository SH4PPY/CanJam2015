using UnityEngine;
using System.Collections;

public class scrCactus : MonoBehaviour
{
	const int SPINE_LAYERS = 10;
	public Material SpineMaterial;
	public bool Spherical = false;

	// Use this for initialization
	void Awake ()
	{

		for (int i = 0; i < SPINE_LAYERS; ++i)
		{
			GameObject spineLayer = (GameObject)Instantiate(transform.Find ("Flesh").gameObject, transform.position, Quaternion.identity);
			spineLayer.transform.parent = transform.parent;
			spineLayer.GetComponent<Renderer>().material = SpineMaterial;

			if (Spherical)
			{
				float scale = 1.0f + (i + 1.0f) / SPINE_LAYERS * 0.1f;
				spineLayer.transform.localScale = Vector3.one * scale;
			}
			else
			{
				float scale = 1.0f + (i + 1.0f) / SPINE_LAYERS * 0.2f;
				spineLayer.transform.localScale = new Vector3(scale, 1.0f, scale);
			}
		}

	}
}
