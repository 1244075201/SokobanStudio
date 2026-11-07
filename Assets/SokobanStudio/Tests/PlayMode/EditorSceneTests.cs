using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
namespace SokobanStudio.Tests {
public class EditorSceneTests {
 [UnityTest] public IEnumerator PreviewReturnsUnmodifiedDraft(){SceneManager.LoadScene("LevelEditor");yield return null;yield return null;var editor=Object.FindObjectOfType<LevelEditorController>();Assert.NotNull(editor);var d=DraftRules.Create("Preview test",10,8);DraftRules.Paint(d,new Cell(2,2),PaintTool.Player,out _);DraftRules.Paint(d,new Cell(3,2),PaintTool.Box,out _);DraftRules.Paint(d,new Cell(4,2),PaintTool.Goal,out _);editor.Open(d);var before=CustomLevelRepository.Serialize(editor.Draft);editor.BeginPreview();Assert.NotNull(editor.Preview);Assert.IsTrue(editor.Preview.Move(new Cell(1,0)));Assert.IsTrue(editor.Preview.Won);Assert.IsTrue(editor.Preview.Undo());editor.Preview.Restart();editor.ReturnToEditor();yield return null;Assert.IsNull(editor.Preview);Assert.AreEqual(before,CustomLevelRepository.Serialize(editor.Draft));}
}
}
