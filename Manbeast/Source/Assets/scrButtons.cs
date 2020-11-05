using UnityEngine;
using System.Collections;

public class scrButtons : MonoBehaviour
{
	public void exit()
	{
		Application.Quit ();
	}

	public void play()
	{
		Application.LoadLevel ("Ingame");
	}
}