using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(MapSetting))]
public class GameManager : MonoBehaviour {

	public static GameManager Instance;

	public Map map;
	[HideInInspector]
	public MapSetting mapSetting;

	public int score;
	public int highscore;
	public int playcount;
	public int combo;
	public GameStatus gameStatus;
	public UserStatus userStatus;

	[Space]
	public string rightString;
	public string leftString;
	public int rightLeft;
	public int leftLeft;

	[Space]
	public int rightAddLineCount;
	public int LeftAddLineCount;

	[Space]
	public bool Mute;
	public bool DarknessMode;
	public AudioClip popSound;
	public AudioClip SliderMove;
	public AudioSource audioSource;

	public Camera MainCamera;

	bool bPaused = false;
	public bool tutorialMode;
	int tutorialPhase;

	private void Awake()
	{
		Application.targetFrameRate = 60;

		if (Instance == null)
			Instance = this;
		else if (Instance != this)
			Destroy(gameObject);

		tutorialMode = false;
		map = new Map();
		mapSetting = GetComponent<MapSetting>();
		audioSource = GetComponent<AudioSource>();

		LoadData();
		if (highscore == 0)
		{
			tutorialMode = true;
			tutorialPhase = 1;
		}
	}

	private void Start () {
		if (userStatus.score == 0)
		{
			InitMap();
		}
		else
		{
			playcount--;
		}
		
		playcount++;

		map.RemoveEmptyLine();
		ViewManager.Instance.SetVisualMode(DarknessMode);
		StartCoroutine(lateStart());

		gameStatus = GameStatus.Playing;
	}

	private IEnumerator lateStart()
	{
		yield return new WaitForSeconds(0.5f);
		ViewManager.Instance.SetViewInit();
	}
	
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			if (gameStatus == GameStatus.Playing)
			{
				gameStatus = GameStatus.Pause;
			}
			else if (gameStatus == GameStatus.Pause)
			{
				gameStatus = GameStatus.Playing;
			}
		}

		if (highscore < score)
			highscore = score;

		if (Input.GetKeyDown(KeyCode.R))
		{
			Reset();
		}
	}

	private void InitMap()
	{
		rightAddLineCount = 0;
		LeftAddLineCount = 0;
		score = 0;
		combo = 1;

		map.container.Clear();

		if (!tutorialMode)
		{
			for (int i = 0; i < 5; i++)
				map.AddLineRight(mapSetting.GetLine(0));

			rightLeft = mapSetting.GetLeft(leftLeft);
			rightString = mapSetting.GetLine(map.tap);
			leftLeft = mapSetting.GetLeft(rightLeft);
			leftString = mapSetting.GetLine(map.tap);
		}
		else
		{
			map.AddLineRight("BA");
			map.AddLineRight("ABB");
			map.AddLineRight("BA");
			rightLeft = 2;
			leftLeft = 3;
			rightString = "GCCC";
			leftString = "DGDD";

			LogManager.instance.CreateLog("Touch to pop!");
		}
	}

	void IncreaseTap()
	{
		rightAddLineCount = 0;
		LeftAddLineCount = 0;
		rightLeft--;
		leftLeft--;

		map.tap++;
		if (!tutorialMode)
		{
			if (rightLeft <= 0)
			{
				rightAddLineCount++;
				rightLeft = mapSetting.GetLeft(leftLeft);
				map.AddLineRight(rightString);
				rightString = mapSetting.GetLine(map.tap);
			}

			if (leftLeft <= 0)
			{
				LeftAddLineCount++;
				leftLeft = mapSetting.GetLeft(rightLeft);
				map.AddLineLeft(leftString);
				leftString = mapSetting.GetLine(map.tap);
			}
		}
		else
		{
			if(tutorialPhase == 1)
			{
				if (rightLeft <= 0)
				{
					rightAddLineCount++;
					rightLeft = 100;
					map.AddLineRight(rightString);
					rightString = "ABCD";
					LogManager.instance.CreateLog("Pop with glass blocks!");
				}

				if (leftLeft <= 0)
				{
					LeftAddLineCount++;
					leftLeft = 100;
					map.AddLineLeft(leftString);
					leftString = "ABCD";
					LogManager.instance.CreateLog("Pop with glass blocks!");
				}		

				if (rightLeft + leftLeft > 100 && map.container.Count == 0)
				{
					tutorialPhase++;
					StartCoroutine(phase2());
					rightLeft = 30;
					leftLeft = 30;
				}
			}
			if(tutorialPhase == 2)
			{
				if (rightLeft + leftLeft < 59 && map.container.Count == 0)
				{
					tutorialPhase++;
					LogManager.instance.CreateLog("Ends when touched the red lines");
					StartCoroutine(phase3());
				}
				else
				{
					LogManager.instance.CreateLog("Pop 5 or more blocks!");
				}
			}
			if(tutorialPhase == 3)
			{
				if (rightLeft <= 0)
				{
					rightAddLineCount++;
					rightLeft = mapSetting.GetLeft(leftLeft);
					map.AddLineRight(rightString);
					rightString = mapSetting.GetLine(map.tap);
				}

				if (leftLeft <= 0)
				{
					LeftAddLineCount++;
					leftLeft = mapSetting.GetLeft(rightLeft);
					map.AddLineLeft(leftString);
					leftString = mapSetting.GetLine(map.tap);
				}
			}
		}
	}

	private IEnumerator phase2()
	{
		yield return null;
		yield return new WaitForSeconds(0.6f);

		map.container.Clear();
		//map.AddLineRight("GFBDG");
		map.AddLineRight("GBBADE");
		map.AddLineRight("FFCBADE");
		map.AddLineRight("GFCBAADE");
		map.AddLineRight("GACCGFFDG");
		map.AddLineRight("BADDEFCG");
		map.AddLineRight("BADEFCC");
		map.AddLineRight("BADEEG");
		//map.AddLineRight("GAECG");

		ViewManager.Instance.SetViewInit();
	}


	private IEnumerator phase3()
	{
		yield return null;
		yield return new WaitForSeconds(0.6f);

		map.container.Clear();
		map.AddLineRight("ABCDEFAB");
		map.AddLineRight("CDEFABCD");
		map.AddLineRight("EFABCDE");
		map.AddLineRight("ABCDEF");
		map.AddLineRight("CDEFAB");
		map.AddLineRight("EFABCDE");
		map.AddLineRight("ABCDEFAB");
		map.AddLineRight("CDEFABCD");

		rightLeft = 2;
		leftLeft = 3;
		rightString = "CDEFABCD";
		leftString = "ABCDEFAB";

		ViewManager.Instance.SetViewInit();
    }

	public void TouchBlock(Block b)
	{
		if (gameStatus != GameStatus.Playing)
			return;

		BlockColor c = b.blockColor;
		ViewManager.Instance.prevMousePosition = Input.mousePosition;
		int count = 0;

		count += map.RemoveBlockNormal(b);
		if (count >= mapSetting.COLOR_BURST)
		{
			count += map.RemoveBlockColor(c);
			combo++;
		}
		else
		{
			combo = 1;
		}

		map.removedLine += map.RemoveEmptyLine();
		map.removedBlock += count;
		score += count * combo;

		LogManager.instance.DeleteAllLog();
		LogManager.instance.CreateLog(LogType.Score, count * combo * 10);
		IncreaseTap();
		ViewManager.Instance.SetView();

		if(map.container.Count > mapSetting.MAX_HEIGHT)
		{
			StartCoroutine( ViewManager.Instance.SetGameOver());
		}
	}

	public void EnableSound()
	{
		audioSource.mute = !audioSource.mute;
		Mute = audioSource.mute;

		audioSource.PlayOneShot(SliderMove, 1.0f);
	}

	public void EnableDarkness()
	{
		DarknessMode = !DarknessMode;
		ViewManager.Instance.SetVisualMode(DarknessMode);

		audioSource.PlayOneShot(SliderMove, 1.0f);
	}
	
	public void PlaySound()
	{
		audioSource.PlayOneShot(popSound, 1.0f);
	}

	public void Reset()
	{
		SaveLoadManager.DeletePlayer();
		map = null;
		mapSetting = null;
		userStatus = null;
		StartCoroutine(Loading());
	}

	public void SceneReload()
	{
		SaveData();
		map = null;
		mapSetting = null;
		userStatus = null;
		StartCoroutine(Loading());
	}

	private IEnumerator Loading()
	{
		yield return null;
		AsyncOperation async;

		async = SceneManager.LoadSceneAsync(0);
		async.allowSceneActivation = false;

		while (async.isDone == false)
		{
			if (async.progress == .9f)
			{
				async.allowSceneActivation = true;
			}
			yield return null;
		}

		Debug.Log("done");
	}

	public void SaveData()
	{
		UserStatus us = new UserStatus();
		us.SetProperty(Mute, DarknessMode);
		us.SetUserStatus(highscore, playcount);

		if(gameStatus == GameStatus.GameOver || ViewManager.Instance.retry)
		{
			map = new Map();
			rightAddLineCount = 0;
			LeftAddLineCount = 0;
			score = 0;
			combo = 1;

			for (int i = 0; i < 5; i++)
				map.AddLineRight(mapSetting.GetLine(0));

			rightLeft = mapSetting.GetLeft(leftLeft);
			rightString = mapSetting.GetLine(map.tap);
			leftLeft = mapSetting.GetLeft(rightLeft);
			leftString = mapSetting.GetLine(map.tap);
		}

		if (tutorialMode)
		{
			us.SetMapInformation(map, rightString, leftString, rightLeft, leftLeft, 0);
		}
		else
		{
			us.SetMapInformation(map, rightString, leftString, rightLeft, leftLeft, score);
		}
		SaveLoadManager.SavePlayer(us);
	}

	public void LoadData()
	{
		userStatus = SaveLoadManager.LoadPlayer();

		if (userStatus.Playcount == 0)
		{
			Mute = false;
			DarknessMode = false;

			map = new Map();
			highscore = 0;
		}
		else
		{
			Mute = userStatus.Mute;
			audioSource.mute = userStatus.Mute;
			DarknessMode = userStatus.Darkness;

			map = new Map(userStatus);
			score = userStatus.score;
			rightString = userStatus.right;
			leftString = userStatus.left;
			rightLeft = userStatus.rightLeft;
			leftLeft = userStatus.leftLeft;

			highscore = userStatus.Highscore;
			playcount = userStatus.Playcount;
		}
	}

	void OnApplicationPause(bool pause)
	{
		if (pause)
		{
			bPaused = true;
			SaveData();
		}
		else
		{
			if (bPaused)
			{
				bPaused = false;
			}
		}
	}
}

public enum GameStatus { Playing, Pause, GameOver, Tutorial }