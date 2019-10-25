using GoogleMobileAds.Api;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Admanager : MonoBehaviour {

	bool hasShownAdOneTime;

	public string android_banner_id;
	public string android_interstitial_id;

	private BannerView bv;
	float timer;
	int score;
	
	public void Start()
	{
		RequestBannerAd();
		RequestInterstitialAds();

		hasShownAdOneTime = false;
		ShowBannerAd();
		timer = 0f;
	}

	public void Update()
	{
		if (!hasShownAdOneTime && GameManager.Instance.gameStatus == GameStatus.GameOver && !GameManager.Instance.tutorialMode && GameManager.Instance.playcount % 2 == 0)
		{
			score = GameManager.Instance.score;
			hasShownAdOneTime = true;
			StartCoroutine( ShowInterstitialAd());
		}

		timer += Time.deltaTime;
		if(timer > 60)
		{
			RequestBannerAd();
			ShowBannerAd();
			timer = 0;
		}
	}

	public void RequestBannerAd()
	{
        string adUnitId = android_banner_id;

		// Create a 320x50 banner at the top of the screen.
		BannerView bannerView = new BannerView(adUnitId, AdSize.SmartBanner, AdPosition.Bottom);
		// Create an empty ad request.
		AdRequest request = new AdRequest.Builder().Build();
		// Load the banner with the request.
		bannerView.LoadAd(request);
		bv = bannerView;
	}

	public void ShowBannerAd()
	{
		bv.Show();
	}

	public IEnumerator ShowInterstitialAd()
	{
		yield return new WaitForSeconds(1.0f);
		if (interstitial.IsLoaded())
		{
			interstitial.Show();
			Debug.Log("SHOW AD");
		}
	}

	InterstitialAd interstitial;
	private void RequestInterstitialAds()
	{
		// Initialize an InterstitialAd.
		interstitial = new InterstitialAd(android_interstitial_id);

		AdRequest request = new AdRequest.Builder().Build();

		//Register Ad Close Event
		interstitial.OnAdClosed += Interstitial_OnAdClosed;

		// Load the interstitial with the request.
		interstitial.LoadAd(request);

		Debug.Log("AD LOADED XXX");
	}

	private void Interstitial_OnAdClosed(object sender, System.EventArgs e)
	{
		if(GameManager.Instance.gameStatus == GameStatus.GameOver)
			GameManager.Instance.score = score;
	}
	
}
