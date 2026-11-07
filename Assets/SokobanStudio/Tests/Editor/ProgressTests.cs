using System;
using System.IO;
using NUnit.Framework;
namespace SokobanStudio.Tests {
public class ProgressTests {
 [Test] public void UnlockReplayAndReload(){var folder=Path.Combine(Path.GetTempPath(),"SokobanProgress_"+Guid.NewGuid().ToString("N"));try{var path=Path.Combine(folder,"progress.json");var p=new PresetProgress(path);Assert.IsTrue(p.Unlocked(1));Assert.IsFalse(p.Unlocked(2));for(int i=1;i<=PresetProgress.LevelCount;i++){Assert.IsTrue(p.Unlocked(i));p.Complete(i);p.Complete(i);Assert.IsTrue(p.Completed(i));}var loaded=new PresetProgress(path);for(int i=1;i<=PresetProgress.LevelCount;i++)Assert.IsTrue(loaded.Completed(i));Assert.IsFalse(loaded.Unlocked(PresetProgress.LevelCount+1));Assert.IsNull(loaded.Error);}finally{if(Directory.Exists(folder))Directory.Delete(folder,true);}}
 [Test] public void SaveFailureRetainsMemoryProgress(){var file=Path.GetTempFileName();try{var p=new PresetProgress(Path.Combine(file,"progress.json"));p.Complete(1);Assert.IsTrue(p.Completed(1));Assert.IsTrue(p.Unlocked(2));Assert.IsNotNull(p.Error);}finally{File.Delete(file);}}
}
}
