using UnityEngine;
using System.Collections;

public class Button : MonoBehaviour {

	// Use this for initialization

	public int scene = 0; 

	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		Grabbable grabbed;
		grabbed = gameObject.GetComponent<Grabbable>();
		if (grabbed.scale = true) {
			scene = scene +1; 
		}
	}
}
