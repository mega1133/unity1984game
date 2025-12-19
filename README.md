# Unity 2D Platformer Foundation

Keyboard-only 2D platformer starter scenes for Unity (tested with the built-in Input Manager).

## How to open
1. Open Unity Hub and add this project folder (`unity1984game`).
2. Open the project with Unity 2D template settings.
3. Ensure scenes are in the Build Settings (File ▸ Build Settings):
   - `Assets/Scenes/Main.unity`
   - `Assets/Scenes/Level0_VictoryMansions.unity`
   - `Assets/Scenes/Level1_Diary.unity`
   - (Optional test scenes) `Assets/Scenes/Level0_Test.unity`, `Assets/Scenes/Level0_TestB.unity`

## How to play (Mac keyboard)
- **Move**: `A` / `Left Arrow` (left), `D` / `Right Arrow` (right)
- **Jump**: `Space` / `W` / `Up Arrow` (single jump only)

## Notes
- `Main` scene auto-loads `Level0_VictoryMansions`.
- Player uses a prefab with `Rigidbody2D` + `BoxCollider2D` and single-jump movement.
- Simple placeholder sprites are generated at runtime so no extra art setup is required.
- Checkpoints (yellow blocks) set the respawn location; the first one near spawn is auto-armed.
- Fail zones (red triggers) show a short fail reason and respawn at the last checkpoint.
- Level exits (cyan blocks) fade to black, load the target scene, then fade back in; demo exits link Level0_Test and Level0_TestB. Exits can pull their target from the current `LevelController.nextSceneName` for consistent wiring.
- Level framework: each scene can add a `LevelController` to set initial obligations, update/complete them, hide/show the board, and define the next scene. `LevelSpawnPoint` auto-seeds the respawn point when entering a new scene. Level0_Test includes a demo controller with three starter obligations, a trigger that completes the first line, and an exit that uses the controller’s next scene.
- Obligations board (top-right) shows the current objectives; Level0_Test includes a demo that updates, completes, hides, and re-shows lines over a few seconds.
- Interaction system: move near a demo pickup or toggle (Level0_Test) to see a "PRESS E" prompt; press `E` to trigger the nearest interactable.
- Dialogue demo: walk to the NPC square in Level0_Test and press `E` to start a short four-line chat bubble sequence; advance with `Space` or `Enter`.
- Cutscene demo: in Level0_Test, entering the corridor trigger to the right locks control, walks the player forward, runs a three-line NPC/player exchange, hides then re-shows obligations, waits briefly, and returns control.
- Safe zones + diary: a green patch in Level0_Test allows opening the diary with `Q`; opening elsewhere fails with "THOUGHTCRIME" and respawns.
- Telescreens: two demo telescreens in Level0_Test hum, and one occasionally emits lines; a trigger can force a command line via `PlayLine`.
- Rocket bomb demo: a trigger in Level0_Test plays a whistle, shows a ground shadow, then impacts; standing on the marker when it lands fails with "ROCKET BOMB".
- Suspicion zones: a brown patch in Level0_Test shows a top-center "SUSPICION" bar while inside; lingering until full fails with "LOITERING", leaving resets and hides the bar. Suspicion pauses while dialogue, cutscenes, or the diary are active.
- Level0_VictoryMansions: tutorial hallway with drab colors, two posters, telescreens, a simple gap with a fail pit, mid-level checkpoint, obligations list ("Move to the exit", "Jump the gap"), and an exit wired to Level1_Diary.
- Level1_Diary: exterior street to Mr. Charrington’s shop and a home corner safe zone. Obligations ("Go to the shop", "Buy the diary", "Find a safe zone", "Write in the diary"). Interact at the shop counter to buy the diary, then reach the green safe-zone patch, press `Q` inside it to write safely, and exit right (wired to Level2_HateHall placeholder).
- Safe zone + diary demo: in Level0_Test a green floor patch marks a safe zone. Press `Q` outside it to trigger a THOUGHTCRIME fail, or press `Q` on it to open the diary panel (close with `Space`/`Enter`).
- Telescreen demo: Level0_Test spawns two telescreens (one hum-only, one with idle lines) and a trigger that calls a command line: “6079 SMITH W. NO UNEXPECTED MOVEMENTS!”.
- Scratch/itch demo: stepping into the green ItchTrigger shows "YOU ARE ITCHY. PRESS R TO SCRATCH."; pressing `R` locks movement for about a second and hides the prompt (blocked while diary/dialogue/cutscenes are active).
- World rewrite demo: in Level0_Test a corridor contains a rewrite trigger. Before crossing, a Syme NPC and poster proclaim "WE WILL NEVER FORGET YOU, SYME!". After triggering rewrite, Syme vanishes, the poster text and color change to an official notice, and a new poster appears when backtracking; a short center-screen "CORRECTION ISSUED." notice flashes when the rewrite occurs.
