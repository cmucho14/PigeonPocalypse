# PigeonPocalypse
CS 583 Final Project 3D Game

A 3D wave-based survival action game where players fight through progressively harder waves of pigeon enemies in a park arena.


Team Members


Brody Armstrong | Player Design & Movement 
Carson Mucho | Enemy AI & Combat Systems 
Emily Stratoudis | UI/UX Design 
Lachlan Carlsen | Art & Level Design 


Game Description

Overview
PigeonPocalypse is a fast-paced 3D survival game where players battle waves of increasingly aggressive pigeons in a park setting. The core gameplay loop revolves around:
Wave-based survival — Fight through progressively harder enemy waves
Character progression — Earn XP, level up, and unlock upgrades
Combat — Use weapons and abilities to defeat enemies

Setting
The game takes place in an urban park arena surrounded by buildings. Players must survive against swarms of pigeon enemies that grow stronger with each wave.

Enemy System
- Pigeon enemies spawn in waves and aggressively chase the player
- Difficulty scales over time: increased health, speed, damage, and spawn rate
- Enemies use NavMesh pathfinding to intelligently pursue the player
- Boss pigeons spawn at milestone waves (planned)

Combat & Progression
- Players start with basic attacks and gain better equipment over time
- XP earned from defeating enemies and surviving waves
- Level ups grant skill points for upgrading attributes
- Loot system with chests and trader NPCs (planned)

HUD & UI
- Health bar display
- XP and level indicator
- Inventory system for stats and equipment (planned)

Implemented Features
- [x] Main menu with working Play/Quit buttons
- [x] End screen with Retry/Main Menu buttons
- [x] Playable character (CowboyRIO) with WASD movement and jumping
- [x] Player rotates to face movement direction
- [x] Camera follows player
- [x] Health system with damage and death
- [x] Enemy AI that chases player using NavMesh
- [x] Enemy attacks player when in range
- [x] Park environment with low-poly assets
- [x] Scene flow: Start Menu → Game → End Screen

Assets Integrated
- CowboyRIO player character
- Pigeon enemy model
- Low Poly Environment Park (trees, benches, fountains)
- Rocks and Terrains Pack
- Custom fonts and UI elements

Member Goals

Brody Armstrong — Player Design
- [ ] Player attack/combat system
- [ ] Weapon equipping and switching
- [ ] Player animations
- [ ] Dodge/roll mechanic

Carson Mucho — Enemy AI & Combat
- [ ] Wave spawning system
- [ ] Difficulty scaling per wave
- [ ] Boss enemy variants
- [ ] Enemy death effects/drops

Emily Stratoudis — UI/UX Design
- [ ] In-game HUD (health bar, XP display)
- [ ] Inventory/equipment UI
- [ ] Wave counter display
- [ ] Pause menu
- [ ] Settings menu

Lachlan Carlsen — Art & Level Design
- [ ] Additional biomes/arenas
- [ ] Visual effects (particles, lighting)
- [ ] Enemy visual variants
- [ ] Environmental hazards/obstacles

