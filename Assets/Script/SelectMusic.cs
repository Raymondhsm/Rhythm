using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using UnityEngine.SceneManagement;
using System.IO;
using osu_beatmap_Csharp;


public class SelectMusic : MonoBehaviour
{
    public Image musicBtn;
    private int musicNum;
    private ScrollRect scrollRect;
    private Vector3 startPosition = new Vector3(640, 360, 0); //实例化预制体的position，可自定义
    private Quaternion startRotation = new Quaternion(0, 0, 0, 0);//实例化预制体的rotation，可自定义
    private Vector3 startContainerPosition;

    private AudioSource music;

    private  int imageCounter;
    //用于动画计时
    private float timer;

    private List<Song> songs = new List<Song>();

    void Start()
    {
        startContainerPosition = new Vector3(0, startPosition.y - 360, startPosition.z);
        Transform parent = GameObject.Find("Canvas/SelectMusicPanelContainer/SelectMusicPanel").transform;
        scrollRect = transform.GetComponent<ScrollRect>();
        Image temp;

        //从文件中读取音乐
        songs = ReadSongsFromFile.initSongs();

        musicNum = songs.Count;
        for (int i = 0; i < musicNum; i++)
        {
            temp = Instantiate(musicBtn, startPosition, startRotation) as Image;
            Debug.Log(songs[i].beatmaps[0].beatmapFileName);
            string musicImgPath = "Musics/"+songs[i].dirName+"/"+songs[i].beatmaps[0].bgImg;
            musicImgPath = musicImgPath.Split('.')[0];//musicImgPath.LastIndexOf('.')+1);
            temp.sprite = Resources.Load<Sprite>(musicImgPath);

            temp.name = "MusicImg" + i.ToString();
            temp.transform.SetParent(parent);
            startPosition = new Vector3(startPosition.x + (i==0?270:220), startPosition.y, startPosition.z);
            if(i==0) temp.GetComponent<RectTransform>().sizeDelta= new Vector2 (300f, 300f);
        }
        
        imageCounter = 0;

        //初始化音乐
		music = gameObject.GetComponent<AudioSource>();
        string audioFileName = "Musics/"+ Path.GetFileName(Directory.GetParent(songs[0].beatmaps[0].beatmapFileName).FullName)+"/"+songs[0].beatmaps[0].audioFileName;
	    AudioClip audioClip = Resources.Load<AudioClip>(audioFileName.Split('.')[0]);
		music.clip = audioClip;
        music.Play();
        // Debug.Log(musicNum);
        // Debug.Log(startPosition);
        // Debug.Log(startContainerPosition);
        //re.transform.SetPositionAndRotation(a, b);

        timer = 0;
    }

    void Update()
    {
        // Debug
        if (Input.GetKeyDown(KeyCode.LeftArrow)) PressRight();
        if (Input.GetKeyDown(KeyCode.RightArrow)) PressLeft();
        if(Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return)) chooseSong(imageCounter);
        timer += Time.deltaTime;
        
    }

    public void chooseSong(int index){
        if(index == imageCounter){
            enterGame();
        }
    }

    public void enterGame(){
        PlayerPrefs.SetString("beatmapFileName",songs[imageCounter].beatmaps[0].beatmapFileName);
        SceneManager.LoadScene(1);
    }

    public void PressLeft()
    {

        if(imageCounter==musicNum-1)return;

        GameObject image;
        Vector2 big =new Vector2(270,0);
        Vector2 small = new Vector2(220,0);
        for(int i=0;i<musicNum;i++){
            image = GameObject.Find("Canvas/SelectMusicPanelContainer/SelectMusicPanel/MusicImg"+i);
            if(i==imageCounter){
                image.gameObject.GetComponent<RectTransform>().anchoredPosition -= big;
                image.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(200,200);
            }
            else if(i==imageCounter+1){
                image.gameObject.GetComponent<RectTransform>().anchoredPosition -= big;
                image.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(300,300);
            }else{
                image.gameObject.GetComponent<RectTransform>().anchoredPosition -= small;
            }
        }
        imageCounter++;
        changeMusic();
    }

    public void PressRight()
    {
        if(imageCounter == 0)return;

        GameObject image;
        Vector2 big =new Vector2(270,0);
        Vector2 small = new Vector2(220,0);
        for(int i=0;i<musicNum;i++){
            image = GameObject.Find("Canvas/SelectMusicPanelContainer/SelectMusicPanel/MusicImg"+i);
            if(i==imageCounter){
                image.gameObject.GetComponent<RectTransform>().anchoredPosition += big;
                image.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(200,200);
            }
            else if(i==imageCounter-1){
                image.gameObject.GetComponent<RectTransform>().anchoredPosition += big;
                image.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(300,300);
            }else{
                image.gameObject.GetComponent<RectTransform>().anchoredPosition += small;
            }
        }
        imageCounter--;
        changeMusic();
    }


    public void changeMusic()
    {
        string audioFileName = "Musics/"+ Path.GetFileName(Directory.GetParent(songs[imageCounter].beatmaps[0].beatmapFileName).FullName)+"/"+songs[imageCounter].beatmaps[0].audioFileName;
	    AudioClip audioClip = Resources.Load<AudioClip>(audioFileName.Split('.')[0]);
		music.clip = audioClip;
        music.Play();
    }
}