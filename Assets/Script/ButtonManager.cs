using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour {

	public void Pause()
	{
		if(GameManager.Instance.gameStatus == GameStatus.Playing)
			GameManager.Instance.gameStatus = GameStatus.Pause;
		else if(GameManager.Instance.gameStatus == GameStatus.Pause)
			GameManager.Instance.gameStatus = GameStatus.Playing;
	}

	public void Retry()
	{
		GameManager.Instance.score = 0;
		ViewManager.Instance.retry = true;
		LogManager.instance.DeleteAllLog();
	}

	public void Return()
	{
		GameManager.Instance.gameStatus = GameStatus.Playing;
	}

	public void Quit()
	{
		GameManager.Instance.SaveData();
		Debug.Log("Quit");
		Application.Quit();
	}

	public void Tutorial()
	{
		if (GameManager.Instance.gameStatus == GameStatus.Playing)
			GameManager.Instance.gameStatus = GameStatus.Tutorial;
		else if (GameManager.Instance.gameStatus == GameStatus.Tutorial)
			GameManager.Instance.gameStatus = GameStatus.Playing;
	}
}
