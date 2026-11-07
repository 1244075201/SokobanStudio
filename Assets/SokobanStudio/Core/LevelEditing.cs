using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
namespace SokobanStudio {
public enum PaintTool { Wall, Floor, Box, Goal, Player, Erase }
public static class DraftRules {
 public static readonly Cell MissingPlayer=new Cell(-1,-1);
 public static LevelData Create(string name,int width,int height){if(string.IsNullOrWhiteSpace(name))throw new ArgumentException("Enter a name.");if(width<6||width>20||height<6||height>15)throw new ArgumentException("Width must be 6-20; height must be 6-15.");var d=new LevelData{id=Guid.NewGuid().ToString("N"),name=name.Trim(),width=width,height=height,terrain=new Terrain[width*height],player=MissingPlayer,boxes=new Cell[0],goals=new Cell[0]};for(int y=0;y<height;y++)for(int x=0;x<width;x++)if(x==0||y==0||x==width-1||y==height-1)d.terrain[y*width+x]=Terrain.Wall;return d;}
 public static List<string> Validate(LevelData d,bool playable){var e=new List<string>();if(d==null){e.Add("Missing level.");return e;}if(string.IsNullOrWhiteSpace(d.name))e.Add("Enter a name.");if(d.width<6||d.width>20||d.height<6||d.height>15||d.terrain==null||d.terrain.Length!=(long)d.width*d.height){e.Add("Invalid map dimensions or terrain size.");return e;}if(d.terrain.Any(t=>!Enum.IsDefined(typeof(Terrain),t)))e.Add("Unknown terrain.");if(d.boxes==null||d.goals==null){e.Add("Missing object arrays.");return e;}if(d.player.Equals(MissingPlayer)){if(playable)e.Add("Place exactly one player.");}else if(!d.Walkable(d.player))e.Add("Player must be on floor inside the map.");if(d.boxes.Any(c=>!d.Walkable(c)))e.Add("A box is outside the floor.");if(d.goals.Any(c=>!d.Walkable(c)))e.Add("A goal is outside the floor.");if(d.boxes.Distinct().Count()!=d.boxes.Length)e.Add("Duplicate boxes.");if(d.goals.Distinct().Count()!=d.goals.Length)e.Add("Duplicate goals.");if(d.boxes.Contains(d.player))e.Add("Player overlaps a box.");if(playable){if(d.boxes.Length==0)e.Add("Place at least one box.");if(d.boxes.Length!=d.goals.Length)e.Add("Box and goal counts must match.");}return e;}
 public static bool Paint(LevelData d,Cell c,PaintTool tool,out string error){error=null;if(c.x<0||c.y<0||c.x>=d.width||c.y>=d.height)return false;int i=c.y*d.width+c.x;var boxes=d.boxes.ToList();var goals=d.goals.ToList();bool player=d.player.Equals(c);bool changed=false;
 switch(tool){case PaintTool.Wall:changed=d.terrain[i]!=Terrain.Wall||boxes.Contains(c)||goals.Contains(c)||player;d.terrain[i]=Terrain.Wall;boxes.Remove(c);goals.Remove(c);if(player)d.player=MissingPlayer;break;
 case PaintTool.Floor:changed=d.terrain[i]!=Terrain.Floor;d.terrain[i]=Terrain.Floor;break;
 case PaintTool.Erase:if(boxes.Remove(c))changed=true;else if(player){d.player=MissingPlayer;changed=true;}else if(goals.Remove(c))changed=true;else if(d.terrain[i]!=Terrain.Floor){d.terrain[i]=Terrain.Floor;changed=true;}break;
 default:if(!d.Walkable(c)){error="Place objects on floor.";return false;}if(tool==PaintTool.Goal){if(!goals.Contains(c)){goals.Add(c);changed=true;}}else if(tool==PaintTool.Box){if(player){error="A box cannot overlap the player.";return false;}if(!boxes.Contains(c)){boxes.Add(c);changed=true;}}else {if(boxes.Contains(c)){error="The player cannot overlap a box.";return false;}changed=!player;d.player=c;}break;}
 d.boxes=boxes.ToArray();d.goals=goals.ToArray();return changed;}
 public static IEnumerable<Cell> Line(Cell a,Cell b){int x=a.x,y=a.y,dx=Math.Abs(b.x-x),dy=-Math.Abs(b.y-y),sx=x<b.x?1:-1,sy=y<b.y?1:-1,err=dx+dy;while(true){yield return new Cell(x,y);if(x==b.x&&y==b.y)break;int twice=2*err;if(twice>=dy){err+=dy;x+=sx;}if(twice<=dx){err+=dx;y+=sy;}}}
}
[Serializable] public class DraftFile {public int schemaVersion=1;public LevelData level;}
public sealed class CustomLevelRepository {
 readonly string root;public CustomLevelRepository(string root){this.root=root;}
 string PathFor(string id){if(!Guid.TryParseExact(id,"N",out _))throw new ArgumentException("Invalid custom level ID.");return Path.Combine(root,id+".json");}
 public static string Serialize(LevelData d){var e=DraftRules.Validate(d,false);if(e.Count>0)throw new ArgumentException(string.Join("\n",e));return JsonUtility.ToJson(new DraftFile{level=d},true);}
 public static LevelData Deserialize(string json){var f=JsonUtility.FromJson<DraftFile>(json);if(f==null||f.schemaVersion!=1)throw new ArgumentException("Unsupported custom file version.");var e=DraftRules.Validate(f.level,false);if(e.Count>0)throw new ArgumentException(string.Join("\n",e));return f.level;}
 public void Save(LevelData d){var json=Serialize(d);var path=PathFor(d.id);Directory.CreateDirectory(root);var temp=path+".tmp";try{File.WriteAllText(temp,json);if(File.Exists(path))File.Replace(temp,path,null);else File.Move(temp,path);}finally{if(File.Exists(temp))File.Delete(temp);}}
 public LevelData Load(string id){var d=Deserialize(File.ReadAllText(PathFor(id)));if(d.id!=id)throw new ArgumentException("File ID mismatch.");return d;}
 public void Delete(string id){File.Delete(PathFor(id));}
 public sealed class Entry {public string id,label,error;public DateTime modified;}
 public List<Entry> List(){var result=new List<Entry>();if(!Directory.Exists(root))return result;foreach(var path in Directory.GetFiles(root,"*.json")){var entry=new Entry{id=Path.GetFileNameWithoutExtension(path)};try{entry.modified=File.GetLastWriteTime(path);var d=Load(entry.id);entry.label=d.name+"  ("+d.width+" x "+d.height+")  "+entry.modified.ToString("yyyy-MM-dd HH:mm");}catch(Exception ex){entry.label=entry.id;entry.error=ex.Message;}result.Add(entry);}return result.OrderByDescending(e=>e.modified).ToList();}
}
}
