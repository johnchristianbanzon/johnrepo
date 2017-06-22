/*
//public int questiontype;
public int questionlimit = 3;
public int ctr = 0;
public int score;
public int rounds = 3;
public int counter;
public int answerindex = 1;
public int currentround=1;
public float timeLeft = 16f;
public GameObject questionObject;
public GameObject canvas; 
public GameObject inputPrefab;

public Boolean correct = false; 
public Boolean AnimationStart = true; 
public Boolean questionsCleared= false;
public Boolean modalRaise = true;
public Boolean modalHide = false;
public string answer; 
public string questioncut; 
public string questionAnswer; 
public string temp; 
public string answerwrote;

public string[] AnswerIdentifier;
//ublic string questionArray;
// Use this for initialization
public GameObject[] selectionObject = new GameObject[12], letterClickable = new GameObject[12];

void Start ()
{
	canvas = GameObject.Find ("Canvas");
	questionObject = Resources.Load ("Prefabs/QuestionModal") as GameObject;
	inputPrefab = Resources.Load ("Prefabs/InputContainer") as GameObject;

	ctr = ctr + 1;

	DeployQuestion (questioncut);
	shuffleAlgo ();
	//ModalRaise ();
	AnswerIdentifier = new string[AnswerIdentifier.Length];
}

public void DeployQuestion (string question)
{
	GameObject newitem = Instantiate (questionObject) as GameObject; 
	newitem.transform.position = new Vector2 (canvas.transform.position.x, -745f);
	newitem.transform.GetChild (0).GetComponent<Text> ().text = question;
	newitem.transform.SetParent (canvas.transform, false);
	newitem.transform.localScale = new Vector3 (1, 1, 1);
	newitem.name = "QuestionModal";
	for (int i = 2; i <= letterClickable.Length+1; i++) {
		newitem.transform.GetChild (i).transform.GetComponent<Button> ().onClick.AddListener (() => LetterOnClick ());
		letterClickable [i - 2] = newitem.transform.GetChild (i).transform.GetComponent<Button> ().gameObject;
	}
	correct = true;
	for (int i = 0; i < questionAnswer.Length; i++) {

		GameObject input = Instantiate (inputPrefab) as GameObject; 
		input.transform.SetParent (newitem.transform.GetChild (1).transform.GetChild (0).GetChild (0).transform, false);
		input.name = "input" + (i + 1);
		input.GetComponent<Button> ().onClick.AddListener (() => AnswerOnClick ());
		selectionObject [i] = GameObject.Find ("input" + (i + 1)).transform.GetComponent<Button> ().gameObject;
		input.transform.GetChild (0).GetComponent<Text> ().text = "";

	}
}



public void ModalRedo ()
{
	questioncut = "";
	questionAnswer = "";
	answerindex = 1;
	CutQuestion (questionlist [UnityEngine.Random.Range (0, questionlist.Count)]);
	GameObject.Find("QuestionModal").transform.GetChild (0).GetComponent<Text> ().text = questioncut;
	for (int i = 0; i < questionAnswer.Length; i++) {

		GameObject input = Instantiate (inputPrefab) as GameObject; 
		input.transform.SetParent (GameObject.Find("QuestionModal").transform.GetChild 
			(1).transform.GetChild (0).GetChild (0).transform, false);
		input.name = "input" + (i + 1);
		input.GetComponent<Button> ().onClick.AddListener (() => AnswerOnClick ());
		selectionObject [i] = input.transform.GetComponent<Button> ().gameObject;
		input.transform.GetChild (0).GetComponent<Text> ().text = "";

	}
	shuffleAlgo ();


}

public void StopQuestion (int score)
{
	//ThisScore (score);
	//QuestionModal.Destroy();
}
//end stop question


public void RaiseModal(){
	//Debug.Log (GameObject.Find ("QuestionModal").transform.position.y);
	if (GameObject.Find ("QuestionModal").transform.position.y < canvas.transform.position.y - 3.25f) {
		GameObject.Find ("QuestionModal").transform.Translate (new Vector3 (0, 1.5f, 0), Space.World);
	} else {
		modalRaise = false;
	}
}


public void LetterOnClick ()
{
	AnswerIdentifier[answerindex-1] = EventSystem.current.currentSelectedGameObject.name;
	answerwrote = "";
	for (int i = 0; i < letterClickable.Length + 1; i++) {
		if (EventSystem.current.currentSelectedGameObject.name == ("Letter" + i)) {
			GameObject lettertemp = GameObject.Find ("Letter" + i);
			GameObject.Find ("input" + answerindex).GetComponent<Image> ().transform.GetChild (0).GetComponent<Text> ().text 
			= lettertemp.transform.GetChild (0).GetComponent<Text> ().text;
			lettertemp.GetComponent<Image> ().color = Color.blue;
			answerindex = answerindex + 1;
			lettertemp.SetActive (false);
		}
		//			GameObject.Find ("Letter" + i).GetComponent<Button>().interactable = false;

	}
	for (int i = 0; i < questionAnswer.Length; i++) {
		answerwrote = answerwrote + (selectionObject [i].transform.GetChild (0).GetComponent<Text> ().text);
	}
	if (answerwrote == questionAnswer) {
		//Destroy (GameObject.Find ("QuestionModal"));

		endGame ();

		clear ();

		GameObject.Find ("Indicator" + currentround).GetComponent<Image> ().color = Color.blue;
		currentround = currentround + 1;
		ModalRedo ();

	} else {
		if (answerwrote.Length == questionAnswer.Length) {
			endGame ();
			clear ();
			GameObject.Find ("Indicator" + currentround).GetComponent<Image> ().color = Color.red;
			currentround = currentround + 1;
			ModalRedo ();
		}

	}

}
public void endGame(){
	if (currentround == 3) {
		modalHide = true;
		questionsCleared = !questionsCleared;
		Destroy (GameObject.Find ("QuestionModal"));
	}
}
public void clear(){
	for (int i = 0; i < letterClickable.Length; i++) {
		if (i < questionAnswer.Length) {
			Destroy(selectionObject [i]);//.transform.GetChild (0).GetComponent<Text> ().text = "";

		}
		letterClickable [i].transform.GetChild (0).GetComponent<Text> ().text = "%";
		letterClickable [i].GetComponent<Image> ().color = Color.white;
		letterClickable [i].SetActive (true);
	}

}
// Update is called once per frame
void Update ()
{
	Timer ();
	if (modalRaise) {
		RaiseModal ();
	} else {

	}
}

public void Timer(){

	if (timeLeft < 1 || questionsCleared) {
		Destroy (GameObject.Find ("QuestionModal"));
	} 
	else {
		timeLeft -= Time.deltaTime;
		GameObject.Find ("Timer").GetComponent<Text> ().text = Math.Truncate(timeLeft).ToString();
	}

}
public void shuffleAlgo ()
{
	int[] RandomExist = new int[questionAnswer.Length];
	string temp = questionAnswer;

	counter = 0;
	int randomnum = 0;      
	for (int z = 0; z < temp.Length; z++) {
		randomnum = UnityEngine.Random.Range (1, 12);        
		RandomExist [counter] = randomnum;
		while (true) {
			int index = Array.IndexOf (RandomExist, randomnum);
			if (index > -1) {
				randomnum = UnityEngine.Random.Range (1, 12);
			} else {
				break;
			}
		}
		for (int i = 0; i < letterClickable.Length; i++) {
			if (randomnum == i) {

				GameObject.Find ("Letter" + i).GetComponent<Image> ().transform.GetChild (0).GetComponent<Text> ().text = temp [counter].ToString ().ToUpper ();       
			}			
		}
		RandomExist [counter] = randomnum;

		counter = counter + 1;

	}

	for (int f = 1; f < 13; f++) {
		string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
		int randomnum2 = UnityEngine.Random.Range (1, 26);
		int index = Array.IndexOf (RandomExist, f);
		if (index > -1) {

		} else {
			GameObject.Find ("Letter" + f).GetComponent<Image> ().transform.GetChild (0).GetComponent<Text> ().text = alphabet [randomnum2].ToString ().ToUpper ();
		}
	}

}
*/