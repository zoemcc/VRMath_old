using UnityEngine;
using System.Collections;

public class ScaleObject : MonoBehaviour {

	// Use this for initialization

	public Vector3 pos; 
	public float step_size; 
	public GameObject Hands; 

	void Start () {
	    var t = gameObject.transform; 
		pos = t.localPosition; 
		step_size = 0.01f; 
		Hands = GameObject.Find ("/OVRCameraController/CameraLeft/HandController"); 
	

	}
	
	// Update is called once per frame
	void Update () {
		Grabbable grabbed;
		HandController h; 

		h = Hands.GetComponent<HandController> ();

		HandModel[] hands = h.GetAllPhysicsHands();
		bool pinch = true;  
		Vector3[] poses = new Vector3[2]; 
		for (int i=0; i<hands.Length; i++) {

			HandModel hand = hands[i]; 
			GrabHand grab_hand;
			grab_hand = hand.GetComponent<GrabHand>();
		
			pinch = grab_hand.pinching_ && pinch; 
			poses[i] = hand.GetPalmPosition(); 
		}


		grabbed = gameObject.GetComponent<Grabbable>();
		var t = gameObject.transform;
	
		if(grabbed.scale && pinch && hands.Length>1){

			Vector3 current_pos = t.localPosition; 
			Vector3 scale = poses[1] - poses[0]; 
			for (int i=0; i<2;i++){
				if(scale[i] < 0){
					scale[i] = -scale[i];
				}
				scale[i] = scale[i]/2.0f; 
			}
		
			print (scale);
			t.localScale = scale;

		}
		t.localPosition = pos; 
	}
}
