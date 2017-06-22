using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public interface IQuestion {
	void Activate (GameObject entity,float timeDuration, Action<int> Result);
}
