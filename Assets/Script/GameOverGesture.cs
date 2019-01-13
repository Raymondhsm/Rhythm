using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;
using Leap.Unity;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverGesture : MonoBehaviour {


	private Hand hand;
	private Frame frame;
	private LeapProvider provider;

	// Use this for initialization
	void Start () {
		provider = FindObjectOfType<LeapProvider>() as LeapProvider;
	}
	
	// Update is called once per frame
	void Update () {
		frame = provider.CurrentFrame;

		if(frame.Hands.Count > 0){
			hand = frame.Hands[0];
		}else {
			hand = null;
		}

		GetComponent<RectTransform>().anchoredPosition = getPosition();


	}

	void OnTriggerStay2D(Collider2D other){
		if(other.CompareTag("retry") && isFisted()) retry(); 
		if(other.CompareTag("backToMenu") && isFisted()) backToMenu(); 
			
	}

	public bool hasHand(){
		if(hand == null) return false;
		else return true;
	}

	public Vector2 getPosition(){
		if(hand == null) return new Vector2(0,0);
		else{
			float cx = hand.PalmPosition.x * 487f / 0.15f;
			float cy = hand.PalmPosition.y * 223f / 0.07f;
			return new Vector2(cx,cy);
		}
	}

	public bool isFisted(){
		if(hand != null && hand.GrabAngle > 3) return true;
		else return false;
	}

	public void retry(){
		SceneManager.LoadScene(1);
	}

	public void backToMenu(){
		SceneManager.LoadScene(0);	
	}
}
