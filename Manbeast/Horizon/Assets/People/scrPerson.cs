using UnityEngine;
using System.Collections;

public class scrPerson : MonoBehaviour
{
	public Terrain Landscape;

	public Transform Body, Head;
	protected Vector3 look;
	protected Vector2 direction;

	public float Health { get; protected set; }
	public float Hunger;
	public float Thirst;
	public float MinSanics, MaxSanics;
	public float Sanics { get; private set; }

	protected Color colour;

	public bool Ded = false;

	protected virtual void Awake()
	{
		Health = 4000 + Random.Range (0.0f, 500.0f);

		Screen.lockCursor = true;
	}

	protected virtual void Update()
	{
		Sanics = Mathf.Lerp (MinSanics, MaxSanics, Health / 100.0f);

		// Reduce health based on hunger.
		Hunger += Time.deltaTime * (GetComponent<Rigidbody>().velocity.magnitude / Sanics + 1);
		if (Hunger <= 0)
			Health += Time.deltaTime * Mathf.Max (-Hunger, 1);
		else
			Health -= Time.deltaTime * Hunger;

		// Reduce health based on thirst.
		Thirst += Time.deltaTime * (GetComponent<Rigidbody>().velocity.magnitude / Sanics + 1);
		if (Thirst <= 0)
			Health += Time.deltaTime * Mathf.Max (-Thirst, 1);
		else
			Health -= Time.deltaTime * Thirst;

		transform.Find("Head").Find ("GameObject").Find("character head").Find ("default").GetComponent<Renderer>().material.color = Color.Lerp (Color.blue, colour, Health / 4000.0f);

		if (Ded)
		{
			enabled = false;
			Invoke ("Kill", 5.0f);
		}

		// Turn the body towards the look yaw.
		Body.eulerAngles = new Vector3(0, Mathf.LerpAngle(Body.eulerAngles.y, look.y, Time.deltaTime * 5), 0);
		
		// Turn the head to the look direction quicker than the body.
		Head.rotation = Quaternion.Lerp (Head.rotation, Quaternion.Euler (look), Time.deltaTime * 10);
	}

	void Kill()
	{
		Destroy (gameObject);
	}

	protected virtual void FixedUpdate()
	{
		RaycastHit hit;
		Physics.Raycast (transform.position + Vector3.up * 10, Vector3.down, out hit, 100, 1 << LayerMask.NameToLayer ("Landscape"));
		float adjustedSanics = Sanics * (1 + Vector3.Dot(direction, hit.normal) * 0.5f);
		GetComponent<Rigidbody>().AddForce (Body.TransformDirection(new Vector3(direction.x, 0, direction.y)) * adjustedSanics * Time.fixedDeltaTime);
		GetComponent<Rigidbody>().position = new Vector3(GetComponent<Rigidbody>().position.x, Landscape.SampleHeight(GetComponent<Rigidbody>().position) + transform.localScale.y, GetComponent<Rigidbody>().position.z);
	}

	protected void Eat(float amount)
	{
		Hunger -= amount;
	}

	protected void Drink(float amount)
	{
		Thirst -= amount;
	}
}
