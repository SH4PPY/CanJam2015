using UnityEngine;
using System.Collections;

public class scrCompass : MonoBehaviour
{
	public GameObject Little, Big;

	void Update()
	{
		Vector3 direction = GameObject.Find ("Station").transform.position - scrPlayer.Instance.transform.position;
		float angle = Vector2.Angle(new Vector2(scrPlayer.Instance.Body.forward.x, scrPlayer.Instance.Body.forward.z),
		                            new Vector2(direction.x, direction.z));

		angle += direction.magnitude * 0.05f * Mathf.Sin (Time.time);

		if (Vector2.Dot(new Vector2(scrPlayer.Instance.Body.right.x, scrPlayer.Instance.Body.right.z),
		                                 new Vector2(direction.x, direction.z)) > 0.0f)
		{
			angle = 360 - angle;
		}

		Big.transform.localEulerAngles  = new Vector3(0, Mathf.LerpAngle(Big.transform.localEulerAngles.y, angle, 0.2f), 0);
		
		Little.transform.Rotate (0, Time.deltaTime * 5, 0, Space.Self);
	}
}