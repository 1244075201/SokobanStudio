using System.Collections.Generic;
using System.Linq;
namespace SokobanStudio {
public sealed class EditorHistory {
 struct Change {public LevelData before,after;}
 readonly List<Change> undo=new List<Change>();readonly Stack<Change> redo=new Stack<Change>();LevelData pending;
 public bool CanUndo=>undo.Count>0;public bool CanRedo=>redo.Count>0;public int Count=>undo.Count;
 public void Clear(){undo.Clear();redo.Clear();pending=null;}
 public void Begin(LevelData d){if(pending==null)pending=d.Clone();}
 public void Commit(LevelData d){if(pending==null)return;if(!MapEquals(pending,d)){undo.Add(new Change{before=pending,after=d.Clone()});if(undo.Count>100)undo.RemoveAt(0);redo.Clear();}pending=null;}
 public bool Undo(LevelData d){Commit(d);if(!CanUndo)return false;var c=undo[undo.Count-1];undo.RemoveAt(undo.Count-1);redo.Push(c);CopyMap(c.before,d);return true;}
 public bool Redo(LevelData d){Commit(d);if(!CanRedo)return false;var c=redo.Pop();undo.Add(c);CopyMap(c.after,d);return true;}
 static void CopyMap(LevelData a,LevelData b){b.terrain=(Terrain[])a.terrain.Clone();b.player=a.player;b.boxes=(Cell[])a.boxes.Clone();b.goals=(Cell[])a.goals.Clone();}
 public static bool MapEquals(LevelData a,LevelData b)=>a.width==b.width&&a.height==b.height&&a.player.Equals(b.player)&&a.terrain.SequenceEqual(b.terrain)&&new HashSet<Cell>(a.boxes).SetEquals(b.boxes)&&new HashSet<Cell>(a.goals).SetEquals(b.goals);
 public static bool EqualsSaved(LevelData a,LevelData b)=>a!=null&&b!=null&&a.id==b.id&&a.name==b.name&&MapEquals(a,b);
}
}
