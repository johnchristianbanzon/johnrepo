using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Net;
using System.IO;

public class SelectLetterIcon : MonoBehaviour, IQuestion
{

	private Action<int> onResult;
	private string questionData = "";
	private string answerData = "";
	//private int selectionlistcount = 13;
	private GameObject[] answerlist = new GameObject[13];
	private GameObject[] selectionlist = new GameObject[13];
	public static List<Question> questionlist;
	private static List<string> questionsDone = new List<string> ();
	public float timeDuration;
	private static int round = 1;
	private int letterno;
	public static int lettercount = 12;
	private GameObject[] selectionButtons = new GameObject[13];
	private GameObject[] inputButtons = new GameObject[13];
	public int currentround = 1;
	public static int answerindex = 1;
	public List<string> answerIdentifier;
	public string answerwrote;
	public static string questionAnswer;
	public static string questionString;
	public static float timeLeft;
	public static int correctAnswers;
	private Action<bool> timerResult;
	private static GameObject questionModal;
	private int roundsLimit = 3;
	//Properties
	/*
	public string QuestionAnswer{
		get{ 
			return questionAnswer;
		}
	}
	public List<string> QuestionsDone{
		get{ return questionsDone; 
		}
		set{ questionsDone = value;
		}
	}*/
	public void Activate (GameObject entity, float timeduration, Action<int> Result)
	{
		round = 1;
		NextRound (round);
		QuestionController qc = new QuestionController ();
		qc.OnResult = Result;
	}

	public void NextRound (int round)
	{

		//		Debug.Log (GetCSV("https://docs.google.com/spreadsheets/d/19cKJ0YqMbNQWQmW_ZuEj4h9AHeyC_-H899MRE1F3rkw/edit#gid=0"));
		questionlist = new List<Question> ();

		PopulateQuestionList ();
		int randomize = UnityEngine.Random.Range (0, questionlist.Count);
		questionAnswer = questionlist [randomize].answer.ToUpper ().ToString ();
		questionString = questionlist [randomize].question;
		while (questionsDone.Contains (questionString)) {
			randomize = UnityEngine.Random.Range (0, questionlist.Count);
			questionAnswer = questionlist [randomize].answer.ToUpper ().ToString ();
			questionString = questionlist [randomize].question;
			if (!questionsDone.Contains (questionString)) {
				break;
			}
		} 
		questionsDone.Add (questionString);

		GameObject questionInput = Resources.Load ("Prefabs/inputContainer") as GameObject;
		questionModal = GameObject.Find ("SelectLetterIconModal");
		for (int i = 0; i < questionAnswer.Length; i++) {
			GameObject input = Instantiate (questionInput) as GameObject; 
			input.transform.SetParent (questionModal.transform.GetChild (1).
				transform.GetChild (0).GetChild (0).transform, false);
			input.name = "input" + (i + 1);
			input.GetComponent<Button> ().onClick.AddListener (() => {
				questionModal.GetComponent<SelectLetterIcon> ().AnswerOnClick ();
			});
			answerlist [i] = input;
			input.transform.GetChild (0).GetComponent<Text> ().text = "";
		}
		ShuffleAlgo ();
		questionAnswer = questionlist [randomize].answer;
		questionModal.transform.GetChild (0).GetComponent<Text> ().text = questionString;
	}

	public void AnswerOnClick ()
	{
		string answerclicked = "";
		answerindex = 1;
		if (EventSystem.current.currentSelectedGameObject.transform.GetChild (0).GetComponent<Text> ().text == "") {
			//CODE FOR CLICKING ON EMPTY
			iTween.ShakePosition (EventSystem.current.currentSelectedGameObject, new Vector3 (10, 10, 10), 0.5f);
		} else {
			for (int i = 1; i < selectionButtons.Length + 1; i++) {
				if (EventSystem.current.currentSelectedGameObject.name == ("input" + i)) {
					answerclicked = inputButtons [i - 1].transform.GetChild (0).GetComponent<Text> ().text;
					inputButtons [i - 1].transform.GetChild (0).GetComponent<Text> ().text = "";
					GameObject.Find (answerIdentifier [i - 1]).transform.GetChild (0).GetComponent<Text> ().text = answerclicked;
				}
			}
			for (int j = 1; j <= questionAnswer.Length + 1; j++) {
				GameObject findEmpty = inputButtons [j].transform.GetChild (0).gameObject;
				if (findEmpty.GetComponent<Text> ().text == "") {
					answerindex = j;
					break;
				} 
			}
		}

	}

	public void LetterOnClick ()
	{
		if (EventSystem.current.currentSelectedGameObject.transform.GetChild (0).GetComponent<Text> ().text == "") {
			iTween.ShakePosition (EventSystem.current.currentSelectedGameObject, new Vector3 (10, 10, 10), 0.5f);
		} else {
			//SelectLetterIcon sli = new SelectLetterIcon ();

			for (int i = 0; i < selectionButtons.Length - 1; i++) {
				selectionButtons [i] = GameObject.Find ("Letter" + (i + 1));
				if (i <= inputButtons.Length) {
					inputButtons [i] = GameObject.Find ("input" + (i + 1));
				}
			}
			for (int j = 1; j <= questionAnswer.Length + 1; j++) {
				GameObject findEmpty = inputButtons [j - 1].transform.GetChild (0).gameObject;

				if (findEmpty.GetComponent<Text> ().text == "") {
					answerindex = j;
					break;
				} 
			}

			answerIdentifier [(answerindex - 1)] = EventSystem.current.currentSelectedGameObject.name;
			answerwrote = "";
			inputButtons [(answerindex - 1)].transform.GetChild (0).
			GetComponent<Text> ().text 
			= EventSystem.current.currentSelectedGameObject.transform.GetChild (0).GetComponent<Text> ().text;
			EventSystem.current.currentSelectedGameObject.transform.GetChild (0).GetComponent<Text> ().text = "";
			for (int j = 0; j < questionAnswer.Length; j++) {
				answerwrote = answerwrote + (GameObject.Find ("input" + (j + 1)).transform.GetChild (0).GetComponent<Text> ().text);
			}
			if (answerwrote.Length == questionAnswer.Length) {
	
				if (answerwrote.ToUpper () == questionAnswer.ToUpper ()) {
					QuestionDoneCallback (true);
				} else {
					QuestionDoneCallback (false);
				}
			}
		}
	}

	public void QuestionDoneCallback (bool result)
	{
		if (result) {
			correctAnswers = correctAnswers + 1;
			GameObject.Find ("Indicator" + currentround).GetComponent<Image> ().color = Color.blue;
			for (int i = 0; i < questionAnswer.Length; i++) {
				GameObject ballInstantiated = Resources.Load ("Prefabs/scoreBall") as GameObject;
				Instantiate (ballInstantiated, 
					inputButtons [i].transform.position, 
					Quaternion.identity);
			}
		} else {
			GameObject.Find ("Indicator" + currentround).GetComponent<Image> ().color = Color.red;
		}
		QuestionSystemTimer qst = new QuestionSystemTimer ();
		Invoke("OnEnd", 0.5f);


	}

	public void OnEnd(){
		QuestionController qc = new QuestionController ();
		Clear ();
		answerindex = 1;
		currentround = currentround + 1;
		iTween.ShakePosition (questionModal, new Vector3 (10, 10, 10), 0.5f);
		NextRound (currentround);
		qc.Returner (delegate {
			qc.onFinishQuestion = true;
		}, currentround, correctAnswers);
	}

	public void PopulateQuestionList ()
	{

		List<string> databundle = CSVParser.GetQuestions ("wingquestion");
		int i = 0;
		foreach (string questions in databundle) {
			string[] splitter = databundle [i].Split (']');	

			questionData = splitter [0];
			answerData = splitter [1];
			questionlist.Add (new Question (questionData, answerData, 0));
			i += 1;
		}


	}

	public void ShuffleAlgo ()
	{
		int[] RandomExist = new int[questionAnswer.Length];
		string temp = questionAnswer;

		letterno = 0;
		int randomnum = 0;      
		for (int z = 0; z < temp.Length; z++) {
			randomnum = UnityEngine.Random.Range (1, selectionlist.Length);        
			RandomExist [letterno] = randomnum;
			while (true) {
				int index = Array.IndexOf (RandomExist, randomnum);
				if (index > -1) {
					randomnum = UnityEngine.Random.Range (1, selectionlist.Length);
				} else {
					break;
				}
			}
			for (int i = 0; i < selectionlist.Length; i++) {
				if (randomnum == i) {
					
					GameObject.Find ("Letter" + i).GetComponent<Image> ().
					transform.GetChild (0).GetComponent<Text> ().text = temp [letterno].ToString ().ToUpper ();       
				}			
			}
			RandomExist [letterno] = randomnum;
			letterno = letterno + 1;

		}

		for (int f = 1; f < 13; f++) {
			string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
			int randomnum2 = UnityEngine.Random.Range (1, 26);
			int index = Array.IndexOf (RandomExist, f);
			if (index > -1) {

			} else {
				GameObject.Find ("Letter" + f).GetComponent<Image> ().transform.GetChild (0).GetComponent<Text> ().text = 
					alphabet [randomnum2].ToString ().ToUpper ();
			}
		}

	}

	public void Clear ()
	{

		answerindex = 1;
		for (int i = 0; i < selectionButtons.Length - 1; i++) {
			selectionButtons [i].transform.GetChild (0).GetComponent<Text> ().text = "";
			if (i <= questionAnswer.Length) {
				Destroy (inputButtons [i]);
			}
		}

	}
}
