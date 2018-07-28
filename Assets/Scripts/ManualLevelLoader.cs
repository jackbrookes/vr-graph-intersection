using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManualLevelLoader : MonoBehaviour
{
    public Level levelToLoad;
	public bool loadOnStart;

    [InspectorButton("LoadLevel")]
	public bool _load;

	public LevelManager levelManager;	

	void Start()
	{
		if (loadOnStart && levelToLoad) LoadLevel();
	}

	void LoadLevel()
	{
		levelManager.DestroyLevel();
        levelManager.LoadLevel(levelToLoad);
	}

}
