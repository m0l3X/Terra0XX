# Terra0XX
Terra0XX is a clientside terraria modification, with an aspect of improving game experience.

*inspired by TerraHax*

# Current list of features:
Modification adds useful stuff to the base game's clientside
It features:
- Allowed use of terraria's deprecated items (such as First Fractal, etc.)
- Faster menu loading
- Cheat commands
- Chat history

And many more coming up!


# Cheat commands
Every command in this mod starts with a '/' symbol. 
List of added cheat commands with their description:
- /ut (alias: /usetime) {integer} - Change UseTime of a handheld item
- /ua (alias: /useanimation) {integer} - Change animation time of a handheld item
- /ar (alias: /autoreuse) {true/false} - Enable/Disable autoreuse of a handheld item
- /i (alias: /item) {integer} {integer} - Give player an item with specified ID (first argument) and count (second argument)
- /gm (alias: /godmode) {true/false} - Enable/Disable godmode
- /mtp (alias: /mousetp) {true/false} - Enable/Disable teleport to mouse pos on right mouse button
- /tp (alias: /tp) {string} - Teleport player to the another one that contains given string in his name
- /h (alias: /help) - Show list of commands

# Installation
Simply put Terraria0XX.exe into terraria's installation folder and run it

Add `-nosteam` launch argument to launch game without steam API.

# Where's source code?
I can't publish full source code of a project due to ReLogic's legal terms, but you can rebuild this mod with these steps:
- Decompile terraria with official tmodloader guide: https://github.com/tModLoader/tModLoader/wiki/tModLoader-guide-for-contributors
- Put files from this repository into /src/Terraria/Terraria/
- Open .csproj with visual studio and build the mod
Have fun and a nice day!
