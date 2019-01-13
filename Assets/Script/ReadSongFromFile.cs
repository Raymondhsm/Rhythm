using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.IO;
using UnityEngine;

namespace osu_beatmap_Csharp
{
    public class Song
    {
        public string dirName;//歌曲目录
        public List<Beatmap> beatmaps = new List<Beatmap>();//歌曲谱图
    };

    public class Beatmap
    {
        public string beatmapFileName;//谱面文件名字

        //[General]
        public string audioFileName;//MP3文件名字
        public int mode;//模式，osu的太鼓达人模式还是钢琴模式，为3则是钢琴模式
        public int previewTime;//ms,选中曲子后开始播放曲子的时间

        //[[Metadata]]
        public string title;//曲子名字
        public string artist;//艺术家
        public string version;//难度，beginner,easy......

        //[Difficulty]
        public double HPDrainRate;//掉血速度
        public double circleSize;//轨道数量
        public double overallDifficulty;//用于计算得分
                                     /*
                                     Score	Hit Window
                                     50	150ms + 50ms * (5 - OD) / 5
                                     100	100ms + 40ms * (5 - OD) / 5
                                     300	50ms + 30ms * (5 - OD) / 5
                                     */
        public double approachRate;//到达速率,用于计算视觉中beat开始出现的时间
                                   /*
                                   The circle starts fading in at X - preempt with:

                                   When AR < 5: preempt = 1200ms + 600ms * (5 - AR) / 5
                                   When AR = 5: preempt = 1200ms
                                   When AR > 5: preempt = 1200ms - 750ms * (AR - 5) / 5
                                   The amount of time it takes for the hit object to completely fade in is also reliant on the approach rate:

                                   When AR < 5: fade_in = 800ms + 400ms * (5 - AR) / 5
                                   When AR = 5: fade_in = 800ms
                                   When AR > 5: fade_in = 800ms - 500ms * (AR - 5) / 5
                                   */

        //[Events]
        public string bgImg;//背景图片

        //[HitObjects]
        public List<Beat> beats = new List<Beat>();//谱图的拍子
    }

    public struct Beat
    {
        public int column;//第几条轨道
        public int hitTime;//ms,hit的时间
        public int type;//beat的类型，长按还是点一下
        public int endTime;//ms,如果type是长按的话，是长按结束的时间，其余为0;
    };

    public class ReadSongsFromFile
    {

        public static List<Song> initSongs()
        {
            string[] dirs = Directory.GetDirectories(Application.dataPath+"/Resources/Musics");
            //每一个歌曲的目录
            List<Song> songs = new List<Song>();
            foreach (string songDir in dirs)
            {
                Console.WriteLine(songDir);
                Song song = new Song();
                song.dirName = Path.GetFileName(songDir);
                string[] beatmapFiles = Directory.GetFiles(songDir, "*.osu");
                //歌曲里面的每一个谱面
                foreach(string beatmapFile in beatmapFiles)
                {
                    Console.WriteLine(beatmapFile);
                    Beatmap beatmap = new Beatmap();
                    // beatmap.beatmapFileName = Path.GetFileName(beatmapFile);
                    //相对于Assets的路径
                    beatmap.beatmapFileName = beatmapFile;
                    readBeatmap(beatmap);
                    //判断是否为符合条件的谱面
                    if (beatmap.mode == 3 && beatmap.circleSize == 4)
                        song.beatmaps.Add(beatmap);
                }
                if (song.beatmaps.Count > 0)
                    songs.Add(song);
            }
            return songs;
            //showBeatmap(songs);
            //readHitObject(songs[0].beatmaps[0]);
            //showHitObjects(songs[0].beatmaps[0]);
        }

        public static void readBeatmap(Beatmap beatmap)
        {
            try
            {
                // 创建一个 StreamReader 的实例来读取文件 
                // using 语句也能关闭 StreamReader
                using (StreamReader sr = new StreamReader(beatmap.beatmapFileName))
                {
                    string head = "", line;

                    // 从文件读取并显示行，直到文件的末尾 
                    while ((line = sr.ReadLine()) != null)
                    {
                        //head作为每一段的标志头
                        //if (line.Equals("[General]") || line.Equals("[Metadata]") || line.Equals("[Difficulty]") || line.Equals("[HitObjects]"))
                        if (Regex.IsMatch(line,"\\[*\\]"))
                        {
                            head = line;
                            continue;
                        }
                        //Console.WriteLine(line);
                        if(head.Equals("[HitObjects]"))
                            break;


                        //[General]
                        if (head.Equals("[General]"))
                        {
                            string[] elements = Regex.Split(line, ": ", RegexOptions.IgnoreCase);
                            switch (elements[0])
                            {
                                case "AudioFilename":
                                    beatmap.audioFileName = elements[1];
                                    break;
                                case "PreviewTime":
                                    beatmap.previewTime = int.Parse(elements[1]);
                                    break;
                                case "Mode":
                                    beatmap.mode = int.Parse(elements[1]);
                                    break;
                                default:
                                    break;
                            }
                        }
                        //[Metadata]
                        if (head.Equals("[Metadata]"))
                        {
                            string[] elements = line.Split(':');
                            switch (elements[0])
                            {
                                case "TitleUnicode":
                                    beatmap.title = elements[1];
                                    break;
                                case "ArtistUnicode":
                                    beatmap.artist = elements[1];
                                    break;
                                case "Version":
                                    beatmap.version = elements[1];
                                    break;
                                default:
                                    break;
                            }
                        }


                        //[Difficulty]
                        if (head.Equals("[Difficulty]"))
                        {
                            string[] elements = line.Split(':');
                            switch (elements[0])
                            {
                                case "HPDrainRate":
                                    beatmap.HPDrainRate = double.Parse(elements[1]);
                                    break;
                                case "CircleSize":
                                    beatmap.circleSize = double.Parse(elements[1]);
                                    break;
                                case "OverallDifficulty":
                                    beatmap.overallDifficulty = double.Parse(elements[1]);
                                    break;
                                case "ApproachRate":
                                    beatmap.approachRate = double.Parse(elements[1]);
                                    break;
                                default:
                                    break;
                            }
                        }
                        //[Events]
                        if (head.Equals("[Events]"))
                        {
                            if(line.Equals("//Background and Video events"))
                            {
                                
                                line = sr.ReadLine();
                                string[] elements = line.Split(',');
                                beatmap.bgImg = elements[2].Substring(1, elements[2].Length - 2);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                // 向用户显示出错消息
                Console.WriteLine("The file could not be read: "+ beatmap.beatmapFileName);
                Console.WriteLine(e.Message);
            }
        }

        public static void readHitObject(Beatmap beatmap)
        {
            try
            {
                // 创建一个 StreamReader 的实例来读取文件 
                // using 语句也能关闭 StreamReader
                using (StreamReader sr = new StreamReader(beatmap.beatmapFileName))
                {
                    string head = "", line;

                    // 从文件读取并显示行，直到文件的末尾 
                    while ((line = sr.ReadLine()) != null)
                    {
                        //head作为每一段的标志头
                        if (line.Equals("[HitObjects]"))
                        {
                            head = line;
                            continue;
                        }
                        //Console.WriteLine(line)
                        //[HitObjects]
                        if (head.Equals("[HitObjects]"))
                        {
                            string[] elements = line.Split(',');
                            Beat beat;
                            //列
                            int x = int.Parse(elements[0]);
                            beat.column = x / (512 / 4);
                            //hitTime
                            beat.hitTime = int.Parse(elements[2]);
                            //type
                            beat.type = int.Parse(elements[3]);
                            //endTime
                            if (beat.type != 128)
                                beat.endTime = 0;
                            else beat.endTime = int.Parse(elements[5].Split(':')[0]);
                            beatmap.beats.Add(beat);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                // 向用户显示出错消息
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }
        }

        public static Beatmap readFullBeatmap(string beatmapFileName)
        {
            Beatmap beatmap = new Beatmap();
            beatmap.beatmapFileName = beatmapFileName;
            readBeatmap(beatmap);
            readHitObject(beatmap);
            return beatmap;
        }
        public static void showBeatmap(List<Song> songs)
        {
            foreach(Song song in songs)
            {
                Console.WriteLine("**songDirName:" + song.dirName);
                Console.WriteLine("songName: " + song.beatmaps[0].title);
                Console.WriteLine("artist: " + song.beatmaps[0].artist);

                for(int i=0;i<song.beatmaps.Count;i++)
                {
                    Console.WriteLine("谱图" + i + ":");
                    Console.WriteLine("audioFileName: "+song.beatmaps[i].audioFileName);
                    Console.WriteLine("mode: " + song.beatmaps[i].mode);
                    Console.WriteLine("circleSize: " + song.beatmaps[i].circleSize);
                    Console.WriteLine("previewTime: "+song.beatmaps[i].previewTime);
                    Console.WriteLine("version: "+ song.beatmaps[i].version);
                    Console.WriteLine("HPDrainRate: " + song.beatmaps[i].HPDrainRate);
                    Console.WriteLine("overallDifficulty: "+ song.beatmaps[i].overallDifficulty);
                    Console.WriteLine("approachRate: "+ song.beatmaps[i].approachRate);
                    Console.WriteLine("bgImg: " + song.beatmaps[i].bgImg);
                    Console.WriteLine();
                }
                Console.WriteLine();
                Console.WriteLine();
            }

        }

        public static void showHitObjects(Beatmap beatmap)
        {
            foreach (Beat beat in beatmap.beats)
            {
                Console.WriteLine(beat.column + "," + beat.type + "," + beat.hitTime + "," + beat.endTime);
            }
        }

    }
}
