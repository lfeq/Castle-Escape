# Castle Escape

Castle Escape is a small 2D platformer created with Unity. The project contains several levels where the player must avoid enemies, pick up a key, and reach the exit to escape the castle. A simple menu and credits screen are included.

## Project Structure

- `Assets/` – Unity assets such as scenes, scripts, prefabs, sprites and animations.
- `Packages/` – Unity package manifest listing dependencies like Cinemachine and the Universal Render Pipeline.
- `ProjectSettings/` – Project configuration files. These indicate the project was created with Unity **6000.0.41f1**.

## Scenes

The `Assets/Scenes` directory holds the playable scenes:

- `MainMenu` – starting menu.
- `Level1`, `Level2`, `Level3` – three gameplay levels that progress sequentially.
- `Credits` – end screen after completing Level 3.
- `SampleScene` – sample/testing scene.

## Controls

The default Unity input axes are used:

- **Move** – `A/D` keys or left/right arrow keys.
- **Jump** – `Space` (mapped to `Jump`).
- **Interact** – `F` (used to trigger items or open doors).

## Getting Started

1. Install [Unity](https://unity.com/) version **6000.0.41f1** or newer.
2. Clone this repository.
3. Open the project folder in Unity Hub and launch the `MainMenu` scene.
4. Press `Play` to start the game. Completing each level will load the next one until the credits are shown.

## Building

To create a standalone build, open the **File → Build Settings…** menu in the Unity editor, add the scenes in build order, choose your target platform and select **Build**.

Enjoy exploring the castle!
