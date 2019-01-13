using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuitBtnClick : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Button btn = this.GetComponent<Button> ();
        btn.onClick.AddListener (OnClick);
	}

	void OnClick(){
		Application.Quit();
	}
	
	
}
