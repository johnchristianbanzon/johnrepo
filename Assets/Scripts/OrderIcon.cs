using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Net;
using System.IO;

public class OrderIcon : MonoBehaviour, IQuestion {
	private static int round = 1;
	private string questionData;
	private string answerData;
	private static string questionAnswer; 
	private static string questionString;
	private GameObject[] answerlist = new GameObject[13];
	private GameObject[] selectionlist = new GameObject[13];
	public static List<Question> questionlist;
	private static List<string> questionsDone = new List<string>();
	private List<string> choices = new List<string> ();

	public void Activate(GameObject entity,float timeduration,Action<int> Result){
		NextRound (round);
		QuestionController qc = new QuestionController ();
		qc.OnResult = Result;
	}

	public void NextRound(int round){
		questionlist = new List<Question> ();
		GetQuestions ();


		int randomize = UnityEngine.Random.Range (0, questionlist.Count);
		questionAnswer = questionlist [randomize].answer.ToUpper().ToString();
		questionString = questionlist [randomize].question;
	

		while (questionsDone.Contains (questionString)) {
			randomize = UnityEngine.Random.Range (0, questionlist.Count);
			questionAnswer = questionlist [randomize].answer.ToUpper().ToString();
			questionString = questionlist [randomize].question;
			if (!questionsDone.Contains (questionString)) {
				break;
			}
		} 
		questionsDone.Add (questionString);
		GameObject questionInput = Resources.Load ("Prefabs/Letter") as GameObject;
		GameObject questionModal = GameObject.Find("OrderModal");
		for (int i = 0; i < questionAnswer.Length; i++) {
			if (questionAnswer [i] == ',' || i==questionAnswer.Length-1) {
				GameObject input = Instantiate (questionInput) as GameObject; 
				input.transform.SetParent (questionModal.transform.GetChild (1).
					transform.GetChild (0).GetChild (0).transform, false);
				input.name = "input" + (i + 1);
				input.GetComponent<Button>().onClick.AddListener (() => {
					//GameObject.Find("SelectLetterIcon").GetComponent<SelectLetterEvent>().AnswerOnClick();
					AnswerOnClick();
				});
				input.transform.GetChild (0).GetComponent<Text> ().text = "";
			}


		}
		PopulateList ();
		ShuffleAlgo ();
		questionModal.transform.GetChild (0).GetComponent<Text> ().text = questionString;


	}
	public void GetQuestions(){
		List<string> databundle = CSVParser.GetQuestions ("ordersample");
		int i = 0;
		foreach(string questions in databundle){

			string[] splitter = databundle[i].Split (']');	

			questionData = splitter [0];
			answerData = splitter [1];
			questionlist.Add (new Question (questionData, answerData, 1));
			i+=1;
		}
	}
	public void WordOnClick(){
		Debug.Log ("clicked");
	}
	public void AnswerOnClick(){
		Debug.Log ("clicked");
	}

	public void PopulateList(){
		string temp = "";
		for (int i = 0; i < questionAnswer.Length; i++) {
			
			switch (questionAnswer [i]) {
			case '"':
				if (i>0) {
					choices.Add (temp);
					temp = "";
				}
				break;
			case ','://
				choices.Add (temp);
				temp = "";
				break;
			default:
				temp = temp + questionAnswer [i];
				break;
			}
		}
	}
	public void ShuffleAlgo(){

		string letterContainer = "ABCD";
		string PrefabName = "";
		List<int> RandomExist = new List<int> ();
		int j = 0;
		int randomnum = 0;      
		foreach (string c in choices) {
			randomnum = UnityEngine.Random.Range (0, letterContainer.Length);        
			RandomExist.Add (randomnum);
			int whilecounter = 0;
			while (true) {
				if(whilecounter==100){
					break;
				}
				//int index = Array.IndexOf (RandomExist, randomnum);
				bool index = RandomExist.Contains(randomnum);
				//Debug.Log (index+"/"+randomnum);

				if (index) {
					randomnum = UnityEngine.Random.Range (0, letterContainer.Length);
				} else {
					Debug.Log ("FOUND");
					break;
				}

				whilecounter++;
			}
		
			GameObject.Find (PrefabName + letterContainer [randomnum]).transform.GetChild (0).GetComponent<Text> ().text = c;
			RandomExist.Add(randomnum);

			j = j + 1;
		}
	}
		
}
