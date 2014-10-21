using UnityEngine;
using System.Collections;

public class GraphSetter : MonoBehaviour {

	Matrix4x4 QuadForm;
	// Use this for initialization
	void Start () {
		Matrix4x4 QuadForm = new Matrix4x4();
		for (int i = 0; i < 4; i++) {
			for (int j = 0; j < 4; j++) {
				QuadForm [i, j] = 0.0f;
			}
		}
		QuadForm [0, 0] = 1.0f;
		QuadForm [2, 2] = 1.0f;
	}
	
	// Update is called once per frame
	void Update () {
		float t = Time.timeSinceLevelLoad;
		QuadForm [0, 0] = Mathf.Sin (t) + 1.2f;
		QuadForm [2, 2] = Mathf.Cos (t) + 1.2f;
		float diagComponent = 0.2f * Mathf.Sin (0.7f * t) + 0.2f;
		QuadForm [2, 0] = diagComponent;
		QuadForm [0, 2] = diagComponent;
		renderer.material.SetMatrix ("_QuadForm", QuadForm);
	}
}
