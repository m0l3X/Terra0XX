
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Terraria.Localization;
using Terraria.DataStructures;
using Terraria.Chat;
using Microsoft.Xna.Framework;
using System.Windows.Interop;
using Terraria.ID;
using Terraria.GameInput;
using Microsoft.Xna.Framework.Input;
using Terraria.UI.Chat;
using System.Windows.Input;
using Terraria.GameContent.Animations;
namespace Terraria.Terra0XX;

class ChatCommandListenerAndExecutor : Main
{
	private static bool MouseTp;
	private static List<string> audit = new List<string>();
	private static int cursor;
	public static void ListenForUpdate()
	{
		
		if (MouseTp) {
			MouseState ms = Mouse.GetState();
			if (ms.RightButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed) {
				player[myPlayer].Teleport(MouseWorld);
				//IEntitySource projectileSource_Item_WithPotentialAmmo = player[myPlayer].GetProjectileSource_Item_WithPotentialAmmo(player[myPlayer].HeldItem, 0);
				//Projectile.NewProjectile(projectileSource_Item_WithPotentialAmmo, MouseWorld, new Vector2(0,0), 857, 100, 1, myPlayer);
				//TODO: cool effects 
			}
			//base.Update(gameTime);
		}
		try {
			if (drawingPlayerChat) {

				bool pressedUp = inputText.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Up) && !oldInputText.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Up);
				bool pressedDown = inputText.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Down) && !oldInputText.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Down);

				if (pressedUp && audit.Count > 0) {
					cursor++;
					if (cursor >= audit.Count)
						cursor = audit.Count - 1; 

					chatText = audit[audit.Count - 1 - cursor];
				}


				if (pressedDown && audit.Count > 0) {
					cursor--;
					if (cursor < -1)
						cursor = -1;

					if (cursor == -1)
						chatText = ""; 
					else
						chatText = audit[audit.Count - 1 - cursor];

				}

				oldInputText = inputText;
			}
		}
		catch(Exception e) {
			LocalMessage(e.Message, Color.Red);
		}
	}

	public static bool CheckCommand(string msg)
	{
		audit.Add(msg);
		cursor = -1;
		if (msg[0] == '/') {
			ExecuteCommand(msg);
			return true;
		}
		else
			return false;
	}
	public static void LocalMessage(string msg, Color col)
	{
		if(msg.Contains('\n')) {
			string[] lines = msg.Split('\n');
			foreach(string l in lines) {
				ChatHelper.DisplayMessageOnClient(NetworkText.FromLiteral(l), col, myPlayer);
			}
		}
		else
			ChatHelper.DisplayMessageOnClient(NetworkText.FromLiteral(msg), col, myPlayer);
	}
	public static void ExecuteCommand(string command) {
		try {
			string com = command;
			string val = "true";
			int count = 1;
			if (command.Contains(' ')) {
				string[] args = command.Split(' ');
				com = args[0];
				val = args[1];
				if (args.Length == 3) count = int.Parse(args[2]);
				if (args.Length > 3) throw (new Exception(message: "fuck you"));
			}
			string msg = "";
			Color col = Color.Red;
			//MessageBox.Show(player[myPlayer].HeldItem.useAnimation.ToString());
			switch (com) {
				default: {
					msg = "Unknown command! use /h to list all the commands";
					break;
				}
				case "/ut" or "/usetime": {
					msg = "Changed UseTime to " + val + ". Prev: " + player[myPlayer].HeldItem.useTime.ToString();
					col = Color.Gold;
					player[myPlayer].HeldItem.useTime = int.Parse(val);
					break;
				}
				case "/ar" or "/autoreuse": {
					msg = "Changed AutoReuse to " + val + ". Prev: " + player[myPlayer].HeldItem.autoReuse.ToString();
					col = Color.Gold;
					player[myPlayer].HeldItem.autoReuse = bool.Parse(val);
					break;
				}
				case "/ua" or "/useanimation": {
					msg = "Changed UseAnimation to " + val + ". Prev: " + player[myPlayer].HeldItem.useTime.ToString();
					col = Color.Gold;
					player[myPlayer].HeldItem.useAnimation = int.Parse(val);

					break;
				}
				case "/dmg" or "/damage": {
					msg = "Changed item damage to " + val + ". Prev: " + player[myPlayer].HeldItem.damage.ToString();
					col = Color.Gold;
					player[myPlayer].HeldItem.damage = int.Parse(val);
					
					break;
				}
				case "/i" or "/item": {
					player[myPlayer].QuickSpawnItem(new EntitySource_ItemUse(player[myPlayer], player[myPlayer].HeldItem), int.Parse(val), count);
					msg = "Spawned item " + val;
					col = Color.Gold;
					break;
				}
				case "/gm" or "/godmode": {
					/*player[myPlayer].statDefense = Int32.MaxValue;
					player[myPlayer].statLifeMax2 = Int32.MaxValue;
					player[myPlayer].statLife = Int32.MaxValue;
					player[myPlayer].statManaMax = Int32.MaxValue;
					player[myPlayer].statManaMax2 = Int32.MaxValue;
					player[myPlayer].manaRegen = Int32.MaxValue;
					player[myPlayer].luckMaximumCap = Int32.MaxValue;
					player[myPlayer].luck = Int32.MaxValue;*/
					player[myPlayer].CustomGodmode = bool.Parse(val);
					//player[myPlayer].meleeNPCHitCooldown = new int[0];
					msg = "Set godmode to " + val;
					col = Color.Gold;
					break;
				}
				case "/mtp" or "/mousetp": {
					MouseTp = bool.Parse(val);
					msg = "Set MouseTp to " + val;
					col = Color.Gold;
					break;
				}
				case "/tp": {
					List<Player> list = new List<Player>();
					foreach (Player pl in player) {
						if (pl.name.Contains(val)) {
							list.Add(pl);
						}
					}
					if (list.Count == 1) {
						player[myPlayer].Teleport(list[0].position);
					}
					else {
						LocalMessage("There are multimple players with given string, please specify:", Color.LightBlue);
						foreach (Player n in list) {
							LocalMessage(n.name, Color.Gray);
						}
					}
					break;
				}
				case "/h" or "/help": {
					msg = "List of avaiable commands: \n" +
						"Form: 'Command, (alias), {input type(s)}, description' \n" +
						"/ut (/usetime) {integer} - Change UseTime of a handheld item \n" +
						"/ua (/useanimation) {integer} - Change animation time of a handheld item \n" +
						"/ar (/autoreuse) {true/false} - Enable/Disable autoreuse of a handheld item \n" +
						"/i (/item) {integer} {integer} - Give player an item with specified ID (first argument) and count (second argument) \n" +
						"/gm (/godmode) {true/false} - Enable/Disable godmode \n" +
						"/mtp (/mousetp) {true/false} - Enable/Disable teleport to mouse pos on right mouse button \n" +
						"/tp (/tp) {string} - Teleport player to the another one that contains given string in his name \n";
					col = Color.LightBlue;
					break;
				}
			}
			LocalMessage(msg, col);

		}
		catch(Exception e) {
			LocalMessage(e.Message, Color.Red);
		}
		
		
	}
}


/*
			
			Main.player[0].HeldItem.shoot = 933;
			Main.player[0].meleeSpeed = 100;
			Projectile proj = new Projectile();
			proj.entityId = 12;
			proj.active = true;*/