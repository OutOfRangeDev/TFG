<div align="center">
  
# Final Year Project: Custom 2D Game Engine 
*By Guillermo Herrera Gutiérrez (aka. OutOfRange)*  
*Developed during my Game Design Degree @ Universidad Europea de Madrid*

### Research of how Game-Engines work and a Game as tech demo.

</div>

The primary objective of this project was to **understand how modern game engines work under the hood** and replicate their core architecture by building a playable game from scratch, solving complex low-level game development problems along the way.

### Project Brief
**Tech Stack:** *C# (.NET), MonoGame, Unity (used for baseline performance comparison), JetBrains dotTrace / dotMemory, LDtk*

**What you will find in this repository:**
* Game Engine Architecture (ECS)
* Memory Optimization (Data Types, Cache Locality, Zero-Allocation)
* Deterministic Physics (Custom AABB)
* Data-Driven Serialization
* Game Programming Patterns

**What you won't find here:**
* Graphics Programming (HLSL, GLSL)
* Game Design Documents (Extensively researched for the dissertation, but this repository focuses strictly on the engineering. Available upon request)
* Art Documents (Also researched for the dissertation, but it's not the focus of the repo. Available upon request)

<div align="center">

## Architecture

### ECS

</div>

While modern commercial engines typically use a **Hybrid ECS**—blending the flexibility of Object-Oriented Programming (OOP) with the raw performance of Data-Oriented Design (DOD)—I deliberately chose to implement a **Pure ECS architecture** for this project.

My goal was to deeply understand memory management at a fundamental level. With the industry aggressively pushing toward strict data-oriented paradigms (like Unity DOTS and Unreal Mass), I wanted to build this architecture from the ground up. Sticking to a pure ECS meant I had to sacrifice the easy, high-level design solutions of OOP, but it forced me to confront and solve the true cost of architectural overhead. 

The final result is a highly modular, composition-based core. Just like in professional engines, behaviors are completely decoupled from entities! If I add a `PhysicsComponent` to an entity, it instantly falls with gravity. If I remove it, it stops. The same applies to rendering and collisions. There are no rigid inheritance trees—just pure, interchangeable data.

# AÑADIR EL DIAGRAMA UML

### Scope & The Tooling Trade-off

Because my primary objective was to understand engine architecture rather than build a commercial product, I deliberately scoped out developing a visual editor (WYSIWYG). 

The major trade-off of this code-first approach is the friction it creates for game design. There is no inspector window to easily add, remove, or tweak components. Instead, modifying an entity requires manually editing the `EntityFactory` in C# or tweaking the raw `JSON` files. While this is inefficient for rapid game production, it was a necessary constraint. Skipping the UI/Editor development allowed me to focus 100% of my time on what really mattered for this research: memory management, system performance, and low-level physics.

### Data Management

This project strictly adheres to the core principles of the Entity-Component-System (ECS) paradigm. 

In practice, this translates to:
* **Entities** are not objects; they are purely logical identifiers (a simple `int` ID).
* **Components** are plain data containers. They hold the raw values and implicitly grant properties to the entity (e.g., attaching a `PhysicsComponent` means the entity is now affected by gravity).
* **Systems** contain all the logic but hold no state themselves. They simply iterate over specific components and update their values frame-by-frame.

> [!NOTE]
> **A quick note on data layout:** Conceptually we say "Entities have components," but under the hood, Entities do not contain lists of components. Instead, the architecture uses a Structure of Arrays (SoA). Centralized "Component Pools" hold contiguous arrays of component data, indexed by the Entity ID. This distinction is the absolute key to how the engine handles memory and CPU caching, which leads into the next section.

<div align="center">

## Memory Management

### Data-Oriented Design (DOD)

</div>

ECS is the practical application of the Data-Oriented Design (DOD) paradigm, which prioritizes the hardware's internal architecture over human-readable OOP structures. 

Choosing this approach was a deliberate trade-off. Instead of the computer taking on the performance overhead, *I* took on the development overhead (writing more complex boilerplate and systems). My goal was to completely eradicate two massive performance killers in managed languages like C#: **Cache Misses** and the **Garbage Collector (GC)**.

Assuming you are reading this, I probably don't need to explain *why* GC spikes and cache misses ruin frame rates. Instead, here is exactly *how* this engine mitigates them, starting with CPU caching (building on the previous section).

### Defeating Cache Misses (Structure of Arrays)

The reason we use centralized "Component Pools" is entirely because of how CPU Cache Lines work. 

When a System requests a component to update it, the CPU doesn't just fetch that single piece of data from RAM; it grabs that data and the memory slots immediately next to it (a Cache Line). Because this engine stores components of the same type contiguously in an array—rather than scattered randomly across the Heap like standard OOP objects—the processor benefits from hardware prefetching. It already has the next component loaded in the cache before the System even asks for it. This virtually eradicates cache misses and drastically speeds up execution time

# DIBUJO/ANIMACIÓN DE COMO FUNCIONA
