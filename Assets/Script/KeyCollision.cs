using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeyCollision : MonoBehaviour {
	public Text AnimationText;

	private float[] handTouchTime;

	private GameController gameController;
	private Animator textAnim; 
	private float lastSoundTime;


	// Use this for initialization
	void Start () {
		handTouchTime = new float[4];
		gameController = GameObject.Find("GameController").GetComponent<GameController>();
		textAnim = AnimationText.GetComponent<Animator>();
		lastSoundTime = 0;
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter(Collider obj){
		
			
		if(obj.CompareTag("leftHand")||obj.CompareTag("rightHand")){
			//音效
			//Debug.Log(GameController.time - lastSoundTime);
			if(GameController.time - lastSoundTime > 300){
				GetComponent<AudioSource>().Play();
				Debug.Log(GameController.time - lastSoundTime);
				lastSoundTime = GameController.time;
				
			}
			switch(transform.gameObject.tag){
			case "leftVerticalKey":
				handTouchTime[0] = GameController.time;
				break;

			case "leftHorizontalKey":
				handTouchTime[1] = GameController.time;
				break;

			case "rightHorizontalKey":
				handTouchTime[2] = GameController.time;
				break;

			case "rightVerticalKey":
				handTouchTime[3] = GameController.time;
				break;
			}

			//判断perfect或其他
			transform.GetComponent<MeshRenderer>().material.color = Color.blue;
		}

		if(obj.CompareTag("beat")){
			// Debug.Log(NoteController.time);
			//Debug.Log("gametime: "+GameController.time);
			switch(gameObject.tag){
			case "leftVerticalKey":
				computeScore(0);
				break;

			case "leftHorizontalKey":
				computeScore(1);
				break;

			case "rightHorizontalKey":
				computeScore(2);
				break;

			case "rightVerticalKey":
				computeScore(3);
				break;
			}

			Destroy(obj.gameObject);
		}

	}

	void OnTriggerExit(Collider obj){
		if(obj.CompareTag("leftHand")||obj.CompareTag("rightHand")){
			transform.GetComponent<MeshRenderer>().material.color = Color.white;
		}

		if(obj.CompareTag("beat")){
			Destroy(obj.gameObject,0f);
		}
	}

	void computeScore(int index){
		float timeDifference = Mathf.Abs(GameController.time-handTouchTime[index]);
		if(timeDifference<gameController.judgeTime[0]){
			gameController.Score += gameController.judgeScore[0];
			showAnimation(0);
		}
		else if(timeDifference<gameController.judgeTime[1]){
			gameController.Score += gameController.judgeScore[1];
			showAnimation(1);
		}
		else if(timeDifference<gameController.judgeTime[2]){
			gameController.Score += gameController.judgeScore[2];
			showAnimation(2);
		}
		else {
			showAnimation(3);
		}
	}

	void showAnimation(int type){
		switch(type){
		case 0:
			AnimationText.text = "PERFECT";
			textAnim.SetTrigger("triggerPerfect");
			break;

		case 1:
			AnimationText.text = "GOOD";
			textAnim.SetTrigger("triggerGood");
			break;

		case 2:
			AnimationText.text = "HIT";
			textAnim.SetTrigger("triggerHit");
			break;

		case 3:
			AnimationText.text = "MISS";
			textAnim.SetTrigger("triggerMiss");
			break;
		}
	}
}
