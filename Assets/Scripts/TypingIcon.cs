using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class TypingIcon : MonoBehaviour, IQuestion{
	private static List<Question> questionlist = new List<Question> ();
	private static string questionAnswer;
	private string questionString;
	private string questionData = "";
	private static string[] answerIdentifier = new string[13];
	public static int answerindex = 1;
	private int roundlimit = 3;
	private string answerwrote;
	public static int currentround = 1;
	private bool clickable = true;
	public GameObject[] indicators = new GameObject[3];
	public static int correctAnswers;
	private string answerData = "";
	private static GameObject questionModal;
	private static List<GameObject> inputlist = new List<GameObject>();
	private static List<GameObject> outputlist = new List<GameObject>();
	private static List<string> questionsDone = new List<string>();

	public void Activate(GameObject entity,float timeduration,Action<int> Result){
		currentround = 1;
		correctAnswers = 0;
		NextRound (currentround);
		QuestionController qc = new QuestionController ();
		qc.OnResult = Result;
	}

	public void NextRound(int round){
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
		GameObject questionInput = Resources.Load ("Prefabs/inputContainer") as GameObject;
		questionModal = GameObject.Find("TypingModal");
		inputlist.Clear ();
		outputlist.Clear ();
		for (int i = 0; i < questionAnswer.Length; i++) {
			GameObject output = Instantiate (questionInput) as GameObject; 
			output.transform.SetParent (questionModal.transform.GetChild (1).
				transform.GetChild (0).GetChild (0).transform, false);
			output.name = "output" + (i + 1);
			output.GetComponent<Button>().onClick.AddListener (() => {
				questionModal.GetComponent<TypingIcon>().OutputOnClick();
			});
			outputlist.Add(output);

			output.transform.GetChild (0).GetComponent<Text> ().text = "";
		}
		questionModal.transform.GetChild (0).GetComponent<Text> ().text = questionString;
	}

	public void InputOnClick(){
		if (!clickable) {
			iTween.ShakePosition(EventSystem.current.currentSelectedGameObject, new Vector3(10,10,10), 0.5f);
		} 
		else {
			answerwrote = "";
			int k = 1;
			foreach (GameObject findEmpty in outputlist) {
				if (findEmpty.transform.GetChild (0).GetComponent<Text> ().text == "") {
					answerindex = k;
					outputlist [(answerindex - 1)].transform.GetChild (0).
					GetComponent<Text> ().text 
					= EventSystem.current.currentSelectedGameObject.transform.GetChild (0).GetComponent<Text> ().text;
					break;
				} else {

				}
				k++;
			}
			foreach(GameObject readWrittenAnswer in outputlist){
				answerwrote = answerwrote + (readWrittenAnswer.transform.GetChild (0).GetComponent<Text> ().text);
			}
			answerIdentifier [(answerindex - 1)] = EventSystem.current.currentSelectedGameObject.name;
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
					outputlist [i].transform.position, 
					Quaternion.identity);
			}
			indicators[currentround-1].transform.GetChild (0).GetComponent<Text> ().text = "1 GP";
			indicators[currentround-1].transform.GetChild (0).DOScale (new Vector3 (5, 5, 5), 1.0f);
			Invoke("TweenCallBack", 1f);
		} else {
			GameObject.Find ("Indicator" + currentround).GetComponent<Image> ().color = Color.red;
			for (int i = 0; i < questionAnswer.Length; i++) {
				outputlist [i].transform.GetChild (0).GetComponent<Text> ().text = questionAnswer [i].ToString().ToUpper();
				outputlist [i].GetComponent<Image> ().color = Color.green;
			}
		}
		iTween.ShakePosition (questionModal, new Vector3 (10, 10, 10), 0.5f);
		clickable = false;
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
		if (currentround > roundlimit) {
			Clear ();
		}
		clickable = true;
	}

	public void OutputOnClick(){
		if (EventSystem.current.currentSelectedGameObject.transform.GetChild (0).GetComponent<Text> ().text == "") {
			iTween.ShakePosition(EventSystem.current.currentSelectedGameObject, new Vector3(10,10,10), 0.5f);
		} else {
			EventSystem.current.currentSelectedGameObject.transform.GetChild (0).GetComponent<Text> ().text = "";
		}
	}
	public void PopulateQuestionList(){
		
		List<string> databundle = CSVParser.GetQuestions ("wingquestion");
		int i = 0;
		foreach(string questions in databundle ){
			string[] splitter = databundle[i].Split (']');	
			questionData = splitter [0];
			answerData = splitter [1];
				questionlist.Add (new Question (questionData, answerData, 0));
			i+=1;
		}
	}

	public void Clear(){
		answerindex = 1;
		foreach (GameObject o in outputlist) {
			Destroy (o);
		}
		questionModal.transform.GetChild (0).GetComponent<Text> ().text = "";
	}
}
