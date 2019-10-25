using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class LogManager : MonoBehaviour {

	public static LogManager instance;

	public Transform Logger;
	public Transform Unused;

	public float LogInterval;
	public float LogLifetime;
	public float LogAppearTime;
	public int MaxShowingLog;

	[Space]
	public Sprite[] iconSet;
	public string[] textSet;
	public int[] colorSet;

	private void Awake()
	{
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy(gameObject);
	}


	private void Update()
	{
		if(ViewManager.Instance.retry)
			GetComponent<RectTransform>().localPosition = Vector2.Lerp(GetComponent<RectTransform>().localPosition, new Vector2(-1200, -550f), 0.15f);
	}

	public void CreateLog(LogType lt, int num)
	{
		LogNumberAdder();

		Transform curr = Unused.GetChild(0);
		curr.gameObject.SetActive(true);
		curr.SetParent(Logger);

		string inputString = "";
		if (lt == LogType.Playcount)
		{
			inputString = "Your " + AddOrdinal(num) + " Play!";
		}
		else if(lt == LogType.Score)
		{
			inputString = num.ToString();
			if (GameManager.Instance.combo > 1)
				inputString += " ( x" + GameManager.Instance.combo.ToString() + " )";
        }
		else
		{
			inputString = textSet[(int)lt];
		}
		StartCoroutine(curr.GetComponent<LogBlock>().Appear(iconSet[(int)lt], inputString, ViewManager.Instance.colorPalette[colorSet[(int)lt]]));
	}

	public void CreateLog(string tuto)
	{
		LogNumberAdder();

		Transform curr = Unused.GetChild(0);
		curr.gameObject.SetActive(true);
		curr.SetParent(Logger);

		StartCoroutine(curr.GetComponent<LogBlock>().Appear(iconSet[4], tuto, ViewManager.Instance.colorPalette[colorSet[4]]));
	}


	public void DeleteAllLog()
	{
		for (int i = 0; i < Logger.childCount; i++)
		{
			StartCoroutine(Logger.GetChild(i).gameObject.GetComponent<LogBlock>().Disappear());
        }
	}

	public void LogNumberAdder()
	{
		for (int i = 0; i < Logger.childCount; i++)
		{
			Logger.GetChild(i).gameObject.GetComponent<LogBlock>().logNumber++;
		}
	}

	public static string AddOrdinal(int num)
	{
		if (num <= 0) return num.ToString();

		switch (num % 100)
		{
			case 11:
			case 12:
			case 13:
				return num + "th";
		}

		switch (num % 10)
		{
			case 1:
				return num + "st";
			case 2:
				return num + "nd";
			case 3:
				return num + "rd";
			default:
				return num + "th";
		}

	}
}

public enum LogType
{
	debug = 0,
	Playcount = 1,
	Score = 2,
	Danger = 3,
	Tutorial = 4,
	GameOver = 5
}
