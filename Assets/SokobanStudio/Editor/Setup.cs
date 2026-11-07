using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
namespace SokobanStudio { public static class Setup {
 public static void CreateScene(){var scene=EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);new GameObject("Sokoban Game",typeof(GameBootstrap));EditorSceneManager.SaveScene(scene,"Assets/Scenes/Game.unity");EditorBuildSettings.scenes=new[]{new EditorBuildSettingsScene("Assets/Scenes/MainMenu.unity",true),new EditorBuildSettingsScene("Assets/Scenes/Game.unity",true),new EditorBuildSettingsScene("Assets/Scenes/LevelEditor.unity",true)};AssetDatabase.SaveAssets();}
}}


