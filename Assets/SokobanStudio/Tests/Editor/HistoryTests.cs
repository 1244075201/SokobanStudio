using NUnit.Framework;
namespace SokobanStudio.Tests {
public class HistoryTests {
 [Test] public void StrokeUndoRedoKeepsMetadata(){var d=DraftRules.Create("Original",10,8);var h=new EditorHistory();h.Begin(d);DraftRules.Paint(d,new Cell(2,2),PaintTool.Wall,out _);DraftRules.Paint(d,new Cell(3,2),PaintTool.Wall,out _);h.Commit(d);Assert.AreEqual(1,h.Count);d.name="Renamed";Assert.IsTrue(h.Undo(d));Assert.AreEqual(Terrain.Floor,d.terrain[22]);Assert.AreEqual("Renamed",d.name);Assert.IsTrue(h.Redo(d));Assert.AreEqual(Terrain.Wall,d.terrain[23]);}
 [Test] public void NoOpBranchLimitAndSavedBaseline(){var d=DraftRules.Create("Map",6,6);var saved=d.Clone();var h=new EditorHistory();h.Begin(d);h.Commit(d);Assert.IsFalse(h.CanUndo);for(int i=0;i<105;i++){h.Begin(d);DraftRules.Paint(d,new Cell(2,2),i%2==0?PaintTool.Wall:PaintTool.Floor,out _);h.Commit(d);}Assert.AreEqual(100,h.Count);h.Undo(d);Assert.IsTrue(EditorHistory.EqualsSaved(saved,d));h.Begin(d);DraftRules.Paint(d,new Cell(3,3),PaintTool.Player,out _);h.Commit(d);Assert.IsFalse(h.CanRedo);Assert.IsFalse(EditorHistory.EqualsSaved(saved,d));}
}
}
