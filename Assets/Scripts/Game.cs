﻿using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Script;
using UnityEngine;
using UnityEngine.SceneManagement;

class Game : MonoBehaviour
{
	
	public static Map map;
	public static Dictionary<string, Player> players;

	// Use this for initialization
	void Start()
	{
		//GameObject go = Instantiate(Resources.Load("Tcorridor"), new Vector3(0, 0, 0), Quaternion.Euler(0.0f, 0.0f, 0.0f)) as GameObject;

		players = new Dictionary<string, Player>();

		Scene scene = SceneManager.GetActiveScene();

		switch (scene.name)
		{
			case "Level1":
				map = new Map("Level1", MapType.TEST);

				map.AddElement(new string[] { "button_1" }, "door_1", ExecType.DOOR);
				map.AddElement(new string[] { "button_2", "button_3" }, "window_1", ExecType.WINDOW);
				map.AddElement(new string[] { "button_4" }, "door_2", ExecType.DOOR);
				map.AddElement(new string[] { "button_5" }, "door_3", ExecType.DOOR);

				map.GetCheckpoints().Add(new Checkpoint(new Vector2(8.664959f, 138.28f), new Vector2(29.03885f, 185.8612f), new Vector3(19.224444f, 21.89996f, 132.9029f)));
				break;

			case "DemoScene":
				map = new Map("DemoScene", MapType.TEST);

				map.AddElement(new string[] { "button_1", "button_2", "button_3" }, "door_1", ExecType.DOOR);
				map.AddElement(new string[] { "button_4" }, "window_1", ExecType.WINDOW);
				break;

			case "initscene":
				map = new Map("initscene", MapType.TEST);

				map.AddElement(new string[] { "button_1" }, "door_1", ExecType.DOOR);
				break;
		}

		//FIXME dont work with multiplayer
		//players.Add("Player1", new Player("Player1", GameObject.FindWithTag("Player"), map));
	}

	// Update is called once per frame
	void Update()
	{
		foreach (Player p in players.Values) 
			p.Move();

		/*
		//some key for s1
		if (Input.GetKeyDown(KeyCode.T))
		{
			SceneManager.LoadScene(2);
		}
		else if (Input.GetKeyDown(KeyCode.K))
		{
			SceneManager.LoadScene(1);
		}
		else if (Input.GetKeyDown(KeyCode.H))
		{
			players["Player1"].Tp(new Vector3(19.224444f, 21.89996f, 132.9029f));
		}*/

		if (Input.GetKeyDown(KeyCode.O))
		{
			map.GetButtons()[0].Push();
			map.GetButtons()[0].Exec();
		}
	}
}
