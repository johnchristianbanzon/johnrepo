using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SlotMachineOnChange : MonoBehaviour {
	
	private ScrollRect myScrollRect;
	private ScrollRect topScrollRect;
	private ScrollRect bottomScrollRect;
	private List<GameObject> textSlots = new List<GameObject>();
	private int numberOfTextSlots = 5;
	private float contentPosition;
	private RectTransform slotContent;
	private float[] positionCompare = new float[5];
	private float positionCounter = 0.5f;
	private float scrollIncrement = 0.25f;
	private int slotIndex = 0;
	private bool stopDrag = false;
	private static string writtenAnswer;
	private static string roullete1Answer;
	private static string roullete2Answer;
	private static string roullete3Answer;
	private static string roullete4Answer;
	private static string roullete5Answer;
	private static string roullete6Answer;
	public bool InitByUser = false; 
	private ScrollRect _scrollRect;
	private ContentSizeFitter _contentSizeFitter; 
	private VerticalLayoutGroup _verticalLayoutGroup;
	private HorizontalLayoutGroup _horizontalLayoutGroup;
	private GridLayoutGroup _gridLayoutGroup;
	private bool _isVertical = false; 
	private bool _isHorizontal = false; 
	private float _disableMarginX = 0;
	private float _disableMarginY = 0;
	private bool _hasDisabledGridComponents = false; 
	private List <RectTransform> items = new List<RectTransform>();

	//private bool stopScrolling = false;

	public void getContentPosition(){

		myScrollRect = GetComponent<ScrollRect>();
		topScrollRect = myScrollRect.transform.GetChild(1).GetComponent<ScrollRect> ();
		bottomScrollRect = myScrollRect.transform.GetChild(2).GetComponent<ScrollRect> ();
		slotContent = myScrollRect.content;
		contentPosition = slotContent.transform.position.y;
	}
	void Start(){
		getContentPosition ();
		GetSlots ();
		getScrollItem ();
		for(int i = 0;i< 5;i++){
			topScrollRect.transform.GetChild (0).GetChild (0).GetChild(i).GetChild (0).GetComponent<Text> ().text =
				myScrollRect.transform.GetChild (0).GetChild (0).GetChild (i).GetChild (0).GetComponent<Text> ().text;
			bottomScrollRect.transform.GetChild (0).GetChild (0).GetChild(i).GetChild (0).GetComponent<Text> ().text =
				myScrollRect.transform.GetChild (0).GetChild (0).GetChild (i).GetChild (0).GetComponent<Text> ().text;
		}
	

	}
	public void getScrollItem(){
		slotIndex = numberOfTextSlots;
		foreach (float p in positionCompare) {
			slotIndex -= 1;
			positionCounter = Mathf.Round (positionCounter * 100f) / 100f;
			if (p == positionCounter) {
					switch (myScrollRect.transform.parent.parent.name) {
					case "Roullete1":
					roullete1Answer = textSlots [slotIndex].activeInHierarchy ? textSlots [slotIndex].transform.GetChild (0).GetComponent<Text> ().text:" ";
						break;
					case "Roullete2":
					roullete2Answer = textSlots [slotIndex].activeInHierarchy ? textSlots [slotIndex].transform.GetChild (0).GetComponent<Text> ().text:" ";
						break;
					case "Roullete3":
					roullete3Answer = textSlots [slotIndex].activeInHierarchy ? textSlots [slotIndex].transform.GetChild (0).GetComponent<Text> ().text:" ";
						break;
					case "Roullete4":
					roullete4Answer = textSlots [slotIndex].activeInHierarchy ? textSlots [slotIndex].transform.GetChild (0).GetComponent<Text> ().text:" ";
						break;
					case "Roullete5":
					roullete5Answer = textSlots [slotIndex].activeInHierarchy ? textSlots [slotIndex].transform.GetChild (0).GetComponent<Text> ().text:" ";
						break;
					case "Roullete6":
					roullete6Answer = textSlots [slotIndex].activeInHierarchy ? textSlots [slotIndex].transform.GetChild (0).GetComponent<Text> ().text:" ";
						break;
					}
					writtenAnswer = roullete1Answer + roullete2Answer + roullete3Answer + roullete4Answer + roullete5Answer + roullete6Answer;
					SlotMachineIcon smi = new SlotMachineIcon ();
					smi.getAnswer (writtenAnswer);

			}
		}
	}
	public void ClearAnswers(){
		
		roullete1Answer = "";
		roullete2Answer = "";
		roullete3Answer = "";
		roullete4Answer = "";
		roullete5Answer = "";
		roullete6Answer = "";
	}
	public void GetSlots(){
		textSlots.Clear ();
		for (int i = 0; i < numberOfTextSlots; i++) {
			textSlots.Add (myScrollRect.transform.GetChild (0).GetChild (0).GetChild (i).gameObject);
			positionCompare [i] = ((1.0f / (numberOfTextSlots - 1))) * (i);
		
		}
	}

	public void OnButtonDown(){
		getContentPosition ();
		GetSlots ();
		if (positionCounter <= 1f && positionCounter >= -0.9f) {
			switch (EventSystem.current.currentSelectedGameObject.name) {
			case "UpIcon":	
				positionCounter  = positionCounter == 1 ? positionCounter = 0 : positionCounter + scrollIncrement;
				break;
			case "DownIcon":
				positionCounter  = positionCounter == 0 ? positionCounter = 1 : positionCounter - scrollIncrement;
				break;
			}
		}
		getScrollItem ();
	}

	void Update(){
		getScrollItem ();
		myScrollRect.verticalNormalizedPosition = 
			Mathf.Lerp(myScrollRect.verticalNormalizedPosition, positionCounter, 0.5f);
		topScrollRect.verticalNormalizedPosition =  positionCounter == 1 ? 
			Mathf.Lerp(myScrollRect.verticalNormalizedPosition, -1, 0.5f):
			Mathf.Lerp(myScrollRect.verticalNormalizedPosition, positionCounter + (scrollIncrement * 2), 0.5f);
		bottomScrollRect.verticalNormalizedPosition = positionCounter == 0 ? 
			Mathf.Lerp(myScrollRect.verticalNormalizedPosition, 2.0f, 0.5f) :
			Mathf.Lerp(myScrollRect.verticalNormalizedPosition, positionCounter - (scrollIncrement * 2), 0.5f);
		

	}

	public void OnEndDrag(){
		stopDrag = false;
		myScrollRect.enabled = true;

	}

	public void OnDrag(){
		GetSlots ();
		if (!stopDrag) {
			positionCounter = myScrollRect.verticalNormalizedPosition;
			//Debug.Log (Mathf.Round (myScrollRect.verticalNormalizedPosition * 100f) / 100f);
			foreach (float p in positionCompare) {
				if (p == slotContent.anchoredPosition.y) {
					stopDrag = true;
				}
			}

		} else {
			myScrollRect.enabled = false;
		}
	}
		
	void Awake ()
	{
		if(!InitByUser)
			Init();
	}
	public void Init()
	{
		if(GetComponent<ScrollRect>() != null)
		{
			_scrollRect = GetComponent<ScrollRect>();
			_scrollRect.onValueChanged.AddListener(OnScroll);
			_scrollRect.movementType = ScrollRect.MovementType.Unrestricted;

			for(int i=0;i<_scrollRect.content.childCount;i++)
			{
				items.Add(_scrollRect.content.GetChild(i).GetComponent<RectTransform>());
			}
			if(_scrollRect.content.GetComponent<VerticalLayoutGroup>() != null)
			{
				_verticalLayoutGroup = _scrollRect.content.GetComponent<VerticalLayoutGroup>();
			}
			if(_scrollRect.content.GetComponent<HorizontalLayoutGroup>() != null)
			{
				_horizontalLayoutGroup = _scrollRect.content.GetComponent<HorizontalLayoutGroup>();
			}
			if(_scrollRect.content.GetComponent<GridLayoutGroup>() != null)
			{
				_gridLayoutGroup = _scrollRect.content.GetComponent<GridLayoutGroup>();
			}
			if(_scrollRect.content.GetComponent<ContentSizeFitter>() != null)
			{
				_contentSizeFitter = _scrollRect.content.GetComponent<ContentSizeFitter>();
			}

			_isHorizontal = _scrollRect.horizontal;
			_isVertical = _scrollRect.vertical;

			if(_isHorizontal && _isVertical)
			{
				Debug.LogError("UI_InfiniteScroll doesn't support scrolling in both directions, plase choose one direction (horizontal or vertical)");
			}

			_itemCount = _scrollRect.content.childCount;
		}
		else
		{
			Debug.LogError("UI_InfiniteScroll => No ScrollRect component found");
		}
	}

	void DisableGridComponents()
	{
		if(_isVertical)
		{
			_recordOffsetY = items[0].GetComponent<RectTransform>().anchoredPosition.y - items[1].GetComponent<RectTransform>().anchoredPosition.y;
			_disableMarginY = _recordOffsetY * _itemCount /2;// _scrollRect.GetComponent<RectTransform>().rect.height/2 + items[0].sizeDelta.y;
		}
		if(_isHorizontal)
		{
			_recordOffsetX = items[1].GetComponent<RectTransform>().anchoredPosition.x - items[0].GetComponent<RectTransform>().anchoredPosition.x;
			_disableMarginX = _recordOffsetX * _itemCount /2;//_scrollRect.GetComponent<RectTransform>().rect.width/2 + items[0].sizeDelta.x;
		}

		if(_verticalLayoutGroup)
		{
			_verticalLayoutGroup.enabled = false; 
		}
		if(_horizontalLayoutGroup)
		{
			_horizontalLayoutGroup.enabled = false; 
		}
		if(_contentSizeFitter)
		{
			_contentSizeFitter.enabled = false; 
		}
		if(_gridLayoutGroup)
		{
			_gridLayoutGroup.enabled = false; 
		}
		_hasDisabledGridComponents = true; 
	}
	private Vector2 _newAnchoredPosition = Vector2.zero;
	//TO DISABLE FLICKERING OBJECT WHEN SCROLL VIEW IS IDLE IN BETWEEN OBJECTS
	private float _treshold = 100f; 
	private int _itemCount = 0;
	private float _recordOffsetX = 0;
	private float _recordOffsetY = 0;
	public void OnScroll(Vector2 pos)
	{

		if(!_hasDisabledGridComponents)
			DisableGridComponents();

		for(int i=0;i<items.Count;i++)
		{
			if(_isHorizontal)
			{
				if (_scrollRect.transform.InverseTransformPoint(items[i].gameObject.transform.position).x > _disableMarginX + _treshold)
				{
					_newAnchoredPosition = items[i].anchoredPosition;
					_newAnchoredPosition.x -= _itemCount * _recordOffsetX;
					items[i].anchoredPosition = _newAnchoredPosition;
					_scrollRect.content.GetChild(_itemCount-1).transform.SetAsFirstSibling();
				}
				else if (_scrollRect.transform.InverseTransformPoint(items[i].gameObject.transform.position).x < -_disableMarginX)
				{
					_newAnchoredPosition = items[i].anchoredPosition;
					_newAnchoredPosition.x += _itemCount * _recordOffsetX;
					items[i].anchoredPosition = _newAnchoredPosition;
					_scrollRect.content.GetChild(0).transform.SetAsLastSibling();
				}
			}

			if(_isVertical)
			{
				if (_scrollRect.transform.InverseTransformPoint(items[i].gameObject.transform.position).y > _disableMarginY + _treshold)
				{
					_newAnchoredPosition = items[i].anchoredPosition;
					_newAnchoredPosition.y -= _itemCount * _recordOffsetY;
					items[i].anchoredPosition = _newAnchoredPosition;
					_scrollRect.content.GetChild(_itemCount-1).transform.SetAsFirstSibling();
				}
				else if (_scrollRect.transform.InverseTransformPoint(items[i].gameObject.transform.position).y < -_disableMarginY)
				{
					_newAnchoredPosition = items[i].anchoredPosition;
					_newAnchoredPosition.y += _itemCount * _recordOffsetY;
					items[i].anchoredPosition = _newAnchoredPosition;
					_scrollRect.content.GetChild(0).transform.SetAsLastSibling();
				}
			}
		}
	}
}
