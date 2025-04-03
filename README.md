# Stock and Shop – Unity-Based Retail Management Simulation

**Stock and Shop** is a modular, simulation-style retail management game developed in Unity as part of my final-year dissertation project. It was awarded a **First Class (73%)** and was the department’s **official nominee for the 2024 TIGA Awards**.

The game places the player in the role of a shop manager tasked with stocking shelves, managing wholesale inventory, serving customers, operating tills, and tracking profits over time. The project was built to demonstrate modular gameplay systems, UI responsiveness, and player-facing mechanics with real commercial simulation roots.

---

## 🧠 Core Features

### 🛍️ Inventory & Shelving System
- Buy items from a wholesale catalogue with fluctuating pricing
- Place items manually on available shelves
- Track inventory with item-specific properties (name, quantity, price)

### 👥 Customer Spawning & Behaviour
- Randomised customer types with purchasing logic
- Customers make purchases based on availability and preferences
- Purchases tracked per day with item-level breakdown

### 💸 Till & Transaction System
- Automated till management when customers are served
- UI updates display live transactions and player balance
- Receipt generation for transactions

### 📈 Daily & Overall Summary
- End-of-day and week reports with:
  - Revenue
  - Item performance
  - Stock depletion
- Supports long-term tracking for gameplay feedback loop

### 🧩 Modular Systems Architecture
- Each system (Inventory, Shelving, Tills, Wholesale, Customer AI) implemented in modular scripts
- Allows easy extension or adaptation (e.g. new shop layouts or item types)

### 🖥️ Dynamic & Responsive UI
- Resizable components for 1-column and 2-column views
- Information bars and dynamic summaries for player feedback
- Designed with scalability and readability in mind

---

## 🔧 Tech Stack

- **Engine:** Unity (2022.x)
- **Language:** C#
- **Docs:** Auto-generated with **Doxygen**
- **Architecture:** Modular MonoBehaviour-based system
- **Version Control:** Git/GitHub
- **IDE:** Visual Studio Community

---

## 📁 Key Scripts Overview

| Category          | Key Scripts |
|-------------------|-------------|
| Inventory Logic   | `InventoryManager`, `InventoryItem`, `InventoryItemUI` |
| Shelf System      | `ShelfManager`, `ShelfItemUI`, `BuyItemHandler` |
| Customer System   | `CustomerSpawner`, `Customer`, `ShopFloorManager` |
| Till System       | `TillManager`, `CashDisplay`, `RecieptGenerator` |
| Wholesale Logic   | `WholesaleManager`, `WholesaleItemUI` |
| Day Cycle & Stats | `DayCycle`, `DailySummaryManager`, `OverallSummaryManager` |
| UI Components     | `NavigationController`, `InformationBar`, `SummaryPrefabScript` |
| Layout Helpers    | `StartAtTop`, `SyncPositionWithButton`, `DynamicContentSizeForOneColumn/TwoColumns` |

---

## 🧪 Development Goals & Outcomes

This project was designed to:
- Explore **economy-driven gameplay systems**
- Demonstrate **player feedback loops** using real-time and end-of-day stats
- Practice **modular system architecture** across interacting gameplay elements
- Build a professional documentation pipeline using **Doxygen**
- Reflect on gameplay engagement through **UI design and clarity**

---

## 🎓 Academic Recognition

- **Grade:** First-Class Honours (73%)
- **TIGA Nomination:** Official 2024 nominee from the University of Suffolk Games Department
- **Supervision:** Mentored by Nick Thomas, nominated after unanimous agreement from the games faculty

---

## 📄 Documentation

Full developer documentation is available under `Doxygen Documentation/`, including:
- Class references
- Inheritance diagrams
- Function summaries
- Structural breakdown

---

## 🕹️ Future Extensions (Planned or Suggested)

- Player progression / difficulty scaling
- UI refactoring with Unity UI Toolkit
- Save/load system
- Multiplayer co-op shop management
- Customer mood/needs system

---

## 🙌 Final Thoughts

This project represents the culmination of my time in the Games Development (Programming) BSc, showcasing my abilities in:
- Gameplay system design
- UI/UX implementation
- Technical planning and documentation
- Game loop construction
- Long-term scalability and clarity in codebase structure

If you’re looking for a programmer who can think modularly, work independently, and create polished, real-world gameplay systems — this is a prime example of what I can do.

