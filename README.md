# ⚙️ System Factory: Custom C# Code Scaffolding Engine & Modern UI Framework

A metadata-driven metaprogramming tool designed to completely eliminate repetitive boilerplate code, enforcing strict Clean/3-Tier Architecture and dynamic UI rendering.

## 📌 Project Overview

The **System Factory** is a robust, custom-built scaffolding engine and UI framework. Instead of writing standard CRUD operations and forms manually, this engine reads directly from SQL Server database metadata. It dynamically generates a highly optimized backend and auto-layouts responsive frontend forms without human intervention.

The solution is strictly decoupled into two projects:

*   **🏭 The Generator Engine:** The metaprogramming tool that reads schemas and outputs standard C# code using T4 Templates.
*   **🎨 ModernUI.Framework:** A standalone, hardware-accelerated UI library built from scratch using GDI+ to power the generated frontend.

## 🏗️ Core Architectural Features

*   **⚡ High-Performance Data Access:** Generates Data Access Layer (DAL) classes utilizing **Dapper** (Micro-ORM) for zero-overhead, lightning-fast database querying.
*   **🧩 Advanced Templating:** Utilizes **T4 Templates (.tt)** to systematically output clean, separated `.cs` files (DTOs, Repositories, Services) that adhere to SOLID principles.
*   **🖌️ Hardware-Accelerated UI:** Replaces traditional drag-and-drop with `ModernUI.Framework`. Calculates X/Y grid layouts and renders multilingual, responsive forms dynamically.
*   **🧠 Smart Dependency Injection:** Auto-generates the DI container registrations for all repositories and services.

## 🗄️ The UI_ColumnMetadata Hub

Instead of hardcoding UI rules, the engine relies on a dedicated database table (`UI_ColumnMetadata`). This acts as the central configuration hub.

| Column Name | MetaControlType    | MetaIconCode | UIRow | UIColSpan | MetaIsSensitive |
| :---------- | :----------------- | :----------- | :---- | :-------- | :-------------- |
| FirstName   | ModernInputGroup   | `&#xE136;`   | 0     | 1         | False           |
| IsActive    | ModernToggle       | NULL         | 0     | 1         | False           |
| Password    | ModernInputGroup   | `&#xE1F6;`   | 1     | 2         | True            |

> **💡 Result:** Changing a control type from a TextBox to a Toggle Switch, or adjusting its grid layout, is as simple as updating a database record. The engine handles the rest.

## 📂 Repository Structure

The solution is structured to maximize modularity, testability, and separation of concerns:

```text
Solution 'CodeGeneratorSolution'
├── ⚙️ CodeGeneratorSolution (The Engine)
│   ├── 📁 Core                  # GeneratorEngine.cs, MetadataFetcher.cs (SQL connection)
│   ├── 📁 Models                # TableSchema.cs, ColumnDefinition.cs
│   ├── 📁 Templates             # T4 Templates (.tt) grouped by Clean Architecture layers
│   ├── 📁 CSharp_Compiler       # Static classes for IDE syntax checking/testing
│   └── 📁 EmbeddedResources     # Static classes marked for Reflection injection
│
└── 🎨 ModernUI.Framework (Decoupled UI Library)
    ├── 📁 Controls              # Custom GDI+ WinForms controls
    ├── 📁 Icons                 # Font-based & SVG icon rendering logic
    └── 📁 Util                  # UI Theme helpers and Language Managers
```

## 🔧 Engineering Decisions: The Compilation Trick

Generating static base classes (like generic helpers or base repositories) as pure strings is error-prone. To solve this, I implemented a unique meta-pattern:

1.  **Develop in CSharp_Compiler:** I write the base classes here. This allows Visual Studio to provide full Intellisense, syntax checking, and compilation testing during development.
2.  **Move to EmbeddedResources:** Once the class is tested and bug-free, a copy is placed in this folder and its Build Action is set to *Embedded Resource*.
3.  **Inject via Reflection:** At generation time, the engine extracts these embedded files directly from the assembly via Reflection and injects them into the target project.

*This guarantees zero syntax errors in the generated base classes.*

## 🎯 Why I Built This?

In enterprise software, manual boilerplate is the enemy of scalability. I built this engine to shift my focus from writing code to architecting systems. It allows me to iterate faster, maintain a single source of truth (the database), and ensure architectural consistency across massive projects.
