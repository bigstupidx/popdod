using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogBlock : MonoBehaviour {

	public LogManager LM;

	public Image IconBack;
	public Image IconImage;
	public Image DescriptBack;
	public Text DescriptText;

	[HideInInspector]
	public int logNumber;
	[HideInInspector]
	public float lifeTime;
	[HideInInspector]
	public bool disappearing;

	private void Update()
	{
		GetComponent<RectTransform>().localPosition = Vector3.Lerp(GetComponent<RectTransform>().localPosition, new Vector3(0f, -1f * LM.LogInterval * logNumber, 0f), 0.3f);

		lifeTime += Time.deltaTime;

		if(logNumber >= LM.MaxShowingLog || lifeTime > LM.LogLifetime)
		{
			if (!disappearing)
				StartCoroutine(Disappear());

			disappearing = true;
		}
	}

	public IEnumerator Appear(Sprite icon, string text, Color c)
	{
		disappearing = false;
		SetColor(c);
		IconImage.sprite = icon;
		DescriptText.text = text;

		SetAlpha(0f);
		GetComponent<RectTransform>().localPosition = Vector3.zero;

		lifeTime = 0;
		logNumber = 0;

		float timer = 0;
		while(timer < LM.LogAppearTime)
		{
			SetAlpha(Mathf.Lerp(0f, 1f, timer / LM.LogAppearTime));

			timer += Time.deltaTime;
			yield return new WaitForSeconds(Time.deltaTime);
		}
		SetAlpha(1f);
		yield return null;
	}

	public IEnumerator Disappear()
	{
		yield return null;
		float timer = 0;
		while (timer < LM.LogAppearTime)
		{
			SetAlpha(Mathf.Lerp(1f, 0f, timer / LM.LogAppearTime));

			timer += Time.deltaTime;
			yield return new WaitForSeconds(Time.deltaTime);
		}
		SetAlpha(0f);

		transform.SetParent(LM.Unused);
		logNumber = -100;
		lifeTime = -100f;
		gameObject.SetActive(false);
	}

	private void SetColor(Color c)
	{
		IconBack.color = c;
		DescriptBack.color = c;

		IconImage.color = Color.white;
		DescriptText.color = Color.white;
	}

	private void SetAlpha(float a)
	{
		Color outer = IconBack.color;
		outer.a = a;
		IconBack.color = outer;
		DescriptBack.color = outer;

		IconImage.color = new Color(1f, 1f, 1f, a);
		DescriptText.color = new Color(1f, 1f, 1f, a);
	}
}
