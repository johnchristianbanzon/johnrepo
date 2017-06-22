using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class QuestionManager : MonoBehaviour
{
	private static int questiontype = 0;
	public GameObject[] questionTypeModals = new GameObject[6];

	private int numberofQuestionTypes = 3;
	private List<string> questionTypeName = new List<string>();
	void Start ()
	{
		//questionTypeModals [4].SetActive (true);
		questionTypeName.Add ("SelectLetterIconModal");
		questionTypeName.Add ("TypingModal");
		questionTypeName.Add ("ChangeOrderModal");
		questionTypeName.Add ("WordChoiceModal");
		questionTypeName.Add ("SlotMachineModal");
		numberofQuestionTypes = questionTypeName.Count;
		// 0 = SelectLetter
		// 1 = Order
		// 2 = ChangeOrder
		questiontype = 2;
	}

	public void SetQuestionEntry(int questionType, int questionTime, Action<int> onResult){
		questionTypeModals[questionType].SetActive (true);
		QuestionController qc = new QuestionController ();
		switch (questionType) {
		case 0:
			SelectLetterIcon selectletterIcon = new SelectLetterIcon ();
			//questionTypeModals[0].SetActive (true);
			qc.SetQuestion (selectletterIcon, questionTime, onResult);
			qc.TimeLeft = questionTime;

			break;
		case 1:
			TypingIcon typingIcon = new TypingIcon ();
			//questionTypeModals[1].SetActive (true);
			qc.SetQuestion (typingIcon, questionTime, onResult);
			qc.TimeLeft = questionTime;
			break;
		case 2:
			//questionTypeModals[2].SetActive (true);
			ChangeOrderIcon changeOrderIcon = new ChangeOrderIcon ();
			qc.SetQuestion (changeOrderIcon, questionTime, onResult);
			qc.TimeLeft = questionTime;
			break;
		case 3:
			WordChoiceIcon wordChoiceIcon = new WordChoiceIcon ();
			qc.SetQuestion (wordChoiceIcon, questionTime, onResult);
			qc.TimeLeft = questionTime;
			break;
		case 4:
			SlotMachineIcon slotMachineIcon = new SlotMachineIcon ();
			qc.SetQuestion (slotMachineIcon, questionTime, onResult);
			qc.TimeLeft = questionTime;
			break;
		}
	}
	public void DebugOnClick(){
		int time;
		Int32.TryParse (GameObject.Find ("Inputu").GetComponent<InputField> ().text, out time);
		if (time > 0) {

			switch (EventSystem.current.currentSelectedGameObject.name) {
			case "sellet":
				SetQuestionEntry (0, time, delegate(int result) {
					Debug.Log ("Total score is: " + result);
					questionTypeModals [0].SetActive (false);
					questionTypeModals [5].SetActive (true);
				});
				break;

			case "type":
				SetQuestionEntry (1, time, delegate(int result) {
					Debug.Log ("Total score is: " + result);
					questionTypeModals [1].SetActive (false);
					questionTypeModals [5].SetActive (true);
				});
				break;

			case "chaord":
				SetQuestionEntry (2, time, delegate(int result) {
					Debug.Log ("Total score is: " + result);
					questionTypeModals [2].SetActive (false);
					questionTypeModals [5].SetActive (true);
				});
				break;
			case "wocho":
				SetQuestionEntry (3, time, delegate(int result) {
					Debug.Log ("Total score is: " + result);
					questionTypeModals [3].SetActive (false);
					questionTypeModals [5].SetActive (true);
				});
				break;

			case "sloMac":
				SetQuestionEntry (4, time, delegate(int result) {
					Debug.Log ("Total score is: " + result);
					questionTypeModals [4].SetActive (false);
					questionTypeModals [5].SetActive (true);
				});
				break;
			}
			questionTypeModals [5].SetActive (false);
			GameObject.Find ("Indicator"+1).GetComponent<Image>().color = Color.white;
			GameObject.Find ("Indicator"+2).GetComponent<Image>().color = Color.white;
			GameObject.Find ("Indicator"+3).GetComponent<Image>().color = Color.white;
		
		}
	}

}

