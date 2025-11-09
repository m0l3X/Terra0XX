
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
using Terraria.Net;
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
					col = Color.LimeGreen;
					player[myPlayer].HeldItem.useTime = int.Parse(val);
					//NetMessage.SendData(21, -1, -1, null, player[myPlayer].HeldItem.netID);
					break;
				}
				case "/ar" or "/autoreuse": {
					msg = "Changed AutoReuse to " + val + ". Prev: " + player[myPlayer].HeldItem.autoReuse.ToString();
					col = Color.LimeGreen;
					player[myPlayer].HeldItem.autoReuse = bool.Parse(val);
					//NetMessage.SendData(21, -1, -1, null, player[myPlayer].HeldItem.netID);
					break;
				}
				case "/ua" or "/useanimation": {
					msg = "Changed UseAnimation to " + val + ". Prev: " + player[myPlayer].HeldItem.useAnimation.ToString();
					col = Color.LimeGreen;
					player[myPlayer].HeldItem.useAnimation = int.Parse(val);
					//NetMessage.SendData(21, -1, -1, null, player[myPlayer].HeldItem.netID);
					break;
				}
				case "/dmg" or "/damage": {
					msg = "Changed item damage to " + val + ". Prev: " + player[myPlayer].HeldItem.damage.ToString();
					col = Color.LimeGreen;
					player[myPlayer].HeldItem.damage = int.Parse(val);
					//NetMessage.SendData(21, -1, -1, null, player[myPlayer].HeldItem);

					//player[myPlayer].HeldItem.useAmmo
					break;
				}
				case "/i" or "/item": {
					player[myPlayer].QuickSpawnItem(player[myPlayer].GetItemSource_Misc(player[myPlayer].HeldItem.type), int.Parse(val), count);
					msg = "Spawned item " + val;
					col = Color.LimeGreen;
					break;
				}
				case "/tb" or "/tileboost": {
					player[myPlayer].HeldItem.tileBoost = int.Parse(val);
					
					msg = "Set tile boost to " + val;
					col = Color.LimeGreen;
					break;
				}
				case "/makenpc" or "/mn": {
					player[myPlayer].HeldItem.DefaultToCapturedCritter(short.Parse(val));
					msg = "Making NPC: " + val;
					col = Color.LimeGreen;
					break;
				}
				case "/summon": {
					//player[myPlayer].QuickSpawnItem(player[myPlayer].GetItemSource_Misc(player[myPlayer].HeldItem.type), int.Parse(val), count);
					for(int i = 0; i < count; i++) {
						if(netMode == 0)
							NPC.NewNPC(player[myPlayer].GetNPCSource_TileInteraction((int)MouseWorld.X, (int)MouseWorld.Y), (int)MouseWorld.X, (int)MouseWorld.Y, int.Parse(val));

						else
							NetMessage.SendData(61, -1, -1, null, myPlayer, int.Parse(val));

					}
					msg = "Summoned " + NPC.GetFullnameByID(int.Parse(val));
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
				case "/id" or "/lookupid": {
					Item.LookupID = bool.Parse(val);
					msg = "Set ID lookup to " + val;
					col = Color.Gold;
					break;
				}
				case "/prid" or "/shoot": {
					msg = "Changed projectile id to " + val + ". Prev: " + player[myPlayer].HeldItem.shoot.ToString();
					col = Color.LimeGreen;
					player[myPlayer].HeldItem.shoot = int.Parse(val);
					break;
				}
				case "/ps" or "/ss": {
					msg = "Changed projectile speed to " + val + ". Prev: " + player[myPlayer].HeldItem.shootSpeed.ToString();
					col = Color.LimeGreen;
					player[myPlayer].HeldItem.shootSpeed = int.Parse(val);
					break;
				}
				case "/h" or "/help": {
					msg = "List of avaiable commands: \n" +
						"Form: 'Command, (alias), {argument(s)}, description' \n" +
						"/ut (/usetime) {frames i guess?} - Change UseTime of a handheld item \n" +
						"/ua (/useanimation) {also frames} - Change animation time of a handheld item \n" +
						"/ar (/autoreuse) {true/false} - Enable/Disable autoreuse of a handheld item \n" +
						"/i (/item) {ID} {count} - Give player an item with specified ID (first argument) and count (second argument) \n" +
						"/gm (/godmode) {true/false} - Enable/Disable godmode \n" +
						"/mtp (/mousetp) {true/false} - Enable/Disable teleport to mouse pos on right mouse button \n" +
						"/tp (/tp) {username} - Teleport player to the another one that contains given string in his name \n" +
						"/dmg (/damage) {integer} - Change item's base damage \n" +
						"/id (/lookupid) {true/false} - Adds item's id to its tooltip \n" +
						"/tb (/tileboost) {integer} - Change tile boost of a handheld item \n" +
						"/summon {id} {count} - (singleplayer) Spawn NPC at cursor with given id\n (multiplayer) Spawns boss/event that can be spawned with a summon item, lookup NPC id's on google \n" +
						"/prid (/shoot) - Change shoot projectile of a handheld item \n" +
						"/ps (/ss) - Change projectile speed of a handheld item \n" +
						"/h (/help) - Show this list";

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