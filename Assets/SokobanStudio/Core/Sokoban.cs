using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace SokobanStudio {
[Serializable] public struct Cell : IEquatable<Cell> {
 public int x,y; public Cell(int x,int y){this.x=x;this.y=y;}
 public bool Equals(Cell b)=>x==b.x&&y==b.y;
 public override bool Equals(object b)=>b is Cell c&&Equals(c);
 public override int GetHashCode()=>x*397^y;
 public static Cell operator +(Cell a,Cell b)=>new Cell(a.x+b.x,a.y+b.y);
}
public enum Terrain { Floor, Wall, Void }
[Serializable] public class LevelData {
 public string id,name; public int width,height; public Terrain[] terrain; public Cell player; public Cell[] boxes,goals;
 public bool Walkable(Cell c)=>c.x>=0&&c.y>=0&&c.x<width&&c.y<height&&terrain[c.y*width+c.x]==Terrain.Floor;
 public LevelData Clone()=>new LevelData{id=id,name=name,width=width,height=height,terrain=(Terrain[])terrain.Clone(),player=player,boxes=(Cell[])boxes.Clone(),goals=(Cell[])goals.Clone()};
 public void Validate(){
  if(width<=0||height<=0||terrain==null||terrain.Length!=(long)width*height)throw new ArgumentException("Invalid map dimensions.");
  if(terrain.Any(t=>!Enum.IsDefined(typeof(Terrain),t)))throw new ArgumentException("Unknown terrain.");
  if(boxes==null||goals==null||boxes.Length==0||boxes.Length!=goals.Length)throw new ArgumentException("At least one box and an equal number of goals are required.");
  if(!Walkable(player)||boxes.Any(c=>!Walkable(c))||goals.Any(c=>!Walkable(c)))throw new ArgumentException("An object is outside the playable floor.");
  if(boxes.Distinct().Count()!=boxes.Length||goals.Distinct().Count()!=goals.Length||boxes.Contains(player))throw new ArgumentException("Illegal object overlap.");
 }
}
[Serializable] public class SourceLevel { public string id,name; public int width,height; public string[] rows; }
[Serializable] public class SourcePack { public int schemaVersion; public SourceLevel[] levels; }
public static class LevelJson {
 public static string Serialize(LevelData data){data.Validate();return JsonUtility.ToJson(data,true);}
 public static LevelData Deserialize(string json){var d=JsonUtility.FromJson<LevelData>(json);if(d==null)throw new ArgumentException("Missing level.");d.Validate();return d;}
 public static LevelData Load(string json,string id){var pack=JsonUtility.FromJson<SourcePack>(json);if(pack==null||pack.schemaVersion!=1||pack.levels==null)throw new ArgumentException("Unsupported level pack.");var s=pack.levels.SingleOrDefault(l=>l.id==id);if(s==null)throw new ArgumentException("Level not found: "+id);return Convert(s);}
 public static LevelData Convert(SourceLevel s){
  if(s.width<=0||s.height<=0||s.rows==null||s.rows.Length!=s.height)throw new ArgumentException("Invalid row count.");
  var d=new LevelData{id=s.id,name=s.name,width=s.width,height=s.height,terrain=new Terrain[s.width*s.height]};var boxes=new List<Cell>();var goals=new List<Cell>();int players=0;
  for(int y=0;y<s.height;y++){if(s.rows[y]==null||s.rows[y].Length!=s.width)throw new ArgumentException("Invalid row width.");for(int x=0;x<s.width;x++){char ch=s.rows[y][x];var c=new Cell(x,y);if("#.PBG*+ ".IndexOf(ch)<0)throw new ArgumentException("Unknown map character.");d.terrain[y*s.width+x]=ch=='#'?Terrain.Wall:ch==' '?Terrain.Void:Terrain.Floor;if(ch=='P'||ch=='+'){d.player=c;players++;}if(ch=='B'||ch=='*')boxes.Add(c);if(ch=='G'||ch=='*'||ch=='+')goals.Add(c);}}
  if(players!=1)throw new ArgumentException("Exactly one player is required.");d.boxes=boxes.ToArray();d.goals=goals.ToArray();d.Validate();return d;
 }
}
public sealed class GameState {
 readonly LevelData initial; readonly Stack<Snapshot> history=new Stack<Snapshot>(); HashSet<Cell> boxes;
 struct Snapshot { public Cell player;public HashSet<Cell> boxes;public int moves,pushes; }
 public Cell Player{get;private set;} public int Moves{get;private set;} public int Pushes{get;private set;}
 public IEnumerable<Cell> Boxes=>boxes.ToArray(); public bool HasBox(Cell c)=>boxes.Contains(c);public bool IsGoal(Cell c)=>Array.IndexOf(initial.goals,c)>=0;
 public bool Won=>initial.goals.All(boxes.Contains);
 public GameState(LevelData data){data.Validate();initial=data.Clone();Restart();}
 public void Restart(){Player=initial.player;boxes=new HashSet<Cell>(initial.boxes);Moves=Pushes=0;history.Clear();}
 public bool Move(Cell direction){
  if(Won||Math.Abs(direction.x)+Math.Abs(direction.y)!=1)return false;
  var next=Player+direction;var beyond=next+direction;bool push=boxes.Contains(next);
  if(!initial.Walkable(next)||(push&&(!initial.Walkable(beyond)||boxes.Contains(beyond))))return false;
  history.Push(new Snapshot{player=Player,boxes=new HashSet<Cell>(boxes),moves=Moves,pushes=Pushes});
  if(push){boxes.Remove(next);boxes.Add(beyond);Pushes++;}Player=next;Moves++;return true;
 }
 public bool Undo(){if(history.Count==0)return false;var s=history.Pop();Player=s.player;boxes=s.boxes;Moves=s.moves;Pushes=s.pushes;return true;}
}
}
