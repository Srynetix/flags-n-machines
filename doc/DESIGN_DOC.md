# Flags'n'Machines Design Doc

**Theme**: Multiplayer racing game with capture the flag mechanics  
**Engine / Language**: Godot Engine 3.3.2 / C#  
**Tags**: 3D, Network, Racing game, Shaders, Gravity changes, Physics and AI, Admin commands

### Aesthetics

- Racing game at third-person or first-person view, in a "small world" concept (like Micro Machines).
- Really simple graphics, should run on most computers (and should be mobile and web friendly).

### Gameplay

- CTF game mixed with elimination mode, based on time limit and rounds
- A round starts with everyone on the map, until only one person remains on the map
    - If someone falls, he lost one point for autodestruction (-1)
    - If someone falls because he was pushed by someone else, he doesn't lose points, but the attacker wins 1 point.
    - There is a flag on the map, with a green zone around. It is quite large at the beginning, but when the flag is taken, the zone become smaller and smaller. When the flag is taken, the zone turns red, and players outside the zone are in "danger mode". Danger mode destroy cars outside the flag zone after some delimited time.
    When the flag is small enough (size of the car), the carrier automatically wins, and he wins 5 points.
    - When someone dies, he can't join back in the game until the next round.
- The gravity is altered in the game, with magnet like movements: if a car jumps or drive towards a wall, the car "snaps" at the wall.
- If you run into the flag carrier, you take the flag

### Abilities

- Cars can jump
- Items can make you fire lasers or dash quickly in a direction
- Items spaws at item spawn point (like Quake or Unreal)

### Game start

- Multiplayer is not lobby based but round based.
- If you are alone in the level, no flag will appear, and you will autorespawn if you fall.
- At level initialization (alone in map), you need to wait a little before the round starts (game wait time). If the time expires and you are still alone, it restarts.
- If max players are present, the game autostarts.
- The admin can start the game at any moment.
- You can add bots in a game.
- Spectator mode, where you can follow another car or move freely on the map.

### Technical

- Cvars: game is based on cvars and commands
    - Vars:
        - max_players
        - round_time
        - game_time
        - game_init_wait_time
        - world_gravity
        - cars_drive_speed
        - cars_jump_speed
        - danger_mode_duration
    - Commands
        - restart_game
        - restart_round
        - kill
        - bot_add
        - bot_kick
- Separate game logic from networking, using adapters