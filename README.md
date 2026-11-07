# Sokoban Studio · 机器人仓库

一款面向 Windows PC 的 2D 推箱子解谜游戏。玩家操控仓库机器人，把货箱逐一推到存储平台。项目包含 15 个预设关卡和可在游戏内使用的关卡编辑器。

## 运行工程

1. 安装 Unity **2022.3.51f1c1**，在 Unity Hub 中添加包含 `Assets`、`Packages`、`ProjectSettings` 的项目目录。
2. 等待包管理器按工程清单恢复 Universal 2D / URP、UGUI 和 Unity Test Framework，完成资源导入与 C# 编译。
3. 打开 `Assets/Scenes/MainMenu.unity`，点击 Play。选择 **Start Game** 进入关卡选择，或通过 **Level Editor** 管理自定义地图。

## 预设关卡与操作

选关卡片显示实际地图缩略图。通关解锁下一关，可以重复挑战已完成的关卡。进度保存在 `Application.persistentDataPath/preset-progress.json`。

| 操作 | 按键 |
| --- | --- |
| 移动或推动相邻的一个箱子 | WASD 或方向键 |
| 撤销一步，恢复位置和计数 | Z 或 Ctrl+Z |
| 重开当前关卡 | R |
| 暂停游戏 | Esc |

箱子与目标可以重叠。全部箱子归位后，结算窗口显示步数和推动次数。

## 关卡编辑器

从 **New** 创建宽 6–20、高 6–15 的地图。选择工具后单击或拖动，绘制地板、墙体、目标、货箱和玩家起点。Ctrl+Z / Ctrl+Y 按整次笔划撤销与重做。

**Save** 可以保存未完成的草稿。**Validate** 检查起点、箱子和目标数量及摆放合法性。通过校验后点击 **Playtest**，使用与正式关卡相同的规则试玩。点击 **Return to Editor** 返回原始布局。自定义地图以 JSON 保存在 `Application.persistentDataPath/CustomLevels`，可从 **My Levels** 加载、编辑或删除。

## 工程结构

| 目录 | 职责 |
| --- | --- |
| `Assets/Scenes` | 主菜单、预设游戏、关卡编辑器场景 |
| `Assets/SokobanStudio/Core` | 分层关卡数据、网格移动规则、撤销历史、校验与存档 |
| `Assets/SokobanStudio/Data/Resources` | 15 关 JSON 配置与参考解法 |
| `Assets/SokobanStudio/Runtime` | 输入、棋盘显示、菜单、编辑器、程序化图形、音乐与音效 |
| `Assets/SokobanStudio/Tests` | EditMode 与 PlayMode 测试 |

运行时状态从关卡数据复制，移动、推动和撤销由同一套 C# 规则处理。编辑器试玩使用独立状态，绘图历史与游戏步数分别管理。

## 截图与测试

游戏截图待补充。15 个预设关卡已通过核心规则解法回放，核对了通关、步数、推动次数、连续撤销和重开。开发期间已在 Unity 编辑器中人工游玩。完整 Test Runner 结果、独立 Windows 构建，以及在全新环境中的存档和界面验收仍待确认。测试入口为 Window → General → Test Runner。
