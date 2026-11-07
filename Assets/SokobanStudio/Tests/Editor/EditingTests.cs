using System;
using System.IO;
using NUnit.Framework;
namespace SokobanStudio.Tests {
public class EditingTests {
 LevelData New()=>DraftRules.Create("Draft",10,8);
 void Paint(LevelData d,int x,int y,PaintTool t){DraftRules.Paint(d,new Cell(x,y),t,out _);}
 [Test] public void DraftIsSaveableButNotPlayable(){var d=New();Assert.IsEmpty(DraftRules.Validate(d,false));Assert.IsNotEmpty(DraftRules.Validate(d,true));var copy=CustomLevelRepository.Deserialize(CustomLevelRepository.Serialize(d));Assert.AreEqual(DraftRules.MissingPlayer,copy.player);}
 [Test] public void LayersAndUniquePlayer(){var d=New();Paint(d,2,2,PaintTool.Goal);Paint(d,2,2,PaintTool.Box);Paint(d,2,2,PaintTool.Box);Assert.AreEqual(1,d.boxes.Length);Paint(d,2,2,PaintTool.Player);Assert.AreEqual(DraftRules.MissingPlayer,d.player);Paint(d,3,3,PaintTool.Player);Paint(d,4,3,PaintTool.Player);Assert.AreEqual(new Cell(4,3),d.player);Paint(d,2,2,PaintTool.Erase);Assert.IsEmpty(d.boxes);Assert.AreEqual(1,d.goals.Length);Paint(d,2,2,PaintTool.Erase);Assert.IsEmpty(d.goals);Paint(d,4,3,PaintTool.Wall);Assert.AreEqual(DraftRules.MissingPlayer,d.player);}
 [Test] public void FloorPreservesObjectsAndWallClearsLayers(){var d=New();Paint(d,2,2,PaintTool.Goal);Paint(d,2,2,PaintTool.Box);Paint(d,2,2,PaintTool.Floor);Assert.AreEqual(1,d.boxes.Length);Paint(d,2,2,PaintTool.Wall);Assert.IsEmpty(d.boxes);Assert.IsEmpty(d.goals);Paint(d,2,2,PaintTool.Erase);Assert.IsTrue(d.Walkable(new Cell(2,2)));}
 [Test] public void PreviewDoesNotChangeDraft(){var d=New();Paint(d,2,2,PaintTool.Player);Paint(d,3,2,PaintTool.Box);Paint(d,4,2,PaintTool.Goal);var before=CustomLevelRepository.Serialize(d);var state=new GameState(d);Assert.IsTrue(state.Move(new Cell(1,0)));Assert.IsTrue(state.Won);state.Undo();state.Restart();Assert.AreEqual(before,CustomLevelRepository.Serialize(d));}
 [Test] public void FastStrokeIncludesIntermediateCells(){CollectionAssert.AreEqual(new[]{new Cell(1,1),new Cell(2,1),new Cell(3,1),new Cell(4,1)},DraftRules.Line(new Cell(1,1),new Cell(4,1)));}
 [Test] public void RepositoryRoundTripOverwriteDeleteAndCorruptFile(){var root=Path.Combine(Path.GetTempPath(),"SokobanTests_"+Guid.NewGuid().ToString("N"));try{var repo=new CustomLevelRepository(root);var d=New();repo.Save(d);d.name="Renamed";repo.Save(d);Assert.AreEqual("Renamed",repo.Load(d.id).name);File.WriteAllText(Path.Combine(root,Guid.NewGuid().ToString("N")+".json"),"broken");Assert.AreEqual(2,repo.List().Count);Assert.AreEqual(1,repo.List().FindAll(e=>e.error!=null).Count);repo.Delete(d.id);Assert.AreEqual(1,repo.List().Count);}finally{if(Directory.Exists(root))Directory.Delete(root,true);}}
 [Test] public void InvalidStructureRejected(){var d=New();d.boxes=new[]{new Cell(0,0)};Assert.IsNotEmpty(DraftRules.Validate(d,false));Assert.Throws<ArgumentException>(()=>CustomLevelRepository.Serialize(d));}
}
}
