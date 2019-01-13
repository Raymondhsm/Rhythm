using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Leap;
using Leap.Unity;


public class Gesture : MonoBehaviour{

	//public Plane mainMenu;
	//public Plane backPlane;
	//public Plane backSelectMusicPlane;

	private Hand hand;
	private Frame frame;
	private LeapProvider provider;

	private bool left;
	private bool right; 

	GameObject mainMenu;
	GameObject backPlane;
	GameObject backSelectMusicPlane;

	private bool isPressPlay;

	void Start(){
		mainMenu = GameObject.Find("Canvas/MainMenu");
		mainMenu.active = true;

		backPlane = GameObject.Find("Canvas/BackPanel");
		backPlane.active = false;

		backSelectMusicPlane = GameObject.Find("Canvas/SelectMusicPanelContainer/SelectMusicPanel");
		backSelectMusicPlane.active = false;

		provider = FindObjectOfType<LeapProvider>() as LeapProvider;
		left = false;
		right = false;

		isPressPlay = false;
	}

	void Update(){
		frame = provider.CurrentFrame;

		if(frame.Hands.Count > 0){
			hand = frame.Hands[0];
		}else {
			hand = null;
		}

		if(left && !isLeft() && isPressPlay) leftTheMenu();
		if(right && !isRight() && isPressPlay) rightTheMenu();

		GetComponent<RectTransform>().anchoredPosition = getPosition();

		left = isLeft();
		right = isRight();
	}

	void OnTriggerStay2D(Collider2D other){
		if(other.CompareTag("back") && isFisted() && isPressPlay) Back();
		if(other.CompareTag("btnPlay") && isFisted()) Play(); 
		if(other.CompareTag("btnQuit") && isFisted()) Quit(); 
		if(other.CompareTag("MusicImg") && isFisted()) chooseSong(int.Parse(""+other.name[other.name.Length-1]));
		Debug.Log(other.name[other.name.Length-1]);
	}

	public bool hasHand(){
		if(hand == null) return false;
		else return true;
	}

	public Vector2 getPosition(){
		if(hand == null) return new Vector2(0,0);
		else{
			float cx = hand.PalmPosition.x * 487f / 0.15f;
			float cy = hand.PalmPosition.y * 768f / 0.5f-100;
			return new Vector2(cx,cy);
		}
	}

	public bool isFisted(){
		if(hand != null && hand.GrabAngle > 3) return true;
		else return false;
	}

	public bool isLeft(){
		if(hand != null && hand.PalmVelocity.x < -1f) return true;
		else return false;
	}

	public bool isRight(){
		if(hand != null && hand.PalmVelocity.x < 1f) return true;
		else return false;
	}

	public void leftTheMenu(){
		GameObject.Find("Canvas/SelectMusicPanelContainer").GetComponent<SelectMusic>().PressLeft();
	}

	public void rightTheMenu(){
		GameObject.Find("Canvas/SelectMusicPanelContainer").GetComponent<SelectMusic>().PressRight();
	}

	public void chooseSong(int index){
		GameObject.Find("Canvas/SelectMusicPanelContainer").GetComponent<SelectMusic>().chooseSong(index);
	}

	public void Back(){
		isPressPlay = false;
		mainMenu.SetActive(true);
		backPlane.SetActive(false);
		backSelectMusicPlane.SetActive(false);
	}

	public void Play(){
		isPressPlay = true;
		mainMenu.SetActive(false);
		backPlane.SetActive(true);
		backSelectMusicPlane.SetActive(true);
	}

	public void Quit(){
		Application.Quit();
	}
	
}
