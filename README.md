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
