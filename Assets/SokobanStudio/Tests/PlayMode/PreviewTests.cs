using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;
namespace SokobanStudio.Tests {
public class PreviewTests {
 [UnityTest] public IEnumerator PreviewPreservesDataAndHasCorrectSize(){var d=DraftRules.Create("Preview",20,15);DraftRules.Paint(d,new Cell(1,1),PaintTool.Goal,out _);DraftRules.Paint(d,new Cell(1,1),PaintTool.Box,out _);var before=CustomLevelRepository.Serialize(d);var go=new GameObject("Preview",typeof(RectTransform));go.AddComponent<LevelPreview>().Bind(d);var image=go.GetComponent<RawImage>();Assert.AreEqual(320,image.texture.width);Assert.AreEqual(240,image.texture.height);Assert.IsFalse(image.raycastTarget);Assert.AreEqual(before,CustomLevelRepository.Serialize(d));Object.Destroy(go);yield return null;}
}
}
