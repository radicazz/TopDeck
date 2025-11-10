# Part 3 Core Requirements — Code & Features

This document lists only the core code changes and feature additions for Part 3.

- Upgradable defenders and tower [Weight: 20%]
  - Implement an upgrade system with at least two upgrade levels per defender and for the tower.
  - Upgrades must increase health; may also modify combat stats (e.g., attack speed, damage, defence, range).
  - Applying an upgrade must change the unit/tower’s visual appearance (e.g., material/mesh/VFX changes).
  - Keep the game balanced and challenging; update procedural wave scaling to account for upgrades.

- Custom shaders [Weight: 10%]
  - Implement at least two custom shaders.
  - One shader performs vertex displacement on its mesh.
  - One shader modifies the base colour/albedo of the material.
  - Integrate shaders into in-game assets so they enhance readability/feedback.

- Custom visual effects (VFX) [Weight: 10%]
  - Implement at least one custom particle system effect (e.g., hit impact, upgrade flash, projectile, tower aura).
  - Implement at least one custom screen-space post-processing effect.
  - Effects must be custom-made and should support gameplay feedback.

- Custom procedural generation feature (implementation) [Weight: 20%]
  - Add one new procedurally generated system that runs at playtime and integrates with the core loop.
  - Example options: procedural soundtrack; per-run procedural narrative; procedural enemy variant generator (appearance + stats); procedural loot/modifiers.
  - The feature must be integrated (not standalone) and contribute positively to the experience.

- Suitable complexity [Weight: 10%]
  - Reflects appropriate difficulty and proficiency for the module.
  - Demonstrates innovative thinking, effective problem-solving, and originality/creativity in code.
  - Applies to the overall implementation across features.

- Balancing and integration [Assessed within above; no separate marks]
  - Update enemy wave spawner/difficulty scaling to consider upgrades and the new procedural feature.
  - Ensure new assets are wired via prefabs/scenes/scripts and participate in the core gameplay loop.
