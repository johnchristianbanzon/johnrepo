using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ChangeOrderIcon : MonoBehaviour, IQuestion{
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
	private string answerwrote;
	public static int currentround = 1;
	public GameObject[] indicators = new GameObject[3];
	public static int correctAnswers;
	private string answerData = "";
	private static GameObject questionModal;
	private static List<GameObject> inputlist = new List<GameObject>();
	private static List<GameObject> outputlist = new List<GameObject>();
	private static List<string> questionsDone = new List<string>();

	public void Activate(GameObject entity,float timeduration,Action<int> Result){
		round = 1;
		currentround = 1;
		correctAnswers = 1;
		NextRound (round);
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
		GameObject greenInput = Resources.Load ("Prefabs/inputContainerUI") as GameObject;
		questionModal = GameObject.Find("ChangeOrderModal");
		inputlist.Clear ();
		outputlist.Clear ();
		for (int i = 0; i < questionAnswer.Length; i++) {
			GameObject input = Instantiate (greenInput) as GameObject; 
			input.transform.SetParent (questionModal.transform.GetChild (2).
				transform.GetChild (0).GetChild (0).transform, false);
			input.name = "input" + (i + 1);
			input.GetComponent<Button>().onClick.AddListener (() => {
				//this.InputOnClick();
				GameObject.Find("ChangeOrderModal").GetComponent<ChangeOrderIcon>().InputOnClick();
				//GameObject.Find("SelectLetterIcon").GetComponent<SelectLetterEvent>().AnswerOnClick();
			});
			inputlist.Add(input);
			input.transform.GetChild (0).GetComponent<Text> ().text = "";

			GameObject output = Instantiate (questionInput) as GameObject; 
			output.transform.SetParent (questionModal.transform.GetChild (1).
				transform.GetChild (0).GetChild (0).transform, false);
			output.name = "output" + (i + 1);
			output.GetComponent<Button>().onClick.AddListener (() => {
				GameObject.Find("ChangeOrderModal").GetComponent<ChangeOrderIcon>().OutputOnClick();
			});
			outputlist.Add(output);

			output.transform.GetChild (0).GetComponent<Text> ().text = "";
		}
		ShuffleAlgo ();
		questionModal.transform.GetChild (0).GetComponent<Text> ().text = questionString;

	}
	public void InputOnClick(){
		if (EventSystem.current.currentSelectedGameObject.transform.GetChild (0).GetComponent<Text> ().text == "") {
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
					EventSystem.current.currentSelectedGameObject.transform.GetChild (0).GetComponent<Text> ().text = "";
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
					correctAnswers = correctAnswers + 1;
					indicators[currentround-1].GetComponent<Image> ().color = Color.blue;

				} else {
					indicators[currentround-1].GetComponent<Image> ().color = Color.red;
					iTween.ShakePosition(questionModal, new Vector3(10,10,10), 0.5f);
				}

				Clear ();
				answerindex = 1;
				currentround = currentround + 1;
				QuestionDoneCallback (true);
			}
		}
	}
	public void OutputOnClick(){
		string answerclicked = "";
		if (EventSystem.current.currentSelectedGameObject.transform.GetChild (0).GetComponent<Text> ().text == "") {
			iTween.ShakePosition(EventSystem.current.currentSelectedGameObject, new Vector3(10,10,10), 0.5f);
		} else {
			for (int i = 1; i < inputlist.Count+1; i++) {
				if (EventSystem.current.currentSelectedGameObject.name == ("output" + i)) {
					answerclicked = outputlist [i-1].transform.GetChild (0).GetComponent<Text> ().text;
					outputlist [i - 1].transform.GetChild (0).GetComponent<Text> ().text = "";
					GameObject.Find (answerIdentifier [i-1]).transform.GetChild (0).GetComponent<Text> ().text = answerclicked;
					Debug.Log (answerIdentifier [i - 1]);
				}

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
						questionModal.SetActive(false);
					}
					else{
						NextRound (currentround);
					}
				}
			},currentround,correctAnswers
		);
	}
	public void PopulateQuestionList(){

		List<string> databundle = CSVParser.GetQuestions ("wingquestion");
		int i = 0;
		foreach(string questions in databundle ){
			string[] splitter = databundle[i].Split (']');	

			questionData = splitter [0];
			answerData = splitter [1];
			if ((i % 2)==0) {
				questionlist.Add (new Question (questionData, answerData, 0));

			}

			i+=1;
		}
	}

	public void ShuffleAlgo ()
	{
		List<int> RandomExist = new List<int>();
		string temp = questionAnswer;

		letterno = 0;
		int randomnum = 0;      
		for (int z = 0; z < temp.Length; z++) {
			randomnum = UnityEngine.Random.Range (0, inputlist.Count);        
			int whileindex = 0;
			while (true) {
				if (whileindex > 100) {
					break;
				}
				bool index = RandomExist.Contains(randomnum);
				if (index) {
					randomnum = UnityEngine.Random.Range (0, inputlist.Count);
				} else {
					break;
				}
				whileindex++;
			}

			inputlist[letterno].transform.GetChild(0).GetComponent<Text>().text = 
				temp[randomnum].ToString().ToUpper();
			RandomExist.Add (randomnum);
			letterno = letterno + 1;
		}
	}
	public void Clear(){

		answerindex = 1;
		foreach (GameObject i in inputlist) {
			Destroy (i);
		}
		foreach (GameObject o in outputlist) {
			Destroy (o);
		}
		questionModal.transform.GetChild (0).GetComponent<Text> ().text = "";
	}
}
