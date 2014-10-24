using UnityEngine;
using System.Collections;

public class Button : MonoBehaviour {

	// Use this for initialization

	static public int scene = 0; 
    static public bool update = false; 

	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		Grabbable grabbed;
		grabbed = gameObject.GetComponent<Grabbable>();
		if (grabbed.scale && !Button.update) {
			scene = scene + 1; 
			Button.update = true; 
		} 
		else if (!grabbed.scale && Button.update) {
			Button.update = false; 
		}
	}
}
