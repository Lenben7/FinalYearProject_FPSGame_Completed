# Final Year Project — 第一人称 PVE 射击游戏

一个用 **Unity + C#** 独立完成的毕业设计项目：第一人称 PVE 射击，包含三把可切换武器、四种状态的敌人 AI、波次生存关卡、任务流程引导与本地最高分存档。

> 仓库仅包含 **C# 源码**（29 个脚本），不含场景、模型、音频等资源文件。运行方式见文末「如何运行」。

---

## 功能一览

**玩家**
- `CharacterController` 移动、跳跃、按住 Shift 奔跑，重力与地面检测
- 鼠标视角控制
- 血量系统：受伤时全屏血屏特效（透明度渐隐）、血量条随比例变色（绿 → 黄 → 红）
- 死亡流程：切镜、死亡动画、结算面板、延迟返回主菜单

**武器**
- 三把武器：`1911`（手枪）、`M16A4`（步枪）、`MP5`（冲锋枪）
- 三种射击模式：单发 / 三连发 / 全自动
- 开镜（ADS）：腰射与开镜使用**两套独立的散布参数**，开镜时隐藏准星、切换后坐力动画
- 换弹：弹匣与备弹分开计数，按实际剩余备弹数量补充
- 射击表现：枪口粒子、后坐力动画、分武器音效、空仓提示音

**弹药与投掷物**
- 弹药箱分三类：手枪弹、步枪弹、通用弹药
- 手雷（致命）与烟雾弹（战术），每类上限 3 个
- 投掷采用**蓄力机制**：按住 `G` / `T` 累积力度（上限 2 倍），松手按当前力度投出

**敌人 AI**
- 基于 `StateMachineBehaviour` 的四状态机：`Idle` → `Patroling` → `Chase` → `Attack`
- 巡逻：从 Waypoint 集合中随机取点，到达后自动切换下一个点，超时则转入 Idle
- 索敌与脱离采用**半径滞回**：18m 内开始追击，超过 21m 才放弃追击
- 追击：`NavMeshAgent` 寻路 + `LookAt` 朝向玩家，进入 2.5m 转入攻击
- 死亡表现：随机播两套死亡动画之一、关闭碰撞体、延迟销毁尸体

**关卡与流程**
- 波次生存：协程控制刷怪节奏，每波敌人数量按倍率递增，波与波之间有冷却倒计时
- 任务系统：`TaskManager` 驱动四段式引导（Tutorial → Awakening → Encounter → Survival），
  通过切换任务文本、按阶段启用刷怪器来串联关卡节奏
- 存档：`PlayerPrefs` 持久化本地最高分，结算时与历史最高分比对

**交互**
- 屏幕中心射线（`ViewportPointToRay`）10m 内检测可交互物体
- 命中物体自动高亮描边，按 `F` 拾取
- 支持拾取武器、弹药箱、医疗包、投掷物四类

**音频**
- `SoundManager` 分轨管理：玩家音轨、敌人音轨、武器音轨，避免互相打断

---

## 操作说明

| 按键 | 功能 |
| --- | --- |
| `W` `A` `S` `D` | 移动 |
| `Shift`（按住） | 奔跑 |
| `Space` | 跳跃 |
| 鼠标移动 | 视角 |
| 鼠标左键 | 射击 |
| 鼠标右键（按住） | 开镜 ADS |
| `R` | 换弹 |
| `1` / `2` | 切换武器槽 |
| `F` | 拾取 / 交互 |
| `G`（按住蓄力，松开投掷） | 手雷 |
| `T`（按住蓄力，松开投掷） | 烟雾弹 |
| `Esc` | 退出游戏 |

---

## 目录结构

```
.
├── Enemy/            敌人本体、生成控制器、特殊敌人（OrdinarySoldier）
├── Enemy_Status/     敌人 AI 的四个状态类（Idle / Patrol / Chase / Attack）
├── Manager/          全局管理器（HUD、交互、音效、任务、武器、任务切换）
├── Player/           玩家移动、视角、血量与死亡流程
├── Tool/             道具与工具类（门、存档、主菜单、黑屏过渡、自销毁等）
└── Weapon/           武器基类、子弹、弹药箱、医疗包、投掷物
```

---

## 技术要点

**1. 用 Animator 状态机承载 AI 逻辑**

每个状态继承 `StateMachineBehaviour`，在 `OnStateEnter / OnStateUpdate / OnStateExit` 里写逻辑，
通过 Animator 的 `bool` 参数（`isPatroling` / `isChasing` / `isAttacking`）驱动切换。
好处是状态切换和动画天然同步，调参在 Animator 面板里可视化；代价是逻辑分散、调试链路较长。

**2. 弹道计算**

从屏幕中心发射 `ViewportPointToRay`，取命中点作为目标点；未命中则取射线上 100m 处的点。
用「目标点 − 枪口位置」得到基准方向，再叠加 `Random.Range(-spread, spread)` 的随机偏移，
最后以 `Rigidbody.AddForce(ForceMode.Impulse)` 发射实体子弹，3 秒后销毁。
散布置随开镜状态在 `hipSpreadIntensity` / `adsSpreadIntensity` 之间切换。

**3. 追击判定的滞回设计**

`Chase` 状态里退出条件是距离 > 21m，而 `Patroling` 状态里进入条件是距离 < 18m。
两个阈值不相等的意义在于：玩家正好站在边界线上来回移动时，不会出现敌人反复"追—停—追"的抖动。

---

## 如何运行

仓库只提供脚本，**没有可直接运行的工程**。要跑起来需要自行补齐：

1. 新建 Unity 工程（建议 **Unity 2021.3 LTS 及以上**，版本：*（待补充实际使用版本）*）
2. 将六个目录放入 `Assets/` 下
3. 安装依赖：
   - `TextMeshPro`（任务文本、HUD、结算界面）
   - `Outline` 描边组件（交互高亮使用）
4. 场景侧配置：
   - Tag：`Player`、`EnemyHand`
   - Layer：`groundMask`、`WeaponRender`
   - 敌人预制体上挂 `Animator`（需含上述四个状态与对应 `bool` 参数）、`NavMeshAgent`、`Enemy`
   - 烘焙 NavMesh，放置 Waypoint 集群并赋给 `Enemy.waypointCluster`
   - 场景中放置 `HUDManager`、`SoundManager`、`InteractionManager`、`WeaponManager`、`TaskManager`、`SaveLoadManager`、`GlobalReferences`

---

## 已知限制与可改进方向

- **子弹未做对象池**：目前每次开火 `Instantiate`、3 秒后 `Destroy`。同屏弹量上来后 GC 压力会明显，
  后续可改为对象池复用。
- **单例偏多**：各 Manager 大量使用 `Instance`，耦合较高。可考虑引入事件总线或轻量依赖注入。
- **输入使用旧版 Input Manager**：迁移到新版 Input System 后更易做按键重绑定与手柄支持。
- **任务流程硬编码**：`TaskManager` 中任务文本与触发点写死在代码里，可改为 ScriptableObject 配置。
- **AI 状态间缺少共享上下文**：四个状态各自 `FindGameObjectWithTag` 查找玩家，可收敛到一个黑板对象。

---

## 说明

本项目为个人毕业设计，代码与美术资源仅用于学习与交流。
