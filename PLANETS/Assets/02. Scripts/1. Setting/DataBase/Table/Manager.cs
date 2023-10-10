using UnityEngine;

public class Manager : MonoBehaviour
{
	static Manager instance;
	int turn, credit, core, dmatter, acore;

	void Awake()
	{
		instance = this;

        if (!PlayerPrefs.HasKey("Turn"))
        {
            turn = 1;
            PlayerPrefs.SetInt("Turn", turn);
        }
        else
            turn = PlayerPrefs.GetInt("Turn");

        //Resource
        if (!PlayerPrefs.HasKey("Credit"))
        {
            credit = 10000;
            PlayerPrefs.SetInt("Credit", credit);
        }
        else
            credit = PlayerPrefs.GetInt("Credit");

        if (!PlayerPrefs.HasKey("Core"))
        {
            core = 0;
            PlayerPrefs.SetInt("Core", core);
        }
        else
            core = PlayerPrefs.GetInt("Core");

        if (!PlayerPrefs.HasKey("DMatter"))
        {
            dmatter = 0;
            PlayerPrefs.SetInt("DMatter", dmatter);
        }
        else
            dmatter = PlayerPrefs.GetInt("DMatter");

        if (!PlayerPrefs.HasKey("ACore"))
        {
            acore = 0;
            PlayerPrefs.SetInt("ACore", acore);
        }
        else
            acore = PlayerPrefs.GetInt("ACore");
    }

	public static Manager Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new GameObject("Manager").AddComponent<Manager>();
			}
			return instance;
		}
	}

    public int Turn
    {
        get
        {
            return turn;
        }
        set
        {
            PlayerPrefs.SetInt("Turn", value);
            Turn = value;
        }
    }

    public int Credit
	{
		get 
		{
			return credit;
		}
		set 
		{
			PlayerPrefs.SetInt("Credit", value);
            credit = value;
		}
	}

    public int Core
    {
        get
        {
            return core;
        }
        set
        {
            PlayerPrefs.SetInt("Core", value);
            core = value;
        }
    }

    public int DMatter 
	{
		get 
		{
			return dmatter;
		}
		set 
		{
			PlayerPrefs.SetInt("DMatter", value);
            dmatter = value;
		}
	}

    public int ACore
    {
        get
        {
            return acore;
        }
        set
        {
            PlayerPrefs.SetInt("ACore", value);
            acore = value;
        }
    }
}