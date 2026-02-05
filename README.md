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

## Technology Stack

- **Framework:** Avalonia UI 11.3 (cross-platform .NET UI framework)
- **Architecture:** MVVM with CommunityToolkit.Mvvm
- **Language:** C# / .NET 10.0
- **Formula Engine:** NCalc for mathematical expression evaluation
- **Graphics:** SkiaSharp for chart rendering
- **DI:** Microsoft.Extensions.DependencyInjection

---

## Getting Started

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

## Current Features

### World Rules Editor

The heart of the system - fully implemented with a polished dark-themed UI.

#### Stats Definition

Create and manage base stats (STR, AGI, INT, VIT) with:
- Base value and growth per level
- Min/max value constraints
- Full CRUD operations (add, edit, delete, duplicate)
- Drag-and-drop reordering

**Derived Stats** with formula support:
- `HP = VIT * 12 + Level * 5`
- `Mana = INT * 10 + Level * 3`
- `PhysDmg = STR * 2 + Level`

**Formula Validation:**
- Real-time syntax validation using NCalc
- Unknown variable detection
- Live preview of calculated values at any level
- Visual valid/invalid status indicators

**Default Stats Included:**
- STR, VIT, INT, AGI (base stats)
- HP, Mana, PhysDmg (derived stats)

#### XP & Level Curves

Multiple curve types supported:
- **Linear:** `BaseXP + (Level - 1) × Multiplier`
- **Exponential:** `BaseXP × Base^(Level - 1)`
- **Custom Formula:** Write your own with NCalc syntax

**Features:**
- Interactive level preview slider (1-100)
- Visual XP curve chart rendered with SkiaSharp
- Dual-line display: XP Required per level & Total XP
- Real-time chart updates when parameters change
- Level preview table showing XP requirements

#### Level-Up Rules

Configure stat bonuses and point allocations on level up.

---

### Skills & Abilities System

Fully implemented skill definition and skill tree editor.

#### Skill Definitions

Create and manage active and passive skills with:
- **Active Skills:** Damage formulas, mana cost, cooldowns
- **Passive Skills:** Effect formulas for stat bonuses
- Skill ranks (1 to max rank)
- Required level and class lock restrictions
- Prerequisite skill requirements

**Formula Support:**
- Real-time validation with NCalc
- Preview values at any level/rank combination
- Available variables: `Level`, `SkillRank`, `STR`, `VIT`, `INT`, `AGI`
- Functions: `Min`, `Max`, `Abs`, `Pow`, `Sqrt`, `Floor`, `Ceiling`, `if(cond, true, false)`

**Example Active Skill:**
```
Fireball:
  Damage = (INT × 2.5) + SkillRank × 10
  Mana Cost = 30 + SkillRank × 5
  Cooldown = Max(1, 10 - SkillRank)
```

**Example Passive Skill:**
```
Iron Skin:
  Effect = VIT × SkillRank + Level × 2
```

#### Status Effects

Attach status effects to skills:
- Effect name and formula
- Duration in seconds
- Buff/Debuff classification

#### Skill Trees

Organize skills into visual tree structures:
- **Tree Properties:** Name, description, associated class
- **Tiered Nodes:** Organize skills by tier level (0 = root)
- **Child Node Links:** Define prerequisites (unlocking a skill unlocks access to children)
- **Evolution Paths:** Branching skill evolutions (e.g., Fireball → Inferno Blast OR Fire Storm)

**Node Relationships:**
- Link child nodes that become available after unlocking a parent skill
- Link evolution nodes for branching upgrade paths
- Visual indicators showing relationship counts per node

**Example Skill Tree:**
```
Combat Skills (Warrior Class)
├── Tier 0: Power Strike
│   ├── Child → Tier 1: Cleave
│   └── Child → Tier 1: Shield Bash
├── Tier 1: Cleave
│   ├── Evolution → Whirlwind
│   └── Evolution → Earthquake Slam
└── Tier 1: Shield Bash
    └── Child → Tier 2: Shield Wall
```

#### Formula Reference

Built-in reference tab showing:
- All available variables with descriptions
- Supported mathematical functions
- Example formulas for common use cases

---

## Project Structure

```
src/
├── ProgressionManager.sln
└── ProgressionManager/
    ├── Controls/           # Custom UI controls
    │   ├── XpCurveChart.cs      # SkiaSharp-based chart
    │   └── Charting/            # Chart rendering components
    ├── Converters/         # XAML value converters
    ├── Data/               # Default data (JSON)
    ├── Extensions/         # Service collection extensions
    ├── Models/             # Data models
    │   └── WorldRules/          # Stats, XP curves, formulas
    ├── Services/           # Business logic
    │   ├── FormulaValidatorService.cs
    │   ├── StatService.cs
    │   ├── XpCurveCalculatorServiceService.cs
    │   └── Interfaces/
    ├── ViewModels/         # MVVM view models
    └── Views/              # UI views (AXAML)
```

---

## Planned Modules


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

## Roadmap

- [x] World Rules Editor with formula validation
- [x] XP curve designer with visual graphs
- [x] Stats management (base & derived)
- [x] Navigation system
- [x] Dark-themed modern UI
- [x] Skill definitions (active/passive with formulas)
- [x] Status effects system
- [x] Skill tree editor with node relationships
- [x] Child node prerequisites (unlock paths)
- [x] Evolution paths (branching skill upgrades)
- [ ] Create character progression timeline
- [ ] Add consistency checker with violation reporting
- [ ] Implement system message generator with templates
- [ ] Add export functionality (Markdown, plain text)
- [ ] Character comparison tool
- [ ] Data persistence (save/load projects)
- [ ] Class and race templates

---

## Contributing

Contributions are welcome! This tool is designed to help the LitRPG writing community maintain consistency and quality in their game system design.

---

## For Authors

This tool helps you focus on storytelling while ensuring your game mechanics remain consistent, believable, and error-free throughout your entire series.

Perfect for:
- First-time LitRPG authors learning game balance
- Series authors tracking complex progression across multiple books
- Anyone who wants professional-quality stat tracking without spreadsheet hell
