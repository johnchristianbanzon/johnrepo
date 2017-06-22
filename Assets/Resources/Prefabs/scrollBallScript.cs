using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class scrollBallScript : MonoBehaviour {

	public float speed = 200.0f;
	Transform r2d;
	private int indicatornum = 1;
	// Function called once when the bullet is created
	void Start () {
		r2d = GetComponent<Transform>();
		for (int i = 1; i < 4; i++) {
			if (GameObject.Find ("Indicator" + i).GetComponent<Image> ().color
			    != Color.blue && GameObject.Find ("Indicator" + i).GetComponent<Image> ().color
			    != Color.red) {
				indicatornum = i - 1;

				break;
			} else {
				indicatornum = 3;
			}
		}
	}
	void Update(){
		r2d.position = Vector3.MoveTowards(r2d.transform.position, GameObject.Find("Indicator"+indicatornum).transform.position, speed);
		if (r2d.position == GameObject.Find ("Indicator" + indicatornum).transform.position) {
			Destroy (r2d.gameObject);
		}
	}
		
}
