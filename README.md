# Castle_Mystery_Hunt
## 项目说明
### 1. 仓库说明
此项目仓库目前创建了7个 Branch 分支，main 分支是主分支，在所有功能模块都实现了之后，将最终版本合并到main上，在此之前不要对main分支里的内容进行改动，作为初版备份使用。

develop 分支是我们主要操作的分支，在此分支之下我已经按照组员名字首字母创建好了每个人对应的分支，请在分工完成之后在各自的分支内进行相应工作并将改动上传到云端，在各自确认无误后将改动合并到 develop 分支下。
（注：不要擅自修改他人分支下的内容，进行分支合并前后请先在群内告知并确认各自进度）

对于使用 Git 和 GitHub 进行版本控制的流程不了解的可以在B站上学习以下两个视频：  
【【Unity版本控制】小组如何使用Git、GitHub、Sourcetree管理项目version control】 [https://www.bilibili.com/video/BV1eZ4y1P752/?share_source=copy_web&vd_source=62ad5f832a76c2c42613b51df35edef5](https://www.bilibili.com/video/BV1eZ4y1P752/?share_source=copy_web&vd_source=62ad5f832a76c2c42613b51df35edef5)  
【手把手教你 git 和 sourcetree】 [https://www.bilibili.com/video/BV1UZ2XBjEpV/?share_source=copy_web&vd_source=62ad5f832a76c2c42613b51df35edef5](https://www.bilibili.com/video/BV1UZ2XBjEpV/?share_source=copy_web&vd_source=62ad5f832a76c2c42613b51df35edef5)

### 2. 游戏说明
目前我已经搭建好了一个基于 3D URP 模板的游戏场景，实现的功能如下：
1) 游戏玩家血条和金币数量的全局显示：血条始终位于 Game 窗口的左上角，金币数在右上角进行记录，默认初始最大血量 hp 为 100，金币数 coin 为 0。
2) 怪物对象的自动寻路及伤害判定：怪物分为静态怪物 Gargoyle 和动态怪物 Ghost，由 StaticEnemyManager 和 MoblieEnemyManager 进行统一批量管理，静态怪物固定在场景中，动态怪物则按照设定的坐标进行寻路，沿固定路径巡逻。怪物攻击逻辑：持续对前方自身触发器范围内发射射线并进行射线检测，若击中玩家对象则默认一次性扣除玩家 25 血量，默认攻击间隔为 1 秒。
3) 血包金币对象拾取：场景中分布的血包及金币由 MedicalManager 和 NewCoinManager 进行统一批量管理，玩家靠近血包和金币可以通过触发器逻辑进行自动拾取，每拾取一个血包默认恢复 50 血量，玩家血量不会超出设定的血量上限；拾取一个金币默认增加100金币值。
4) 失败/胜利条件判定：当玩家血量小于等于0时判定玩家死亡，游戏失败，隐藏血条及金币值，播放失败音效并将失败图片 FailBackground 全屏显示， 1 秒后重启游戏场景，重新开始关卡；当玩家金币值达到 500 并且抵达游戏终点时判定游戏胜利，隐藏血条及金币值，播放胜利音效并将胜利图片 GameExitBackground 全屏显示 , 1 秒后显示退出文字 " Press Esc to Exit... "。

#### 数值配置

| 参数 | 默认值 | 说明 |
| :--- | :---: | :--- |
| 初始血量 / 血量上限 | 100 / 100 | 玩家出生状态 |
| 怪物单次伤害 | 25 | 每次射线判定命中 |
| 怪物攻击间隔 | 1 秒 | 同一怪物两次伤害的最小间隔 |
| 血包回复量 | 50 | 受血量上限限制 |
| 单枚金币价值 | 100 | — |
| 胜利所需金币 | 500 | 约 5 枚金币 |
| 失败 / 胜利淡入时长 | 1 秒 | 全屏图片淡入时间 |

#### 场景主要对象

| 对象 | 作用 |
| :--- | :--- |
| JohnLemon | 玩家角色，挂载移动与属性脚本 |
| Gargoyle | 静态怪物，固定视野检测 |
| Ghost | 动态怪物，沿路径巡逻 |
| Medical | 血包道具，触发器拾取回血 |
| Coin | 金币道具，触发器拾取加钱 |
| GameEnding | 终点触发器与胜负结算逻辑 |
| Canvas / Game | HUD 容器（血条、金币） |
| Canvas / FailBackgorund | 失败全屏图 |
| Canvas / GameExitBackground | 胜利全屏图 |

#### 功能模块与脚本对应

| 模块 | 主要脚本 | 职责 |
| :--- | :--- | :--- |
| 玩家移动 | `PlayerMovement` | 读取输入、移动、转向、驱动动画 |
| 玩家属性 | `PlayerAttributes` | 血量、金币数据与受伤 / 回血 / 加金币接口 |
| 怪物视野 | `Observer` | 触发器内射线检测、按间隔造成伤害 |
| 怪物巡逻 | `WayPointsPatrol` | NavMeshAgent 沿路径点巡逻 |
| 拾取 | `Coin` / `Medical` | 触发器拾取并调用属性接口 |
| UI 与结算 | `GameEnding` | 刷新 HUD、失败 / 胜利流程、重开关卡 |

### 3.VR相关
1.将原场景中的玩家角色 JohnLemon 及其移动脚本替换为由 XR 控制器实现角色移动。可新建一个空物体作为XR控制器的子对象，将挂载在 JohnLemon 身上的碰撞体脚本复制粘贴到此空物体上，实现与场景上的触发器的交互。  
2.原场景中用于实现相机跟随的 MainCamera 和 Virtual Camera 可删除，由 XR Origin内置相机作为新的主相机。  
3.可以在有门的地方制作如：拼接机关 Socket、密码锁 Combo Lock、滚动条 Slider 等逻辑并添加事件监听，当 拼接/解锁/滑动 完成后通过铰链机构实现开门。  
4.也可以添加如持续抓取 Grab 某道具，可杀死触发器范围内的怪物等功能模块。
