using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System;

[Serializable]
public class PlayerShipData
{
    public int ID;
    public int Lv;

    public string Race;
    public string Type;
    public string Plus;
    public string Name;
    public string Stat;

    public PlayerShipData(int id, int lv, string race, string type, string plus, string name, string stat)
    {
        ID = id;
        Lv = lv;
        Race = race;
        Type = type;
        Plus = plus;
        Name = name;
        Stat = stat;
    }
}

public class NextChangeScene : Singleton<NextChangeScene>
{
    [HideInInspector] public List<PlayerShipData> shipLists = new List<PlayerShipData>();
    [HideInInspector] public string folderPath, shipFilePath;
    [HideInInspector] public TweenAlpha loadingBlind;
    bool isLoaded = false;

    void Awake()
	{
        Screen.SetResolution(1920, 1080, true);
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Application.targetFrameRate = 60;

        DataCreate();
        //DontDestroyOnLoad(this);

        // test setting
        isLoaded = false;
        loadingBlind = GameObject.FindGameObjectWithTag("Blind").GetComponent<TweenAlpha>();
        loadingBlind.GetComponent<UISprite>().color = new Color32(255, 255, 255, 255);
        StartAction();
    }

    //void OnEnable()
    //{
    //    SceneManager.sceneLoaded += OnSceneLoaded;
    //}
 
    //void OnDisable()
    //{
    //    SceneManager.sceneLoaded -= OnSceneLoaded;
    //}

    //void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    //{
    //    isLoaded = false;
    //    loadingBlind = GameObject.FindGameObjectWithTag("Blind").GetComponent<TweenAlpha>();
    //    loadingBlind.GetComponent<UISprite>().color = new Color32(255, 255, 255, 255);
    //    StartAction();
    //}

    void DataCreate()
    {
        folderPath = (Application.platform == RuntimePlatform.Android ? Application.persistentDataPath : Application.dataPath) + "/Resources/Data/";
        shipFilePath = Path.Combine(folderPath, "PlayerShipInfoData.json");

        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);
    }

    public void DataDelete()
    {
        if (File.Exists(shipFilePath))
            File.Delete(shipFilePath);

        PlayerPrefs.DeleteAll();
        GoLobbyScene();
    }

    void StartAction()
    {
        loadingBlind.delay = 1.0f;
        loadingBlind.PlayForward();
    }

    void LoadAction()
    {
        loadingBlind.delay = 0.0f;
        loadingBlind.PlayReverse();
    }

    void GoLobbyScene()
    {
        LoadAction();
        StartCoroutine("StartLoad", "02_Lobby");
    }

    void GoGameScene()
    {
        LoadAction();
        StartCoroutine("StartLoad", "03_Battle");
    }

    public void NextLobbyScene()
    {
        Time.timeScale = 1.0f;
        GoLobbyScene();
    }

    public void NextGameScene()
    {
        Time.timeScale = 1.0f;
        GoGameScene();
    }

    public void ResetGame()
    {
        if (File.Exists(shipFilePath))
            File.Delete(shipFilePath);

        PlayerPrefs.DeleteAll();

        LoadAction();
        StartCoroutine("StartLoad", "01_Title");
    }

    public IEnumerator StartLoad(string startSceneName)
    {
        if (isLoaded == false)
        {
            yield return new WaitForSeconds(1f);

            isLoaded = true;
            SceneManager.LoadScene(startSceneName);
        }
    }
}
