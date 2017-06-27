using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class WordChoiceIcon : MonoBehaviour, IQuestion
{
	private static int round = 1;
	private static List<Question> questionlist = new List<Question> ();
	private static string questionAnswer;
	private string questionString;
	private string questionData = "";
	private int letterno;
	public static int answerindex = 1;
	private string answertemp;
	private int roundlimit = 3;
	public static int currentround = 1;
	public GameObject[] indicators = new GameObject[3];
	public static int correctAnswers;
	private string synonymData = "";
	private string antonymData = "";
	private static string answer1 = "";
	private static string answer2 = "";
	private static GameObject questionModal;
	private static List<GameObject> inputlist = new List<GameObject> ();
	private static List<string> questionsDone = new List<string> ();
	private List<string> wrongChoices = new List<string> ();
	private List<GameObject> answerClicked = new List<GameObject> ();
	private bool isSynonym = false;

	public void Activate (GameObject entity, float timeduration, Action<int> Result)
	{
		round = 1;
		currentround = 1;
		correctAnswers = 0;
		NextRound (round);
		QuestionController qc = new QuestionController ();
		qc.OnResult = Result;
	}

	public void NextRound (int round)
	{
		PopulateQuestionList ();

		int randomize;
		foreach (Question q in questionlist) {
		}
		randomize = UnityEngine.Random.Range (0, questionlist.Count);
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
		for (int i = 1; i < 5; i++) {
			inputlist.Add (GameObject.Find ("Word" + i));
		}
		questionsDone.Add (questionString);
		questionModal = GameObject.Find ("WordChoiceModal");
		ShuffleAlgo ();
		if (isSynonym) {
			questionModal.transform.GetChild (0).GetComponent<Text> ().text = "Synonym: " + questionString.ToUpper ();
		} else {
			questionModal.transform.GetChild (0).GetComponent<Text> ().text = "Antonym: " + questionString.ToUpper ();
		}
	}

	public void InputOnClick ()
	{
		GameObject answerclick = EventSystem.current.currentSelectedGameObject;

		if (answerclick.GetComponent<Image> ().color == Color.gray) {
			answerclick.GetComponent<Image> ().color = Color.white;
		} else {
			answerclick.GetComponent<Image> ().color = Color.gray;
		}
		int colorindex = 0;
		for (int i = 0; i < 4; i++) {
			if (inputlist [i].GetComponent<Image> ().color == Color.gray) {
				colorindex += 1;
				answerClicked.Add (inputlist [i]);
				if (colorindex >= 2) {
					answertemp = answerClicked [0].transform.GetChild (0).GetComponent<Text> ().text;
					int answerclickindex = 0;
					foreach (GameObject c in answerClicked) {
						answerclickindex += 1;
						string answertemp = c.transform.GetChild (0).GetComponent<Text> ().text;
						if (answerclickindex == 2) {
							if (answertemp == answer1 || answertemp == answer2) {
								QuestionDoneCallback (true);
							} else {
								QuestionDoneCallback (false);

							}
						}
					}

				}

			}
		}
		answerClicked.Clear ();
	}

	public void QuestionDoneCallback (bool result)
	{
		if (result) {
			correctAnswers = correctAnswers + 1;
			GameObject.Find ("Indicator" + currentround).GetComponent<Image> ().color = Color.blue;
			for (int i = 0; i < answerClicked.Count; i++) {
				GameObject ballInstantiated = Resources.Load ("Prefabs/scoreBall") as GameObject;
				Instantiate (ballInstantiated, 
					answerClicked[i].transform.position, 
					Quaternion.identity);
			}
			indicators[currentround-1].transform.GetChild (0).GetComponent<Text> ().text = "1 GP";
			indicators[currentround-1].transform.GetChild (0).DOScale (new Vector3 (5, 5, 5), 1.0f);
			Invoke("TweenCallBack", 1f);
		} else {
			GameObject.Find ("Indicator" + currentround).GetComponent<Image> ().color = Color.red;
			for (int i = 0; i < inputlist.Count; i++) {
				//inputlist [i].transform.GetChild (0).GetComponent<Text> ().text = questionAnswer [i].ToString().ToUpper();
				if (answer1 == inputlist [i].transform.GetChild (0).GetComponent<Text> ().text ||
				    answer2 == inputlist [i].transform.GetChild (0).GetComponent<Text> ().text) {
					inputlist [i].GetComponent<Image> ().color = Color.red;
					//inputlist [i].transform.GetChild(0).GetComponent<Text> ().color = Color.white;
				} else {
					inputlist [i].GetComponent<Image> ().color = Color.white;
				}
			}
		}
		iTween.ShakePosition (questionModal, new Vector3 (10, 10, 10), 0.5f);
		Invoke("OnEnd", 1f);
	}

	public void TweenCallBack(){
		indicators[currentround-1].
		transform.GetChild (0).DOScale (new Vector3(1,1,1),1.0f);
		indicators[currentround-1].
		transform.GetChild (0).GetComponent<Text> ().text = " ";
	}

	public void OnEnd(){
		QuestionController qc = new QuestionController ();
		Clear ();
		answerindex = 1;
		currentround = currentround + 1;
		NextRound (currentround);
		qc.Returner (delegate {
			qc.onFinishQuestion = true;
		}, currentround, correctAnswers);
		if (currentround == 4) {
			Clear ();
		}
	}

	public void PopulateQuestionList ()
	{
		questionlist.Clear ();

		List<string> databundle = CSVParser.GetQuestions ("wordchoice");
		int i = 0;
		int randomnum = UnityEngine.Random.Range (1, 3);
		foreach (string questions in databundle) {
			string[] splitQuestion = databundle [i].Split (']');	
			questionData = splitQuestion [0];	
			synonymData = splitQuestion [1];
			antonymData = splitQuestion [2];
			wrongChoices.Add (splitQuestion [3]);
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
			i += 1;
		}
	}

	public void ShuffleAlgo ()
	{
		List<int> RandomExist = new List<int> ();
		string[] temp = questionAnswer.Split ('/');
		letterno = 0;
		int randomnum = UnityEngine.Random.Range (0, 5); 
		int whileindex = 0;
		for (int i = 0; i < 4; i++) {
			
			while (true) {
				if (whileindex > 100) {
					break;
				}
				randomnum = UnityEngine.Random.Range (0, 4);
				bool index = RandomExist.Contains (randomnum);
				if (index) {
					randomnum = UnityEngine.Random.Range (0, 4);
				} else {
					RandomExist.Add (randomnum);
					letterno = letterno + 1;
					switch (letterno) {
					case 1:
						inputlist [randomnum].transform.GetChild (0).GetComponent<Text> ().text = 
						temp [0].ToString ().ToUpper ();
						answer1 = temp [0].ToString ().ToUpper ();

						break;
					case 2:
						inputlist [randomnum].transform.GetChild (0).GetComponent<Text> ().text = 
						temp [1].ToString ().ToUpper ();
						answer2 = temp [1].ToString ().ToUpper ();

						break;
					case 3:
						inputlist [randomnum].transform.GetChild (0).GetComponent<Text> ().text = 
							wrongChoices [UnityEngine.Random.Range (0, wrongChoices.Count)].ToUpper ();
						break;
					case 4:
						inputlist [randomnum].transform.GetChild (0).GetComponent<Text> ().text = 
							wrongChoices [UnityEngine.Random.Range (0, wrongChoices.Count)].ToUpper ();
						break;
					}

					break;

				}
				whileindex++;
			}
		}
			
			

	}

	public void Clear ()
	{
		foreach (GameObject g in inputlist) {
			g.GetComponent<Image> ().color = Color.white;
		}
		answerindex = 1;
	}
}
