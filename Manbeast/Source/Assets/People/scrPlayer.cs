using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class scrPlayer : scrPerson
{
	public static scrPlayer Instance { get; private set; }

	public Vector3 CameraOffset;

	public int NumFollowers = 12;
	public GameObject FollowerPrefab;
	private List<scrFollower> followers = new List<scrFollower>();
	private bool followersFollowing = false;
	private GameObject evidence = null;
	private float followStopDistance = 5.0f;
	private float followCallDistance = 30.0f;

	bool armRaised = false;

	public AudioClip[] Riffs;
	float timeToNextRiff = 5.0f;

	protected override void Awake()
	{
		colour = transform.Find("Head").Find ("GameObject").Find("character head").Find ("default").GetComponent<Renderer>().material.color;
		evidence = gameObject;
		Instance = this;

		base.Awake ();

		// Generate followers.
		for (int i = 0; i < NumFollowers; ++i)
		{
			followers.Add(((GameObject)Instantiate(FollowerPrefab, transform.position, Quaternion.identity)).GetComponent<scrFollower>());
			followers[i].transform.position += new Vector3(Random.Range (4, 8) * Mathf.Sin ((float)i / NumFollowers * Mathf.PI * 2), 0, Random.Range (4, 8) * Mathf.Cos ((float)i / NumFollowers * Mathf.PI * 2));
			followers[i].Landscape = Landscape;
		}

		Camera.main.transform.position = Body.TransformPoint (CameraOffset);
		RaycastHit hit;
		if (Physics.Linecast (transform.position, Camera.main.transform.position, out hit))
			Camera.main.transform.position = transform.position + (Camera.main.transform.position - transform.position).normalized * hit.distance * 0.9f;

		Health = 8000.0f;
	}

	protected override void Update ()
	{
		base.Update ();

		HandleLookInput ();
		HandleMovementInput ();

		if (Input.GetButton ("Fire1"))
		{
			// Check if gathering evidence.
			RaycastHit hit;
			if (Physics.Raycast(Head.position, Head.forward, out hit, 10, 1 << LayerMask.NameToLayer("Consumable")))  
			{
				if (hit.transform.root.GetComponent<scrConsumable>().Evidence != null)
				{
					evidence = hit.transform.root.GetComponent<scrConsumable>().TakeEvidence();
				}

				// Set evidence viewmodel.
			}

			// Check if talking to followers.
			if (evidence != null && !followersFollowing)
			{
				Debug.Log ("calling to followers");
				if (Physics.CheckSphere(transform.position, followCallDistance, 1 << LayerMask.NameToLayer("Follower")))
				{
					followersFollowing = true;

					// Drop evidence.
					evidence = null; 
					// Call out with a different loudness depending on distance.
				}
			}

		}

		//if (followersFollowing)
		//{
			MakeFollowersFollow();
		//}

		timeToNextRiff -= Time.deltaTime;
		if (timeToNextRiff <= 0)
		{
			timeToNextRiff = Random.Range (15.0f, 40.0f);
			GetComponent<AudioSource>().PlayOneShot(Riffs[Random.Range (0, Riffs.Length)], 5);
		}
	}

	protected override void FixedUpdate()
	{
		base.FixedUpdate ();

		// Rotate the camera to the same direction as the player.
		Camera.main.transform.rotation = Quaternion.Lerp (Camera.main.transform.rotation, Head.rotation, 25 * Time.deltaTime);

		// Move the camera to the offset.
		Camera.main.transform.position = Vector3.Lerp (Camera.main.transform.position, Head.TransformPoint (CameraOffset), 25 * Time.deltaTime);
	}

	void HandleLookInput()
	{
		// Turning.
		look.y += Input.GetAxis ("Mouse X"); 
		if (look.y >= 360)
			look.y -= 360;
		else if (look.y < 0)
			look.y = 360 - look.y;

		look.x -= Input.GetAxis ("Mouse Y"); 
		if (look.x > 20)
			look.x = 20;
		else if (look.x < -40)
			look.x = -40;
	}

	void HandleMovementInput()
	{
		direction.x = Input.GetAxis ("Horizontal");
		direction.y = Input.GetAxis ("Vertical");
		direction.Normalize ();

		if (Input.GetButton("Sprint"))
		{
			direction *= 2.0f;
		}
	}

	void SortFollowers()
	{
		for (int i = 0; i < followers.Count; ++i)
		{
			for (int j = 0; j < followers.Count; ++j)
			{
				if (followers[j].Health > followers[i].Health)
				{
					scrFollower temp = followers[j];
					followers[j] = followers[i];
					followers[i] = temp;
				}
			}
		}
	}

	void ToggleFollowersFollowing()
	{
		followersFollowing = !followersFollowing;
		if (followersFollowing)
			SortFollowers ();
	}

	void MakeFollowersFollow()
	{
		Collider[] c = Physics.OverlapSphere (transform.position, 15.0f, 1 << LayerMask.NameToLayer ("Consumable"));
		if (c.Length != 0)
		{
			followersFollowing = false;
			for (int i = 0; i < followers.Count; ++i)
			{
				followers[i].GoTowardsConsumable(c[0].GetComponent<scrConsumable>());
			}
		}
		else
		{
			for (int i = 0; i < followers.Count; ++i)
				followers[i].IgnoreConsumable();
		}

		if (Vector2.Distance (new Vector2 (followers [0].transform.position.x, followers [0].transform.position.z), new Vector2 (transform.position.x, transform.position.z)) > followStopDistance)
			followers [0].MoveTowards (transform.position);
		else
			followers [0].Stop ();

		for (int i = followers.Count - 1; i >= 1; --i)
		{
			if (followers[i].Health <= 0)
			{
				followers[i].Ded = true;
				followers.Remove(followers[i]);
			}
			if (Vector2.Distance(new Vector2(followers[i].transform.position.x, followers[i].transform.position.z), new Vector2(followers[i - 1].transform.position.x, followers[i - 1].transform.position.z)) > followStopDistance)
				followers [i].MoveTowards (followers[i - 1].transform.position);
			else
				followers[i].Stop();
		}
	}
	
}
