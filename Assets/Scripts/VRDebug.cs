using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VRDebug : MonoBehaviour
{

	List<string> debugList = new List<string>();

	public int maxHistory, maxCharacters;

	Text debug;

	static VRDebug instance;

    // Use this for initialization
    void Awake()
    {
        debugList.AddRange(
			new string[maxHistory]
		);
        debug = GetComponentInChildren<Text>();
    }

    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }
    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

	public void HandleLog(string logString, string stackTrace, LogType type)
	{
        debugList.Add(
			logString.Length > maxCharacters ?
			logString.Substring(0, maxCharacters) :
			logString
			);
		debugList.RemoveAt(0);
		UpdateList();
	}

	void UpdateList()
	{
        debug.text = string.Join("\n", debugList.ToArray());
    }

}
