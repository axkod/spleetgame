﻿using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Assets.Script;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using UnityEngine.UI;

class Game : MonoBehaviour
{
	public static Game game;
	
	public static Map map;
	public static List<Player> players;
	public static bool multi;

	private bool pauseGui = false;
	public bool pause = false;
	public bool multiplayer = false;

	public string startMap = "plat1";

	public static int level;
	public static List<string> finishLevels;

	public bool isPopup;

	// Use this for initialization
	void Start()
	{
		Map.dicoMaps = new Dictionary<string, Map>();
		foreach (string s in Map.mapsName)
			Map.dicoMaps.Add(s, Map.OldGetMap(s));
	
		Cursor.lockState = CursorLockMode.Locked;

		Player.nbHints = 2;
		Player.settings = new Settings();

		players = new List<Player>();
		multi = multiplayer;
		
		Scene scene = SceneManager.GetActiveScene();
		
		level = 3;
		finishLevels = new List<string>();

		UseSave();

		switch (scene.name)
		{
			case "Level1":
				map = new Map("Level1", MapType.TEST, new Vector3(0,0,0));

				map.AddElement(new string[] { "button_1" }, "door_1", ExecType.DOOR);
				map.AddElement(new string[] { "button_2", "button_3" }, "window_1", ExecType.WINDOW);
				map.AddElement(new string[] { "button_4" }, "door_2", ExecType.DOOR);
				map.AddElement(new string[] { "button_5" }, "door_3", ExecType.DOOR);

				map.GetCheckpoints().Add(new Checkpoint(new Vector2(8.664959f, 138.28f), new Vector2(29.03885f, 185.8612f), new Vector3(19.224444f, 21.89996f, 132.9029f)));
				break;

			case "DemoScene":
				map = new Map("DemoScene", MapType.TEST, new Vector3(0,0,0));

				map.AddElement(new string[] { "button_1", "button_2", "button_3" }, "door_1", ExecType.DOOR);
				map.AddElement(new string[] { "button_4" }, "window_1", ExecType.WINDOW);
				break;
		}

		if (startMap != "None")
		{
			if (startMap.Length == 4 && startMap.StartsWith("Stp"))
				startMap = "Stp" + level;
			map = Map.GetMap(startMap);
			map.Construct();
		}
		else
			map = new Map("None", MapType.TEST, new Vector3(-13, 5, -8));

		/*foreach (GameObject p in GameObject.FindGameObjectsWithTag("Player"))
			players.Add(new Player(players.Count, p, map));*/

		game = this;

		isPopup = false;

		if (multi)
		{
			if (Player.settings.host)
			{
				NetworkManager.singleton.StartHost();
			}
			else
			{
				NetworkManager.singleton.networkPort = Player.settings.port;
				NetworkManager.singleton.networkAddress = Player.settings.ip.ToString();
				NetworkManager.singleton.StartClient();
			}
		}
		
	}

	// Update is called once per frame
	void Update()
	{
		List<Player> toremove = new List<Player>();
		foreach (Player p in players)
		{
			if (p.body == null)
				toremove.Add(p);
			else
				p.Move();
		}
		players.RemoveAll(p => toremove.Contains(p));

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

		/*if (Input.GetKeyDown(KeyCode.O))
		{
			map.GetButtons()[0].Push();
			map.GetButtons()[0].Exec();
		}*/

		/*if (Input.GetKeyDown(KeyCode.M))
		{
			GameObject.FindWithTag("MapGUI").transform.GetChild(0).gameObject.SetActive(true);
		}*/

		if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.N))
		{
			Player.nbHints++;
			string s = "s";
			if (Player.nbHints < 2)
				s = "";
			Game.game.PopupMessage(Manager.manager.GetTranslation("newhint").Replace("%h", Player.nbHints.ToString()) + s, 1);
		}

		if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.B))
		{
			foreach(Executor button in map.GetButtons())
			{
				if (!button.IsActivate())
				{
					button.Push();
					button.Exec();
				}
			}
		}

		if (Input.GetKeyDown(Player.settings.pause))
		{
			pause = !pause;
			pauseGui = true;
		}
		else
		{
			pause = GameObject.FindWithTag("PauseGUI").transform.GetChild(0).gameObject.activeSelf || GameObject.FindWithTag("MapGUI").transform.GetChild(0).gameObject.activeSelf;
			pauseGui = GameObject.FindWithTag("PauseGUI").transform.GetChild(0).gameObject.activeSelf;
		}

		if (pause)
		{
			//Time.timeScale = 0.0f;
			Cursor.lockState = CursorLockMode.Confined;
		}
		else
		{
			//Time.timeScale = 1.0f;
			Cursor.lockState = CursorLockMode.Locked;
			PlayerPrefs.Save();
		}

		if (pauseGui)
			GameObject.FindWithTag("PauseGUI").transform.GetChild(0).gameObject.SetActive(pause);


		if (multi)
		{
			if (!NetworkClient.active && !NetworkServer.active)
			{
				NetworkManager.Shutdown();
				SceneManager.LoadScene(1);
			}
		}
	}
	
	public void Save()
	{
		/*if (!Directory.Exists("saves"))
		{
			Directory.CreateDirectory("saves");
		}*/
		/*string toSave = "";

		toSave += "MAP:" + map.GetName() + "\n";

		//FIXME multiplayer
		Vector3 check = players[0].GetCurrentCheckpoint();
		toSave += "LASTCHECK:" + check.x + "," + check.y + "," + check.z + "\n";

		toSave += "LEVEL:" + level;

		try
		{
			File.WriteAllText("save.txt", toSave);
		}
		catch(Exception e)
		{
			Debug.LogWarning("Error when trying to save : "+e.GetType());
		}*/

		PlayerPrefs.SetInt("STPlevel", level);
		foreach(string m in Map.mapsName)
		{
			if (finishLevels.Contains(m))
				PlayerPrefs.SetInt("LEVEL" + m, 1);
			else
				PlayerPrefs.SetInt("LEVEL" + m, 0);
		}
		
		PlayerPrefs.Save();
	}
	
	public void UseSave()
	{
		level = PlayerPrefs.GetInt("STPlevel", 3);
		if (level < 3)
		{
			foreach (string m in Map.mapsName)
			{
				int finish = PlayerPrefs.GetInt("LEVEL" + m, 0);
				if (finish == 1)
					finishLevels.Add(m);
			}
		}
	}
	
	public void DestroyMap()
	{
		/*if (multi)
		{
			NetworkManager.Shutdown();
		}*/
		GameObject.FindWithTag("MapGUI").transform.GetChild(0).gameObject.SetActive(false);
		if (!multi)
			players.Clear();
		map.DestroyObjects();
		Map.dicoMaps[map.GetName()] = Map.OldGetMap(map.GetName());
	}
	
	public void LoadMap(string name)
	{
		DestroyMap();
		map = Map.GetMap(name);
		map.Construct();
		if (multi)
		{
			for (int i = 0; i < players.Count; i++)
			{
				players[i].Tp(map.spawnPoint[i]);
				players[i].SetCurrentCheckpoint(map.spawnPoint[i]);
			}
		}
	}
	
	public static int AddPlayer(GameObject obj)
	{
		if (players == null)
			players = new List<Player>();
		int i = players.Count;
		players.Add(new Player(i, obj, map));
		return i;
	}
	
	public IEnumerator LoadMapTime(string name)
	{
		yield return new WaitForSeconds(0.1f);
		LoadMap(name);
	}
	
	public void PopupMessage(string message, int delay)
	{
		isPopup = true;
		GameObject canvas = GameObject.FindWithTag("SampleTextGUI").transform.GetChild(0).gameObject;
		Text text = canvas.transform.GetChild(0).transform.GetChild(0).gameObject.GetComponent<Text>();
		text.text = message;
		canvas.SetActive(true);
		StartCoroutine(DepopMessage(delay));
	}
	
	public IEnumerator DepopMessage(float delay)
	{
		GameObject canvas = GameObject.FindWithTag("SampleTextGUI").transform.GetChild(0).gameObject;

		float step = 1.0f / (delay * 100.0f);
		
		while (delay > 0)
		{
			yield return new WaitForSeconds(0.01f);
			canvas.transform.GetChild(0).GetComponent<CanvasRenderer>().SetAlpha(delay * 100.0f * step);
			delay -= 0.01f;
		}
		canvas.SetActive(false);
		canvas.transform.GetChild(0).GetComponent<CanvasRenderer>().SetAlpha(1.0f);
		isPopup = false;
	}
}
