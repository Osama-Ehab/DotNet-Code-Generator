# ⚙️ Custom C# Code Generator & Dynamic UI Scaffolder

![.NET](https://img.shields.io/badge/.NET-5C2D91?style=for-the-badge&logo=.net&logoColor=white)
![C#](https://img.shields.io/badge/c%23-%23239120.svg?style=for-the-badge&logo=c-sharp&logoColor=white)
![SQL Server](https://img.shields.io/badge/SQL%20Server-CC292B?style=for-the-badge&logo=microsoft-sql-server&logoColor=white)
![WinForms](https://img.shields.io/badge/WinForms-0078D4?style=for-the-badge&logo=windows&logoColor=white)

## 📌 Project Overview
The **Custom Code Generator** is a robust developer tool I am currently architecting to automate the repetitive scaffolding of Data Access Layer (DAL) and Business Logic Layer (BLL) classes. By reading directly from a SQL Server database, this tool enforces a strict **3-Tier Architecture** and completely eliminates boilerplate code.

Beyond standard code generation, this tool is designed to scaffold **Dynamic UI Components** (WinForms) based on database metadata, enforcing the **DRY (Don't Repeat Yourself)** principle across enterprise-level applications like the DVLD (Driving & Vehicle License Department) system.

## 🏗️ Core Architectural Features (The Engine)
This tool goes beyond simple string manipulation; it interacts deeply with the database engine:
* **Metadata Extraction:** Leverages SQL Server `INFORMATION_SCHEMA` to dynamically read tables, columns, data types, and constraints.
* **Extended Properties Integration:** Utilizes SQL Server Extended Properties to map database fields to specific UI controls, and validation rules.
* **Strict 3-Tier Output:** Automatically generates clean, separated `.cs` files for DAL and BLL that are ready to be plugged into any standard .NET project.
* **Adaptive Base Forms (In Progress):** Generates reusable generic forms (`frmGenericManage`, `frmGenericAddEdit`, `ctrlCardSelector`) based on the extracted schema.

## 🚀 Current Status: Active Development (WIP)
*This project is currently under active development. I am building this tool iteratively alongside my academic studies (Data Science).*

**Current Version Capabilities:**
- [x] Successful connection to SQL Server and metadata extraction.
- [x] Generation of fully functional DAL and BLL C# classes.
- [x] Mapping SQL data types to C# data types dynamically.

**Roadmap & Future Enhancements:**
- [ ] Complete the generation engine for WinForms UserControls (`ctrlList`, `ctrlAddEdit`).
- [ ] Finalize the integration of the Generic UI Framework.
- [ ] Deploy the generated code into the DVLD Management System as a real-world proof of concept.

## 🧠 Why I Built This?
While learning 3-Tier Architecture, I realized that writing DAL and BLL classes manually for databases with dozens of tables is inefficient and prone to human error. I decided to build this tool to deeply understand database metadata, metaprogramming, and to drastically speed up the development of my future enterprise projects.

---
*Created by Osama Ehab - Aspiring .NET Developer*
