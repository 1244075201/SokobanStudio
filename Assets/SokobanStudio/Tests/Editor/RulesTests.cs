using System;
using NUnit.Framework;
using UnityEngine;
namespace SokobanStudio.Tests {
public class RulesTests {
 LevelData Preset(string id)=>LevelJson.Load(Resources.Load<TextAsset>("PresetLevels").text,id);
 Cell Dir(char c)=>c=='L'?new Cell(-1,0):c=='R'?new Cell(1,0):c=='U'?new Cell(0,-1):new Cell(0,1);
 [TestCase("level_01","LLUU",4,2)] 
 [TestCase("level_02","RDRRUUULRDDDLLUUUUDDLLUUURR",27,4)] 
 [TestCase("level_03","LUUURLDDRRUURUL",15,5)] 
 [TestCase("level_04","DLURRRDLULLDDRULURUULDRDDRRULDLUULUR",36,9)] 
 [TestCase("level_05","UURURRDDLUURULLLLLULLDDRDRRDRUU",31,12)] 
 [TestCase("level_06","LLULUURDDDRDLDLUUUURULLL",24,11)] 
 [TestCase("level_07","UURRRRDRURULLLDLLDDRRRDDLDRR",28,13)] 
 [TestCase("level_08","RRRDRRRUULLLDLUUULURDRRRURDDDDDLDR",34,18)] 
[TestCase("level_09","UUUURRRDDLDLUULURDDRRRDRUURUL",29,8)]
[TestCase("level_10","UURRRDRUUDRRUULLDDRRDRUURULL",28,11)]
[TestCase("level_11","DLDLLURRRURRDDLLLULLDDRULURR",28,10)]
[TestCase("level_12","RDRRDDRDDLLULLDLURRRDLLRRRRUULUULLDDUURRDDRDDLLULL",50,11)]
[TestCase("level_13","RUULLLULDRRRRDDLURULLLDDLLLUURRDRDLUUURDD",41,13)]
[TestCase("level_14","DLUULLLDDRURURRUULLDDRRDDLULLDLLUUUURDRD",40,12)]
[TestCase("level_15","LLDURRDLURRRDLDDRRUDLLUUULLDRLDRUULLLDRURRDDLDLLUDRRURUULDURRRDL",64,11)]
 public void SolutionsUndoAndRestart(string id,string moves,int count,int pushes){var d=Preset(id);var original=LevelJson.Serialize(d);var s=new GameState(d);foreach(char c in moves)Assert.IsTrue(s.Move(Dir(c)));Assert.IsTrue(s.Won);Assert.AreEqual(count,s.Moves);Assert.AreEqual(pushes,s.Pushes);Assert.IsFalse(s.Move(Dir('L')));Assert.IsTrue(s.Undo());Assert.IsFalse(s.Won);Assert.IsTrue(s.Move(Dir(moves[moves.Length-1])));for(int i=0;i<count;i++)Assert.IsTrue(s.Undo());Assert.AreEqual(d.player,s.Player);Assert.AreEqual(0,s.Moves);Assert.AreEqual(0,s.Pushes);Assert.IsFalse(s.Undo());foreach(char c in moves)s.Move(Dir(c));s.Restart();Assert.AreEqual(d.player,s.Player);CollectionAssert.AreEquivalent(d.boxes,s.Boxes);Assert.AreEqual(0,s.Moves);Assert.AreEqual(0,s.Pushes);Assert.IsFalse(s.Undo());Assert.AreEqual(original,LevelJson.Serialize(d));}
 [Test] public void JsonRoundTrip(){var d=Preset("level_02");Assert.AreEqual(LevelJson.Serialize(d),LevelJson.Serialize(LevelJson.Deserialize(LevelJson.Serialize(d))));}
 [Test] public void WallDoesNotCount(){var s=new GameState(Preset("level_01"));Assert.IsFalse(s.Move(Dir('R')));Assert.AreEqual(0,s.Moves);Assert.IsFalse(s.Undo());}
 LevelData Map(params string[] rows)=>LevelJson.Convert(new SourceLevel{id="test",name="test",width=rows[0].Length,height=rows.Length,rows=rows});
 [Test] public void TwoBoxesCannotBePushed(){var s=new GameState(Map("PBBGG"));Assert.IsFalse(s.Move(Dir('R')));Assert.AreEqual(0,s.Pushes);}
 [Test] public void BoxCannotLeaveMap(){var s=new GameState(Map("GPB"));Assert.IsFalse(s.Move(Dir('R')));}
 [Test] public void VoidBlocks(){var s=new GameState(Map("P BG"));Assert.IsFalse(s.Move(Dir('R')));Assert.IsFalse(s.Move(Dir('L')));}
 [Test] public void GoalsRemainAfterBoxMoves(){var d=Map("P*G.B.");var s=new GameState(d);Assert.IsTrue(s.Move(Dir('R')));Assert.IsTrue(s.IsGoal(new Cell(1,0)));Assert.IsTrue(s.IsGoal(s.Player));Assert.IsTrue(s.Undo());Assert.IsTrue(s.HasBox(new Cell(1,0)));}
 [Test] public void PlayerOnGoalAndIsolation(){var d=Map("+B.");var s=new GameState(d);d.terrain[2]=Terrain.Wall;Assert.IsTrue(s.Move(Dir('R')));Assert.IsTrue(s.IsGoal(new Cell(0,0)));}
 [TestCase("PPBG")][TestCase("PBBG")][TestCase("P?BG")][TestCase("....")]
 public void InvalidMapsRejected(string row){Assert.Throws<ArgumentException>(()=>Map(row));}
 [Test] public void InvalidDimensionsAndOverlap(){Assert.Throws<ArgumentException>(()=>Map("PBG",".."));var d=Preset("level_01");d.player=d.boxes[0];Assert.Throws<ArgumentException>(()=>d.Validate());}
}
}

