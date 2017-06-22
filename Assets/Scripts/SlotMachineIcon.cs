using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SlotMachineIcon : MonoBehaviour, IQuestion{
	private static int round = 1;
	private Action<int> onResult;
	private static List<Question> questionlist = new List<Question> ();
	private static string questionAnswer;
	private string questionString;
	private string questionData = "";
	private static string[] answerIdentifier = new string[13];
	private int letterno;
	public static int answerindex = 1;
	private int roundlimit = 3;
	private static bool stoptimer = false;
	private bool isSynonym = true;
	public static int currentround = 1;
	private string synonymData = "";
	private string antonymData = "";
	public GameObject[] indicators = new GameObject[3];
	public static int correctAnswers;
	private int numberOfRoulletes = 6;
	private string answerData = "";
	private static GameObject questionModal;
	private static List<GameObject> roulletes = new List<GameObject>();
	private static int randomnum = 0;
	public List<GameObject> inputlist = new List<GameObject>();
	private static List<string> questionsDone = new List<string>();
	private List<GameObject> roulleteText = new List<GameObject>();
	private static bool instantiateDone = false;
	private static bool gotAnswer = true;
	private static GameObject ballInstantiated;

	public float bulletSpeed = 1f;
	public Rigidbody bullet;

	public string answerwrote {
		get;
		set;
	}
	private static int timeLeft;
	private void StartTimer(){
		if (stoptimer) {
			if (timeLeft > 0) {
				timeLeft--;
			} else {
				answerindex = 1;
				currentround = currentround + 1;
				QuestionDoneCallback (true);
				stoptimer = false;
				CancelInvoke ("StartTimer");
			}
		}
	}

	public void Activate(GameObject entity,float timeduration,Action<int> Result){
		round = 1;
		correctAnswers = 0;
		instantiateDone = false;
		currentround = 1;
		NextRound (round);
		QuestionController qc = new QuestionController ();
		qc.OnResult = Result;
		//InvokeRepeating("StartTimer", 0, 1);
	}
	void Start(){
		
	}
	public void NextRound(int round){
		gotAnswer = true;
		stoptimer = false;
		Debug.Log (instantiateDone);
		for (int i = 0; i < numberOfRoulletes; i++) {
			if (!instantiateDone) {
				roulletes.Add(GameObject.Find ("Roullete" + (i + 1)));
			
				if (i == numberOfRoulletes - 1) {
					instantiateDone = true;
				
				}
			} else {
				roulletes [i].SetActive (true);
			}
		}
		PopulateQuestionList ();
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
		findSlotMachines ();
		questionModal = GameObject.Find("SlotMachineModal");
		for(int i = 6 ; i > questionAnswer.Length ;i--){
			roulletes[i-1].SetActive(false);
		}

		ShuffleAlgo ();
		if (isSynonym) {
			questionModal.transform.GetChild (0).GetComponent<Text> ().text = "Synonym: "+questionString.ToUpper();
		} else {
			questionModal.transform.GetChild (0).GetComponent<Text> ().text = "Antonym: "+questionString.ToUpper();
		}

	}

	public void findSlotMachines(){
		
		roulleteText.Clear ();
		GameObject content;
		Debug.Log (questionAnswer + "/" + questionAnswer.Length);
		for (int i = 1; i <= questionAnswer.Length; i++) {
			content = GameObject.Find ("RoulleteContent"+i);
			for (int j = 0; j < 5; j++) {
				roulleteText.Add (content.transform.GetChild(j).gameObject);
			}
		}
	}



	public void QuestionDoneCallback(bool result){
		QuestionController qc = new QuestionController ();
		qc.Returner (
			delegate {
				qc.onFinishQuestion =true;
				if (result) {

					if(currentround>roundlimit){
						for(int i = 1;i<=3;i++){
							GameObject.Find ("Indicator" + i).GetComponent<Image> ().color = Color.white;
						}
						questionModal.SetActive(false);
					}
					else{
						NextRound (currentround);
					}
				}
			},currentround,correctAnswers
		);
	}
		
	public void getAnswer(string ans){
		if (questionAnswer == ans) {
			correctAnswerGot ();
		}

	}
	public void correctAnswerGot(){
		
		if (gotAnswer) {
			iTween.ShakePosition(questionModal, new Vector3(10,10,10), 0.5f);
			correctAnswers = correctAnswers + 1;
			for (int i = 0; i < questionAnswer.Length; i++) {
				ballInstantiated = Resources.Load ("Prefabs/scoreBall") as GameObject;
				Instantiate (ballInstantiated, 
					roulletes [i].transform.position, 
					Quaternion.identity);
			}
			GameObject.Find ("Indicator"+currentround).GetComponent<Image> ().color = Color.blue;
			stoptimer = true;
			timeLeft = 2;
			gotAnswer = false;
		}

	}

	public void PopulateQuestionList(){
		questionlist.Clear ();
		List<string> databundle = CSVParser.GetQuestions ("slotmachine");
		int i = 0;
		int randomnum = UnityEngine.Random.Range (1, 3);
		foreach(string questions in databundle ){
			string[] splitter = databundle [i].Split (']');
			questionData = splitter [0];
			synonymData = splitter [1];
			antonymData = splitter [2];
			antonymData.Remove (antonymData.Length - 1);
			if (questionData.Length > 1) {
				switch (randomnum) {
				case 1:
					questionlist.Add (new Question (questionData, synonymData, 3));
					isSynonym = true;
					break;
				case 2:
					questionlist.Add (new Question (questionData, antonymData, 3));
					isSynonym = false;
					break;
				}
			}
			i+=1;
		}
	}

	public void ShuffleAlgo ()
	{
		string Letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
		int letterIndex = 0;
		int letterStartIndex = 0;
		int letterEndIndex = 5;
	    randomnum = UnityEngine.Random.Range (letterStartIndex+1, letterEndIndex);
		for (int i = 0; i < roulleteText.Count; i++) {
			roulleteText [i].transform.GetChild (0).GetComponent<Text> ().text = (i%randomnum)==0 ?
				questionAnswer [letterIndex].ToString ().ToUpper ():
				Letters [UnityEngine.Random.Range (0, Letters.Length)].ToString ().ToUpper ();
			if ((i % randomnum) == 0) {
				letterIndex += 1;
				letterStartIndex = letterEndIndex;
				letterEndIndex = letterEndIndex + 5;
				randomnum = UnityEngine.Random.Range (letterStartIndex, letterEndIndex);
			}
		}
	}
	public void Clear(){
		answerindex = 1;

		questionModal.transform.GetChild (0).GetComponent<Text> ().text = "";
	}
}
