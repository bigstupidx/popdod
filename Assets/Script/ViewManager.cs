using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent (typeof(GameManager))]
public class ViewManager : MonoBehaviour {

	public static ViewManager Instance;

	[Header ("Block")]
	public float xInterval;
	public float yInterval;

	[Space]
	public float burstTime;
	public float burstInterval;
	public float spawningTime;
	public float spawningInterval;
	public float movingTimeToDest;
	public int burstDelay;

	[Space]
	public Color[] colorPalette;
	public Color[] blurColor;
	public Sprite[] ColorBlindImage;
	public Vector2 BlockScale;

	// 0 ~ 6 block color, 7 background, 8 blur, 9 text, 10 ~ 12 warning, 13 panel background
	public Color[] BrightMode;
	public Color[] DarknessMode;

	[Header("UI")]
	public Text scoreText;
	public Text highscoreText;
	public Text otherText;
	public Text gameoverText;
	public Text[] num;
	public Image[] image;
	public GameObject upWarning;
	public GameObject DownWarning;
	public GameObject LeftWarning;
	public GameObject RightWarning;
	public Color[] backgroundColor;
	Color targetColor;
	public GameObject pausePanel;
	public GameObject SoundOn;
	public GameObject SoundOff;
	public GameObject DarkOn;
	public GameObject DarkOff;
	public Image pauseRetryButton;
	public Image pauseReturnButton;
	public Image pauseSoundButton;
	public Image pauseDarkButton;

	public GameObject gameoverPanel;
	public GameObject TutorialPanel;
	public Text TouchCount;
	public Text RemovedCount;
	public bool retry;
	public Button pauseButton;
	public GameObject pauseButton_pause;
	public Button QuitButton;
	public Image tempQuit;

	[Space]
	public Text LeftBox;
	public Text RightBox;
	public Vector3 prevMousePosition;

	[Space]
	public Transform LeftPreview;
	public Transform RightPreview;

	[Header("Game Object")]
	public GameObject BlockContainer;
	public GameObject currentBlockContainer;
	public GameObject Blur;
	public GameObject MainCamera;

	int oldScore;
	int newScore;
    float timer;
	float scaleTime;

	[Space]
	public GameObject[] TutorialScene;
	int tutoNum;

	[Space]
	public Text playcount;

	private void Awake()
	{
		if (Instance == null)
			Instance = this;
		else if (Instance != this)
			Destroy(gameObject);
	}

	private void Start()
	{
		targetColor = backgroundColor[0];
		prevMousePosition = new Vector3(-1000, -1000, 0);
		retry = false;

		oldScore = 0;
		newScore = 0;
		timer = 0f;
		scaleTime = 0f;

		tutoNum = 0;
		highscoreText.text = (10 * GameManager.Instance.highscore).ToString();
		if (GameManager.Instance.tutorialMode)
			otherText.text = "Tutorial";
		else
			otherText.text = "New Record!";
		SetButton();
		playcount.text = (GameManager.Instance.playcount).ToString();
		LogManager.instance.CreateLog(LogType.Playcount, GameManager.Instance.playcount);
	}

	private void LateUpdate()
	{
		tempQuit.color = QuitButton.GetComponent<Image>().color;

		if (!retry)
		{
			SmoothScoreIncrease();
		}
		else
		{
			scoreText.text = ((int)(Mathf.Lerp(int.Parse(scoreText.text), 0, 0.35f))).ToString();

			Color c = highscoreText.color;
			c.a = Mathf.Lerp(c.a, 0f, 0.2f);
			highscoreText.color = c;
			Color d = otherText.color;
			d.a = Mathf.Lerp(d.a, 0f, 0.2f);
			otherText.color = d;
		}

		int bc = -GameManager.Instance.mapSetting.MAX_HEIGHT + GameManager.Instance.map.container.Count;
		if (retry)
			targetColor = backgroundColor[0];
		else if (bc < -1)
			targetColor = backgroundColor[0];
		else if (bc == -1)
			targetColor = backgroundColor[1];
		else if (bc == 0)
			targetColor = backgroundColor[2];
		else
			targetColor = backgroundColor[3];

		upWarning.GetComponent<Image>().color = Color.Lerp(upWarning.GetComponent<Image>().color, targetColor, 0.1f);
		DownWarning.GetComponent<Image>().color = upWarning.GetComponent<Image>().color;
		RightWarning.GetComponent<SpriteRenderer>().color = upWarning.GetComponent<Image>().color;
		LeftWarning.GetComponent<SpriteRenderer>().color = upWarning.GetComponent<Image>().color;

		Color blurTarget = (GameManager.Instance.gameStatus == GameStatus.Playing) ? blurColor[0] : blurColor[1];
		Blur.GetComponent<SpriteRenderer>().color = Color.Lerp(Blur.GetComponent<SpriteRenderer>().color, blurTarget, 0.2f);

		Vector2 pauseTarget = (GameManager.Instance.gameStatus == GameStatus.Pause) ? Vector2.zero : new Vector2(1200, 0);
		if(!retry)
		pausePanel.GetComponent<RectTransform>().localPosition = Vector2.Lerp(pausePanel.GetComponent<RectTransform>().localPosition, pauseTarget, 0.15f);
		else if(pausePanel.GetComponent<RectTransform>().localPosition.x < 100)
			pausePanel.GetComponent<RectTransform>().localPosition = Vector2.Lerp(pausePanel.GetComponent<RectTransform>().localPosition, new Vector2(-1200, 0), 0.15f);

		if (retry)
		{
			targetColor = backgroundColor[0];
			MainCamera.transform.localPosition = Vector2.Lerp(MainCamera.transform.localPosition, new Vector2(6.5f, 0), 0.2f);
		}

		if (scoreText.text == "0" && retry && MainCamera.transform.localPosition.x > 6) 
			GameManager.Instance.SceneReload();

		if (GameManager.Instance.gameStatus == GameStatus.GameOver && !retry)
		{
			if(!gameoverPanel.activeSelf)
				StartCoroutine(SetGameOverPanel());
			//gameoverPanel.GetComponent<RectTransform>().localPosition = Vector2.Lerp(gameoverPanel.GetComponent<RectTransform>().localPosition, Vector2.zero, 0.15f);
		}
		else if (GameManager.Instance.gameStatus == GameStatus.GameOver && retry)
		{
			gameoverPanel.GetComponent<RectTransform>().localPosition = Vector2.Lerp(gameoverPanel.GetComponent<RectTransform>().localPosition, new Vector2(-1200, 0), 0.15f);
		}

	
		if((GameManager.Instance.gameStatus == GameStatus.Playing && !QuitButton.enabled) || retry)
		{
			QuitButton.enabled = true;
			pauseButton.enabled = true;
			pauseButton_pause.SetActive(false);

			Color tmp = QuitButton.GetComponent<Image>().color;
			tmp.a = 1.0f;
			QuitButton.GetComponent<Image>().color = tmp;
			pauseButton.GetComponent<Image>().color = tmp;
		}
		else if (GameManager.Instance.gameStatus != GameStatus.Playing && QuitButton.enabled)
		{
			QuitButton.enabled = false;
			pauseButton.enabled = false;

			Color tmp = QuitButton.GetComponent<Image>().color;
			tmp.a = 0.2f;
			pauseButton.GetComponent<Image>().color = tmp;
		}
		if(GameManager.Instance.gameStatus == GameStatus.Pause)
		{
			pauseButton_pause.SetActive(true);
		}
		
		if (TutorialPanel.activeSelf)
		{
			if(GameManager.Instance.gameStatus != GameStatus.Tutorial)
			{
				TutorialPanel.GetComponent<RectTransform>().localPosition = Vector2.Lerp(TutorialPanel.GetComponent<RectTransform>().localPosition, new Vector2(-1200, 0), 0.15f);
				if (TutorialPanel.GetComponent<RectTransform>().localPosition.x < -1150)
				{
					TutorialPanel.SetActive(false);
					tutoNum = 0;
				}
			}
			else
			{
				TutorialPanel.GetComponent<RectTransform>().localPosition = Vector2.Lerp(TutorialPanel.GetComponent<RectTransform>().localPosition, new Vector2(0, 0), 0.15f);
			}

			for(int i = 0; i < TutorialScene.Length; i++)
			{
				if (i == tutoNum)
					TutorialScene[i].SetActive(true);
				else
					TutorialScene[i].SetActive(false);
			}
		}
		else
		{
			if (GameManager.Instance.gameStatus == GameStatus.Tutorial)
				TutorialPanel.SetActive(true);
        }
	}

	public void SetButton()
	{
		if (GameManager.Instance.DarknessMode)
		{
			DarkOn.SetActive(true);
			DarkOff.SetActive(false);
		}
		else
		{
			DarkOn.SetActive(false);
			DarkOff.SetActive(true);
		}

		if (GameManager.Instance.audioSource.mute)
		{
			SoundOn.SetActive(false);
			SoundOff.SetActive(true);
		}
		else
		{
			SoundOn.SetActive(true);
			SoundOff.SetActive(false);
		}
	}

	private void SmoothScoreIncrease()
	{
        if (newScore != GameManager.Instance.score)
		{
			scaleTime = burstDelay * burstInterval + burstTime;
			oldScore = newScore;
			newScore = GameManager.Instance.score;
			timer = 0f;
		}

		int curr = (int)Mathf.Lerp(oldScore, newScore, 1 - Mathf.Pow(1 - timer / scaleTime, 2));
		if (int.Parse(scoreText.text) < 10 * curr)
			scoreText.text = (10 * curr).ToString();

		if (timer < scaleTime)
			timer += Time.deltaTime;
		else
			scoreText.text = (10 * GameManager.Instance.score).ToString();

		if (int.Parse(highscoreText.text) <= int.Parse(scoreText.text) || GameManager.Instance.tutorialMode)
		{
			highscoreText.text = scoreText.text;

			Color c = highscoreText.color;
			c.a = Mathf.Lerp(c.a, 0f, 0.2f);
			highscoreText.color = c;
			Color d = otherText.color;
			d.a = Mathf.Lerp(d.a, 1f, 0.2f);
			otherText.color = d;
		}
		else
		{
			Color c = highscoreText.color;
			c.a = Mathf.Lerp(c.a, 1f, 0.2f);
			highscoreText.color = c;
			Color d = otherText.color;
			d.a = Mathf.Lerp(d.a, 0f, 0.2f);
			otherText.color = d;
		}
	}

	public void SetView()
	{
		Map map = GameManager.Instance.map;

		for (int i = currentBlockContainer.transform.childCount - 1; i >= 0; i--)
		{
			Block blk = currentBlockContainer.transform.GetChild(i).gameObject.GetComponent<BlockObject>().block;
			if (map.Contains(blk)) {
				StartCoroutine(currentBlockContainer.transform.GetChild(i).gameObject.GetComponent<BlockObject>().BlockMove(
					new Vector2(xInterval * ((map.container.Count - 1) * 0.5f - map.FindIndex(blk).First),
					yInterval * ((map.container[map.FindIndex(blk).First].Count - 1) * 0.5f - map.FindIndex(blk).Second))));
			}
			else
			{
				StartCoroutine(currentBlockContainer.transform.GetChild(i).gameObject.GetComponent<BlockObject>().BlockPop(currentBlockContainer.transform.GetChild(i).gameObject.GetComponent<BlockObject>().dist / 2));
			}
		}

		StartCoroutine(SetBox(burstDelay * burstInterval + burstTime));

		for(int i = 0; i < GameManager.Instance.rightAddLineCount; i++)
		{
			for(int j = 0; j < map.container[i].Count; j++)
			{
				Transform tmp = BlockContainer.transform.GetChild(0);
				tmp.localScale = BlockScale;
				tmp.SetParent(currentBlockContainer.transform);
				tmp.localPosition = new Vector2(2.9f, yInterval * ((map.container[i].Count - 1) * 0.5f - j));
				tmp.gameObject.GetComponent<BlockObject>().block = map.container[i][j];
				tmp.gameObject.SetActive(true);
				tmp.gameObject.GetComponent<BlockObject>().SetBlock();
				StartCoroutine(tmp.gameObject.GetComponent<BlockObject>().BlockMove(new Vector2(xInterval * ((map.container.Count - 1) * 0.5f - i), yInterval * ((map.container[i].Count - 1) * 0.5f - j))));
			}
		}

		for(int i = map.container.Count - 1; map.container.Count -1 - i < GameManager.Instance.LeftAddLineCount; i--)
		{
			for(int j = 0; j < map.container[i].Count; j++)
			{
				Transform tmp = BlockContainer.transform.GetChild(0);
				tmp.localScale = BlockScale;
				tmp.SetParent(currentBlockContainer.transform);
				tmp.localPosition = new Vector2(-2.9f, yInterval * ((map.container[i].Count - 1) * 0.5f - j));
				tmp.gameObject.GetComponent<BlockObject>().block = map.container[i][j];
				tmp.gameObject.SetActive(true);
				tmp.gameObject.GetComponent<BlockObject>().SetBlock();
				StartCoroutine(tmp.gameObject.GetComponent<BlockObject>().BlockMove(new Vector2(xInterval * ((map.container.Count - 1) * 0.5f - i), yInterval * ((map.container[i].Count - 1) * 0.5f - j))));
			}
		}
	}

	public void SetViewInit()
	{
		StartCoroutine (SetBox(0));
		if (currentBlockContainer.transform.childCount >= 1)
		{
			foreach (Transform child in currentBlockContainer.transform)
			{
				child.gameObject.SetActive(false);
				child.SetParent(BlockContainer.transform);
			}
		}
		oldScore = 0;
		newScore = GameManager.Instance.score;
		scaleTime = 0.5f;
		timer = 0;

		Map map = GameManager.Instance.map;
		int midLine = map.container.Count / 2;
		int midIdx = map.container[midLine].Count / 2;
		Block mid = map.container[midLine][midIdx];
		for(int i = 0; i < map.container.Count; i++)
		{
			for(int j = 0; j < map.container[i].Count; j++)
			{
				Transform tmp = BlockContainer.transform.GetChild(0);
				tmp.SetParent(currentBlockContainer.transform);
				tmp.localPosition = new Vector2(xInterval * ((map.container.Count - 1) * 0.5f - i), yInterval * ((map.container[i].Count - 1) * 0.5f - j));
				tmp.gameObject.GetComponent<BlockObject>().block = map.container[i][j];
				tmp.gameObject.SetActive(true);
				tmp.gameObject.GetComponent<BlockObject>().SetBlock();

				StartCoroutine(tmp.gameObject.GetComponent<BlockObject>().BlockSpawn(10 + 
					map.GetDistance(mid, tmp.gameObject.GetComponent<BlockObject>().block)));
			} 
		}
	}

	public IEnumerator SetPreviewBox(int t, Transform tmp)
	{
		yield return new WaitForSeconds(t * spawningInterval);
		tmp.gameObject.SetActive(true);

		
		Vector2 OriginScale = tmp.localScale;
		tmp.localScale = Vector2.zero;

		float timer = 0;
		while (timer < spawningTime)
		{
			float x = (timer / spawningTime);
			tmp.localScale = Vector2.Lerp(Vector2.zero, OriginScale, 1 - Mathf.Pow(1 - x, 2));
			yield return new WaitForSeconds(Time.deltaTime);
			timer += Time.deltaTime;
		}

		tmp.localScale = OriginScale;
		
	}

	public IEnumerator SetBox(float t)
	{
		//yield return new WaitForSeconds(t);
		yield return null;
		if (GameManager.Instance.leftLeft == 1 && GameManager.Instance.gameStatus != GameStatus.GameOver) {
			for (int i = 0; i < LeftPreview.childCount; i++)
			{
				if (GameManager.Instance.leftString.Length > i) {
					LeftPreview.GetChild(i).gameObject.GetComponent<SpriteRenderer>().color = colorPalette[(int)(GameManager.Instance.leftString[i] - 'A')];
					LeftPreview.GetChild(i).GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = ColorBlindImage[(int)(GameManager.Instance.leftString[i] - 'A')];
                    LeftPreview.GetChild(i).position = new Vector2(-2.9f, yInterval * ((GameManager.Instance.leftString.Length - 1) * 0.5f - i));
					StartCoroutine(SetPreviewBox(Mathf.Abs(i - GameManager.Instance.leftString.Length / 2) , LeftPreview.GetChild(i)));
				}
			}
		}
		else
		{
			for (int i = 0; i < LeftPreview.childCount; i++)
			{
				LeftPreview.GetChild(i).gameObject.SetActive(false);
			}
		}

		if (GameManager.Instance.rightLeft == 1 && GameManager.Instance.gameStatus != GameStatus.GameOver)
		{
			for (int i = 0; i < RightPreview.childCount; i++)
			{
				if (GameManager.Instance.rightString.Length > i)
				{
					RightPreview.GetChild(i).gameObject.GetComponent<SpriteRenderer>().color = colorPalette[(int)(GameManager.Instance.rightString[i] - 'A')];
					RightPreview.GetChild(i).position = new Vector2(2.9f, yInterval * ((GameManager.Instance.rightString.Length - 1) * 0.5f - i));
					RightPreview.GetChild(i).GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = ColorBlindImage[(int)(GameManager.Instance.rightString[i] - 'A')];
                    StartCoroutine(SetPreviewBox(Mathf.Abs(i - GameManager.Instance.rightString.Length / 2), RightPreview.GetChild(i)));
				}
			}
		}
		else
		{
			for (int i = 0; i < RightPreview.childCount; i++)
			{
				RightPreview.GetChild(i).gameObject.SetActive(false);
			}
		}

		LeftBox.text = GameManager.Instance.leftLeft.ToString();
		RightBox.text = GameManager.Instance.rightLeft.ToString();

		yield return new WaitForSeconds(burstDelay * burstInterval + burstTime);
		int bc = -GameManager.Instance.mapSetting.MAX_HEIGHT + GameManager.Instance.map.container.Count;
		if (bc == 0)
			LogManager.instance.CreateLog(LogType.Danger, 0);
	}

	public void ActiveHighlighted(Block b)
	{
		burstDelay = 0;
		List<Block> tmp = GameManager.Instance.map.FindBlockListConnected(b);
		if (tmp.Count >= 5)
			tmp.AddRange(GameManager.Instance.map.FindBlockListColor(b.blockColor));

		for(int i  = 0; i < currentBlockContainer.transform.childCount; i++)
		{
			if (tmp.Contains(currentBlockContainer.transform.GetChild(i).gameObject.GetComponent<BlockObject>().block))
			{
				currentBlockContainer.transform.GetChild(i).gameObject.GetComponent<BlockObject>().Highlighted.SetActive(true);
				currentBlockContainer.transform.GetChild(i).gameObject.GetComponent<BlockObject>().dist =
					GameManager.Instance.map.GetDistance(b, currentBlockContainer.transform.GetChild(i).gameObject.GetComponent<BlockObject>().block);

				burstDelay = Mathf.Max(currentBlockContainer.transform.GetChild(i).gameObject.GetComponent<BlockObject>().dist, burstDelay);
			}
		}
	}

	public void DeactiveHighlighted()
	{
		for (int i = 0; i < currentBlockContainer.transform.childCount; i++)
		{
			currentBlockContainer.transform.GetChild(i).gameObject.GetComponent<BlockObject>().Highlighted.SetActive(false);
		}
		burstDelay = 0;
	}

	// 0 ~ 6 block color, 7 background, 8 blur, 9 text, 10 ~ 12 warning, 13 panel background
	public void SetVisualMode(bool dark)
	{
		if (dark)
		{
			for (int i = 0; i < 7; i++)
				colorPalette[i] = DarknessMode[i];

			MainCamera.GetComponent<Camera>().backgroundColor = DarknessMode[7];
			blurColor[0] = DarknessMode[8];
			blurColor[0].a = 0f;
			blurColor[1] = DarknessMode[8];

			highscoreText.color = DarknessMode[9];
			otherText.color = DarknessMode[9];
			scoreText.color = DarknessMode[9];
			gameoverText.color = DarknessMode[9];

			for(int i = 0; i < 4; i++)
			{
				image[i].color = DarknessMode[9];
			}
			num[0].color = DarknessMode[9];
			num[1].color = DarknessMode[9];
			
			backgroundColor[0] = blurColor[0];
			backgroundColor[1] = DarknessMode[10];
			backgroundColor[2] = DarknessMode[11];
			backgroundColor[3] = DarknessMode[11];
			pausePanel.GetComponent<Image>().color = DarknessMode[12];
			gameoverPanel.GetComponent<Image>().color = DarknessMode[12];
		}
		else
		{
			for (int i = 0; i < 7; i++)
				colorPalette[i] = BrightMode[i];

			MainCamera.GetComponent<Camera>().backgroundColor = BrightMode[7];
			blurColor[0] = BrightMode[8];
			blurColor[0].a = 0f;
			blurColor[1] = BrightMode[8];

			highscoreText.color = BrightMode[9];
			otherText.color = BrightMode[9];
			scoreText.color = BrightMode[9];
			gameoverText.color = BrightMode[9];

			for (int i = 0; i < 4; i++)
			{
				image[i].color = BrightMode[9];
			}
			num[0].color = BrightMode[9];
			num[1].color = BrightMode[9];

			backgroundColor[0] = blurColor[0];
			backgroundColor[1] = BrightMode[10];
			backgroundColor[2] = BrightMode[11];
			backgroundColor[3] = BrightMode[11];
			pausePanel.GetComponent<Image>().color = BrightMode[12];
			gameoverPanel.GetComponent<Image>().color = BrightMode[12];
		}

		pauseRetryButton.color = colorPalette[0];
		pauseReturnButton.color = colorPalette[1];
		pauseSoundButton.color = colorPalette[2];
		pauseDarkButton.color = colorPalette[3];

		for(int i = 0; i < currentBlockContainer.transform.childCount; i++)
		{
			currentBlockContainer.transform.GetChild(i).gameObject.GetComponent<BlockObject>().SetBlock();
		}
		if (GameManager.Instance.leftLeft == 1 && GameManager.Instance.gameStatus != GameStatus.GameOver)
		{
			for (int i = 0; i < LeftPreview.childCount; i++)
			{
				if (GameManager.Instance.leftString.Length > i)
				{
					LeftPreview.GetChild(i).gameObject.GetComponent<SpriteRenderer>().color = colorPalette[(int)(GameManager.Instance.leftString[i] - 'A')];
				}
			}
		}
		for (int i = 0; i < RightPreview.childCount; i++)
		{
			if (GameManager.Instance.rightString.Length > i)
			{
				RightPreview.GetChild(i).gameObject.GetComponent<SpriteRenderer>().color = colorPalette[(int)(GameManager.Instance.rightString[i] - 'A')];
			}
		}

		if (!QuitButton.enabled)
		{
			Color c = QuitButton.GetComponent<Image>().color;
			c.a = 0.2f;
			pauseButton.GetComponent<Image>().color = c;
		}

		if (int.Parse(highscoreText.text) < int.Parse(scoreText.text) || GameManager.Instance.tutorialMode)
		{
			highscoreText.text = scoreText.text;

			Color c = highscoreText.color;
			c.a = 0f;
			highscoreText.color = c;
			Color d = otherText.color;
			d.a = 0f;
			otherText.color = d;
		}
		else
		{
			Color c = highscoreText.color;
			c.a = 0f;
			highscoreText.color = c;
			Color d = otherText.color;
			d.a = 0f;
			otherText.color = d;
		}
	}

	public void NextScene()
	{
		if (tutoNum + 1 < TutorialScene.Length)
			tutoNum++;
		else
			GameManager.Instance.gameStatus = GameStatus.Playing;
	}

	public IEnumerator SetGameOver()
	{
		yield return new WaitForSeconds(burstDelay * burstInterval + burstTime);
		TouchCount.text = GameManager.Instance.map.tap.ToString();
		RemovedCount.text = GameManager.Instance.map.removedBlock.ToString();
		GameManager.Instance.gameStatus = GameStatus.GameOver;
	}

	public IEnumerator SetGameOverPanel()
	{
		gameoverPanel.SetActive(true);
		yield return new WaitForSeconds(0.05f);
		LogManager.instance.CreateLog(LogType.GameOver, 0);
	}
}