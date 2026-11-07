using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
namespace SokobanStudio {
[Serializable] public class ProgressData { public int version=1;public List<string> completed=new List<string>(); }
public sealed class PresetProgress {
 public const int LevelCount=15;
 readonly string path;ProgressData data=new ProgressData();public string Error{get;private set;}
 public PresetProgress(string path){this.path=path;try{if(File.Exists(path)){var loaded=JsonUtility.FromJson<ProgressData>(File.ReadAllText(path));if(loaded==null||loaded.version!=1||loaded.completed==null)throw new Exception("Invalid progress file.");data=loaded;}}catch(Exception e){Error="Progress load failed: "+e.Message;}}
 public bool Completed(int number)=>data.completed.Contains(Id(number));
 public bool Unlocked(int number)=>number>=1&&number<=LevelCount&&(number==1||Completed(number)||Completed(number-1));
 public static string Id(int number)=>"level_"+number.ToString("00");
 public void Complete(int number){if(number<1||number>LevelCount)return;string id=Id(number);if(!data.completed.Contains(id))data.completed.Add(id);try{Directory.CreateDirectory(Path.GetDirectoryName(path));string temp=path+".tmp";File.WriteAllText(temp,JsonUtility.ToJson(data,true));if(File.Exists(path))File.Replace(temp,path,null);else File.Move(temp,path);Error=null;}catch(Exception e){Error="Progress save failed: "+e.Message;}}
}
}
