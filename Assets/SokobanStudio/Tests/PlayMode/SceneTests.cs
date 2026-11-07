using System.Collections;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;
namespace SokobanStudio.Tests {
public class SceneTests { PresetProgress original;string temp; [SetUp] public void Setup(){original=Navigation.Progress;temp=System.IO.Path.Combine(System.IO.Path.GetTempPath(),"SokobanUI_"+System.Guid.NewGuid().ToString("N"));Navigation.Progress=new PresetProgress(System.IO.Path.Combine(temp,"progress.json"));} [TearDown] public void Cleanup(){Navigation.Progress=original;if(System.IO.Directory.Exists(temp))System.IO.Directory.Delete(temp,true);}
 [UnityTest] public IEnumerator SceneStartsAndDisplaysWin(){Navigation.Selected="level_01";SceneManager.LoadScene("Game");yield return null;yield return null;var game=Object.FindObjectOfType<GameBootstrap>();Assert.NotNull(game.State);Assert.AreEqual(1,Object.FindObjectsOfType<Canvas>().Length);foreach(var d in new[]{new Cell(-1,0),new Cell(-1,0),new Cell(0,-1),new Cell(0,-1)})Assert.IsTrue(game.State.Move(d));game.SendMessage("Refresh");yield return new WaitForSecondsRealtime(.4f);Assert.IsTrue(Object.FindObjectsOfType<Text>().Any(t=>t.text.Contains("MISSION COMPLETE")));game.Undo();Assert.IsFalse(game.State.Won);game.Restart();Assert.AreEqual(0,game.State.Moves);}
 [UnityTest] public IEnumerator MainMenuHasUniqueCanvasAndEntryButtons(){Navigation.SelectPage=false;SceneManager.LoadScene("MainMenu");yield return null;yield return null;Assert.AreEqual(1,Object.FindObjectsOfType<Canvas>().Length);var buttons=Object.FindObjectsOfType<Button>();Assert.IsTrue(buttons.Any(b=>b.name=="Start Game"));buttons.First(b=>b.name=="Start Game").onClick.Invoke();yield return null;Assert.AreEqual(8,Object.FindObjectsOfType<Button>().Count(b=>b.GetComponentsInChildren<Text>().Any(t=>t.text.Contains("AVAILABLE")||t.text.Contains("LOCKED")||t.text.Contains("COMPLETED"))));}
}
}


