using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

using osu_beatmap_Csharp;

public class GameController : MonoBehaviour {

	public GameObject beats;
	public float zPoint;
	public float keyZPoint;

	public Canvas canvas;
	public Image image;
	public Camera mainCamera;

	private Beatmap beatmap;
	// public static float time;
	private int beatCounter;
	private float timeOffset;




	public int[] judgeScore;
	public int[] judgeTime;
	public int Score;
	public Text ScoreText;
	//public Text[] AnimationText;

	private Animator gameOverAnim;
	
	private AudioSource music;

	public static float time;

	public static bool isEnd;

	private bool isPlaying=false;
	// Use this for initialization
	void Start () {
		//初始化谱图
		string beatmapFileName = PlayerPrefs.GetString("beatmapFileName");
		beatmap = ReadSongsFromFile.readFullBeatmap(beatmapFileName);

		timeOffset = (zPoint - keyZPoint) / Move.speed * 1000 ;//转换单位为毫秒
		beatCounter = 0;
		Score = 0;
		time=0;
		//初始化音乐
		music = gameObject.GetComponent<AudioSource>();
		string audioFileName = "Musics/"+ Path.GetFileName(Directory.GetParent(beatmap.beatmapFileName).FullName)+"/"+beatmap.audioFileName;
		AudioClip audioClip = Resources.Load<AudioClip>(audioFileName.Split('.')[0]);
		music.clip = audioClip;

		gameOverAnim = canvas.GetComponent<Animator>();
		isEnd = false;

		image.gameObject.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		if(time>30&&!isPlaying) {
			music.Play();
			isPlaying=true;
			Debug.Log("song begin time:"+time);
		}
		time+=Time.deltaTime*1000;
		ScoreText.text = "Score : " + Score;
		createNote();
		if(isPlaying && !music.isPlaying)
			{
			gameOverAnim.SetTrigger("GameOver");
			mainCamera.GetComponent<Animator>().SetTrigger("GameOver");
			isEnd = true;
			image.gameObject.SetActive(true);
			}
	}

	void createNote()
	{
		if(beatCounter < beatmap.beats.Count){
			for(float beatTime = beatmap.beats[beatCounter].hitTime;beatTime - timeOffset <= GameController.time && beatCounter < beatmap.beats.Count;){
				
				float x = -0.22f, y = 0.03f;
				float zRotation = 0f;

				//judge the column
				switch(beatmap.beats[beatCounter].column){
				case 0:
					x = -0.22f;
					y = 0.03f;
					zRotation = 90f;
					break;

				case 1:
					x = -0.12f;
					y = -0.1f;
					break;

				case 2:
					x = 0.12f;
					y = -0.1f;
					break;

				case 3:
					x = 0.22f;
					y = 0.03f;
					zRotation = 90f;
					break;
				}

				//the beats position and rotation
				Vector3 beatsPosition = new Vector3(x,y,zPoint);
				Vector3 beatsRotationVector = new Vector3(0f,0f,zRotation);
				Quaternion beatsRotation = Quaternion.Euler(beatsRotationVector);
	
				//create the beats
				Instantiate(beats,beatsPosition,beatsRotation);

				//let the beatTime equal to next beat's time
				if(beatCounter < beatmap.beats.Count - 1)beatTime = beatmap.beats[++beatCounter].hitTime;
				else ++beatCounter;
			}
		}
	}
}
