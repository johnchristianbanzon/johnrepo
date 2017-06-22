using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class QuestionSystemTimer : MonoBehaviour {
	public static QuestionSystemTimer instance;
	private static bool stoptimer = false;
	private static int timeLeft = 0;

	void Start () {
		InvokeRepeating ("Timer",0,1);
	}

	public void StartTimer(int timeReceived){
		timeLeft = timeReceived;
		stoptimer = true;

	}

	private void Timer(){
		
		if (stoptimer) {
			if (timeLeft > 0) {
				timeLeft--;
			} else {
				stoptimer = false;
			}
		}
	}
}
