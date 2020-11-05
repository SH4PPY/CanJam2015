using UnityEngine;
using System.Collections;

public class scrFollower : scrPerson
{
	public AudioClip[] RandomClips;
	public AudioClip HungryClip, ThirstyClip;
	private float timeUntilNextRandomAudio = 0.0f;

	private scrConsumable targetConsumable = null;

	protected override void Awake ()
	{
		base.Awake ();
		colour = transform.Find("Head").Find ("GameObject").Find("character head").Find ("default").GetComponent<Renderer>().material.color;
		colour.r += Random.Range (-0.3f, 0.3f);
		transform.Find("Head").Find ("GameObject").Find("character head").Find ("default").GetComponent<Renderer>().material.color = colour;
		transform.Find ("Head").transform.localScale *= Random.Range (0.8f, 1.2f);
		timeUntilNextRandomAudio = Random.Range (2.0f, 20.0f);
	}

	protected override void Update ()
	{
		base.Update ();

		timeUntilNextRandomAudio -= Time.deltaTime;
		if (timeUntilNextRandomAudio <= 0)
		{
			GetComponent<AudioSource>().pitch = Random.Range (0.8f, 1.2f);
			GetComponent<AudioSource>().PlayOneShot(RandomClips[Random.Range (0, RandomClips.Length)]);
			timeUntilNextRandomAudio = Random.Range (15.0f, 30.0f);
		}

		look = Quaternion.LookRotation (scrPlayer.Instance.transform.position - transform.position, Vector3.up).eulerAngles;

		if (targetConsumable != null)
		{
			direction = new Vector2(targetConsumable.transform.position.x, targetConsumable.transform.position.z) - new Vector2(transform.position.x, transform.position.z);
			direction.Normalize();

			look = Quaternion.LookRotation(direction, Vector3.up).eulerAngles;
		}
	}

	protected override void FixedUpdate()
	{
		RaycastHit hit;
		Physics.Raycast (transform.position + Vector3.up * 10, Vector3.down, out hit, 100, 1 << LayerMask.NameToLayer ("Landscape"));
		float adjustedSanics = Sanics * (1 + Vector3.Dot(direction, hit.normal) * 0.5f);
		GetComponent<Rigidbody>().AddForce (transform.TransformDirection(new Vector3(direction.x, 0, direction.y)) * adjustedSanics * Time.fixedDeltaTime);
		GetComponent<Rigidbody>().position = new Vector3(GetComponent<Rigidbody>().position.x, Landscape.SampleHeight(GetComponent<Rigidbody>().position) + transform.localScale.y, GetComponent<Rigidbody>().position.z);
	}
	
	public void MoveTowards(Vector3 position)
	{
		direction = (new Vector2(position.x, position.z) - new Vector2(transform.position.x, transform.position.z)).normalized;
	}

	public void Stop()
	{
		direction = Vector2.zero;
	}

	public void GoTowardsConsumable(scrConsumable consumable)
	{
		targetConsumable = consumable;
	}

	public void IgnoreConsumable()
	{
		targetConsumable = null;
	}

}
