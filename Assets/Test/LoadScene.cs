using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadScene : MonoBehaviour {

	// Use this for initialization
	IEnumerator Start () {
		yield return new WaitForSeconds(.6f);
		UnityEngine.SceneManagement.SceneManager.LoadScene("2");
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
