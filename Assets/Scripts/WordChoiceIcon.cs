using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class WordChoiceIcon : MonoBehaviour, IQuestion
{
	private static int round = 1;
	private Action<int> onResult;
	private static List<Question> questionlist = new List<Question> ();
	private static string questionAnswer;
	private string questionString;
	private string questionData = "";
	private static string[] answerIdentifier = new string[13];
	private int letterno;
	public static int answerindex = 1;
	private string answertemp;
	private int roundlimit = 3;
	private string answerwrote;
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
			Debug.Log ("hey");
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
								OnEnd (true);
							} else {
								OnEnd (false);

							}
						}
					}

				}

			}
		}
		answerClicked.Clear ();
	}

	public void OnEnd (bool isWin)
	{
		Debug.Log ("ENDING!");
		if (isWin) {
			correctAnswers = correctAnswers + 1;
			indicators [currentround - 1].GetComponent<Image> ().color = Color.blue;
		} else {
			indicators [currentround - 1].GetComponent<Image> ().color = Color.red;
			iTween.ShakePosition (questionModal, new Vector3 (10, 10, 10), 0.5f);
		}
		Clear ();
		answerindex = 1;
		currentround = currentround + 1;
		QuestionDoneCallback (true);
	}

	public void QuestionDoneCallback (bool result)
	{
		QuestionController qc = new QuestionController ();
		qc.Returner (
			delegate {
				qc.onFinishQuestion = true;
				if (result) {
					if (currentround > roundlimit) {
						questionModal.SetActive (false);
					} else {
						NextRound (currentround);
					}
				}
			}, currentround, correctAnswers
		);
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
