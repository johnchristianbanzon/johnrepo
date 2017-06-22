using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Question{
	public string question;
	public string answer;
	public int type;
	public Question(string question, string answer, int type){
		this.question = question;
		this.answer = answer;
		this.type = type;
	}
}
