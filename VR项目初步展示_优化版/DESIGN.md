# DESIGN.md — VR项目初步展示（优化版）

## 一、需求对齐（Step 1）

- **主题**：《古堡寻秘》VR 密室逃脱游戏的期末项目初步展示
- **目标**：课堂向老师展示，明确回答两件事——「想做什么内容」「用到什么 VR 技术」
- **受众**：任课老师与同班同学；场合为课堂初步展示（非答辩）
- **页数**：9 页（保留初稿全部核心信息：组员 / 主题玩法 / VR 技术 / 项目进度）
- **调性**：科技感、沉浸感、探索神秘感；专业但不死板

## 二、全局视觉约定

### ① 画布与安全区
- 画布 1280×720，padding 上下左右 60px；内容区 1160×600（x 60–1220，y 60–660）
- 页尾 y=670，字号 14px，全篇固定在左下角
- 四区：页眉 y 60–110 / 标题 y 110–180 / 正文 y 190–650 / 页脚 y 670

### ② 配色（科技未来 · 科技深空，全篇一套）
- 主色 accent：`#63A7D4`（冷蓝，发光边/编号/强调）
- 强调深档 accent_t：`#0F1920`；浅档 accent_l：`#B9D7EC`
- 底色 bg：`#0B1521`（深空蓝黑）
- 深底 deep_bg：`#0B1521` / on_deep：`#FFFFFF`
- 文字四级：ink `#FFFFFF` / secondary `#A9CFE7` / body `#E7E8E9` / muted `#C9CCCE`
- 卡片底 surface：`#232C37`；分隔线 line：`#60676F`
- 图表系列色：#24428F #3A5CBC #63A7D4 #8FBCE4 #E3F1F8 #13263B
- 点缀色：冷蓝紫 `#4DA3FF`（发光）；古堡金 `#C9A24B`（仅金币/宝藏语义，极少量）
- 色彩面积：主色 ≤25% / 辅色 ≤30% / 强调 ≤10%；Hero 页主色+点缀可到 20%

### ③ 字体系统
- 中文标题：思源黑体 Bold；中文正文：思源黑体 Regular
- 西文/数字：Barlow Condensed（仅标题/大数字）
- 字号阶梯：封面主标 84 / 章节大字 64 / 巨型锚点数字 96 / 页面主标 36 / 卡片小标 24 / 正文 20 / 标签 16 / 页脚 14

### ④ 视觉母题
- 每页标题下方一条贯通的 1px 细线（accent 35% 混底色，约 `#2C4459`），仅落标题一处
- 页脚左下固定 `古堡寻秘 · VR密室逃脱` + 右侧页码 `0X / 09`
- 内容页用左侧竖排小编号（01–04）+ 冷蓝色块做栏目锚点

### ⑤ 配图清单（全篇 3D 游戏渲染风格，统一冷蓝 + 古堡金光）
| 文件 | 用途 | 生成方式 | 真实内容 | 使用页 |
| --- | --- | --- | --- | --- |
| `cover.png` | 主角/氛围 | ImageGen | 幽暗古堡长廊 + 蓝色魔法光 + VR 沉浸感，无文字 | P1、P9 |
| `castle_scene.png` | 主角 | ImageGen | 第一人称 VR 视角古堡大厅，悬浮金币与谜题门，游戏渲染 | P4 |
| `vr_hands.png` | 证据 | ImageGen | VR 控制器双手与发光密码锁机关交互，科技冷蓝 | P7 |

配图规则：主角图风格全篇统一为 3D 游戏渲染；压图文字一律先加深色蒙层（rgba(3,10,20,0.55–0.72)）保证 ≥4.5:1；图片不自带文字/Logo。

## 三、逐页设计

### P1 封面
- title: 古堡寻秘 · VR密室逃脱游戏 / type: cover / role: hero / rhythm: peak
- layout: 满版图 + 渐变蒙层（cover.png 出血）
- visual: L1 cover.png 满版；文字压深蒙层下部，主标 84px 白字，副标「虚拟现实技术 · 期末项目初步展示」20px secondary
- density: 字数约 45 / 图片 1 张 / 留白约 45%
- anti_pattern: 禁止形状/图标拼贴主视觉；禁止文字投影

### P2 目录
- title: 目录 CONTENTS / type: catalog / role: supporting / rhythm: transition
- layout: 经典纵排（4 条：01 项目概述 / 02 玩法与机制 / 03 VR技术应用 / 04 团队与进度）
- visual: 无图；超大编号 Barlow Condensed + 章节名 + 1px 行分隔线
- density: 字数约 60 / 图片 0 / 留白约 55%
- anti_pattern: 禁止卡片容器 / 行底色填充；禁止写章节简述

### P3 组员（团队介绍）
- title: 项目团队 / type: content / role: supporting / rhythm: valley
- layout: 左标题色块 + 右内容（m）—— 左 30% 深蓝大标题块「TEAM / 5 位成员」，右 70% 两个班级分组名单
- visual: 数字媒体技术 2 班（邓广毅、杨创沛）、3 班（邓广政、阮子耀、刘广鸿）；user 图标 + 姓名卡
- density: 字数约 90 / 图片 0 / 留白约 35%
- anti_pattern: 禁止 5 张等宽卡片横排；禁止只写名字无班级分组

### P4 我们要做什么 — 项目主题
- title: 我们要做什么 / type: content / role: supporting / rhythm: valley
- layout: 左图右文（6:4，左 58% castle_scene.png，右 42% 文字）
- visual: L1 castle_scene.png；右侧「VR密室逃脱游戏《古堡寻秘》」+ 基于 3D URP 模板 + 一句话定位
- density: 字数约 160 / 图片 1 张 / 留白约 20%
- anti_pattern: 禁止 50:50 等分；禁止主视觉缩成小图

### P5 核心玩法与机制
- title: 核心玩法与机制 / type: content / role: supporting / rhythm: peak
- layout: 流程链（g）—— 5 阶段横向：进入古堡探索 → VR控制器交互 → 收集金币 → 遭遇怪物/受击扣血 → 胜负判定
- visual: 发光节点 + 箭头连接；底部输出条「胜利：收集金币 / 失败：生命值 ≤ 0」
- density: 字数约 180 / 图片 0 / 留白约 25%
- anti_pattern: 禁止等宽卡片横排冒充流程；禁止阶段 >6

### P6 用到什么 VR 技术（一）— 技术总览
- title: 用到什么 VR 技术 / type: content / role: supporting / rhythm: valley
- layout: 卡片组 + 底部结论条（d）—— 4 张非等高卡：XR控制器 / 交互机关 / 角色与场景交互 / UI面板
- visual: 每张卡 FAIcon 锚点（gamepad/puzzle-key/hand-pointer/desktop）+ 标题 + 一句说明；结论条「XR 交互 + 机关逻辑 + UI 构成完整 VR 体验」
- density: 字数约 220 / 图片 0 / 留白约 20%
- anti_pattern: 禁止 4 张完全等高均分；禁止把结论写进每张卡

### P7 用到什么 VR 技术（二）— 交互机关详解
- title: 交互机关与角色交互 / type: content / role: supporting / rhythm: valley
- layout: 左图右文（a，左 vr_hands.png 42%，右 58% 机关清单）
- visual: L2 vr_hands.png；右侧 4 条——Socket 拼接机关 / Combo Lock 密码锁 / Slider 滚动条 / 铰链机构开门 + Grab 持续抓取杀怪
- density: 字数约 200 / 图片 1 张 / 留白约 18%
- anti_pattern: 禁止 50:50 等分；禁止机关清单无事件监听说明

### P8 项目进度
- title: 项目进度 / type: content / role: supporting / rhythm: valley
- layout: 对比式（h）—— 左「已完成」/ 右「下一步」，中间竖线分隔，两区严格对齐
- visual: 已完成：场景搭建 + 基础游戏功能/机制实现；下一步：结合课上知识（Combo Lock 密码锁、铰链机关开门等）
- density: 字数约 150 / 图片 0 / 留白约 30%
- anti_pattern: 禁止两区高度不对等；禁止用箭头伪装成流程

### P9 结尾
- title: 谢谢观看 / Q&A / type: ending / role: hero / rhythm: peak
- layout: 满版图 + 渐变蒙层收尾（复用 cover.png）
- visual: cover.png 出血 + 深蒙层；大字「谢谢观看」+ 小字「Q & A · 欢迎提问」
- density: 字数约 30 / 图片 1 张 / 留白约 55%
- anti_pattern: 禁止只写「谢谢」占满整页无落款；禁止加页尾

## 四、完成检查
- 封面/结尾 Hero 共 2 页（22%）；呼吸页 P2 目录 + P9 结尾；明暗节奏深底统一、靠图片与发光制造反差
- 相邻页版式不重复；非对称页面占比 ≥40%
- 每页正文 ≥18px，标题 ≥36px；文字色 ≤4 种；强调色每页 ≤2–3 处
- 全篇 3D 渲染图风格统一；压图文字均加蒙层
