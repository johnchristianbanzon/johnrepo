using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabFactory : MonoBehaviour {
	private Dictionary<string,List<GameObject>> pooledPrefabRecordMap = new Dictionary<string,List<GameObject>> ();
	private Dictionary<string,GameObject> loadedPrefabRecordMap = new Dictionary<string,GameObject> ();
	public GameObject Load (string prefabName)
	{
		GameObject prefabObject = Resources.Load<GameObject> ("Prefab/" + prefabName);
		loadedPrefabRecordMap.Add (prefabName, prefabObject);
		return prefabObject;
	}
	/*
	public GameObject Show (string prefabName, GameObject parent)
	{
		if (prefabName == null || parent == null) {
			Debug.Log ("prefabName or parent is null");
			return null;
		}
			
	}*/
	private void Init (GameObject gameObject, GameObject parent)
	{
		gameObject.transform.parent = parent.transform;
		gameObject.transform.localPosition = new Vector3 (0, 0, 0);
		gameObject.transform.localRotation = Quaternion.identity;
		gameObject.transform.localScale = new Vector3 (1, 1, 1);
		gameObject.SendMessage ("OnPoolInit", SendMessageOptions.DontRequireReceiver);
	}
}
