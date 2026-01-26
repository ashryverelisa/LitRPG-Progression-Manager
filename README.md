# LitRPG World & Progression Manager

A consistency & progression tool for LitRPG authors

## Core Purpose

Help LitRPG writers:

- Design game systems (stats, skills, XP curves)
- Track character progression across chapters
- Prevent math errors & stat inconsistencies
- Generate system messages & tables automatically

**Think:** Scrivener + RPG system editor + sanity checker

---

## Core Modules

### World Rules Editor

This is the heart of the system.

#### Stats Definition

Create base stats (STR, AGI, INT, VIT, LUCK)

**Derived stats:**
- `HP = VIT × 12 + Level × 5`
- `Mana = INT × 10`
- Min/max constraints
- Growth per level

**UI Idea:**
- Spreadsheet-like grid
- Formula editor with validation
- Live preview of calculated values

#### XP & Level Curves

- XP required per level
- Multiple curve types:
  - Linear
  - Exponential
  - Custom formula
- Visual graph of progression

**Avalonia Strength:**
- Charting with SkiaSharp
- Reactive updates when formulas change

---

### Skill & Ability System

#### Skill Definitions

- Active / Passive
- Cooldowns
- Scaling formulas
- Status effects

**Example:**
```
Fireball:
  Damage = (INT × 2.5) + SkillLevel × 10
  Mana Cost = 30 + SkillLevel × 5
  Cooldown = max(1, 10 - SkillLevel)
```

#### Skill Trees

- Prerequisites
- Branching evolution paths
- Skill ranks
- Class-locked skills

**UI:**
- Node-based editor
- Zoomable canvas
- Click-to-edit nodes

---

### Classes, Races & Templates

#### Class Templates

- Base stat modifiers
- Allowed equipment
- Starting skills
- Level-up bonuses

#### Races

- Passive bonuses
- Growth modifiers
- Racial evolutions

**Example:**
```
Elf → High Elf → Star Elf
```

---

### Character Progression Tracker

This is where it becomes gold for authors.

#### Per-Character Tracking

- Level
- Stats per chapter
- Skills learned/unlocked
- Equipment changes

#### Chapter Timeline

| Chapter | Level | HP  | STR | Skills Gained    |
|---------|-------|-----|-----|------------------|
| 12      | 9     | 430 | 18  | Fireball II      |
| 13      | 10    | 490 | 20  | Mana Regen       |

**Feature:**
- Jump to any chapter and see exact stats
- Compare two chapters side-by-side

---

### Consistency Checker

This is the killer feature.

#### Auto-Detect Problems

- Stat jumps without explanation
- XP gained > possible XP
- Skill used before unlocking
- Cooldown violations
- HP/Mana exceeding max

**Output Example:**
```
⚠ Chapter 37: Mana used (220) exceeds max mana (180)
```

---

### System Message Generator

Auto-generate believable LitRPG UI text.

#### Templates

```
[SYSTEM]: You have reached Level {Level}.
[SYSTEM]: {SkillName} has reached Rank {Rank}.
[SYSTEM]: New Class Unlocked: {ClassName}
```

#### Export

- Plain text
- Markdown
- Copy-paste ready for manuscripts

---

##  Technology Stack

- **Framework:** Avalonia UI (cross-platform .NET UI framework)
- **Architecture:** MVVM (Model-View-ViewModel)
- **Language:** C# / .NET

---

##  Getting Started

### Prerequisites

- .NET SDK (version 10.0)
- Compatible with Windows, macOS, and Linux

### Building the Project

```bash
cd src
dotnet restore
dotnet build
```

### Running the Application

```bash
cd src/ProgressionManager
dotnet run
```

---

##  Project Structure

```
src/
├── ProgressionManager.sln
└── ProgressionManager/
    ├── Models/          # Data models and business logic
    ├── ViewModels/      # MVVM view models
    ├── Views/           # UI views (AXAML)
    └── Assets/          # Icons and resources
```

---

##  Current Features

- **World Rules Management:** Define and manage game system rules
- **Navigation System:** Easy access to different modules
- **View Models:** Characters, Classes, Races, Skills, Timeline, World Rules, Validation

---

## 🗺️ Roadmap

- [ ] Complete World Rules Editor with formula validation
- [ ] Implement XP curve designer with visual graphs
- [ ] Build skill tree node editor
- [ ] Create character progression timeline
- [ ] Add consistency checker with violation reporting
- [ ] Implement system message generator with templates
- [ ] Add export functionality (Markdown, plain text)
- [ ] Character comparison tool
- [ ] Data persistence (save/load projects)

---

## Contributing

Contributions are welcome! This tool is designed to help the LitRPG writing community maintain consistency and quality in their game system design.

---

##  For Authors

This tool helps you focus on storytelling while ensuring your game mechanics remain consistent, believable, and error-free throughout your entire series.

Perfect for:
- First-time LitRPG authors learning game balance
- Series authors tracking complex progression across multiple books
- Anyone who wants professional-quality stat tracking without spreadsheet hell
