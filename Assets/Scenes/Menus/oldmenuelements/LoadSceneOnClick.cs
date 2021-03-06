﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class LoadSceneOnClick : MonoBehaviour {

	public void LoadByIndex(int sceneIndex)
	{
		PlayerPrefs.Save();
		SceneManager.LoadScene(sceneIndex);
		Time.timeScale = 1.0f;
	}
}
