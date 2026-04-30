using GoogleMobileAds.Api;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;
using UnityEngine;

public class AddMob : MonoBehaviour
{
    public static AddMob Instance;

    //gerçek reklam kimlikleri
    private string bannerId = "ca-app-pub-2008643567428334/2254159986";
    private string interstitialId = "ca-app-pub-2008643567428334/1818033950";
    private string rewardedId = "ca-app-pub-2008643567428334/2991514821";

    private BannerView bannerView;
    private InterstitialAd interstitialAd;
    public RewardedAd rewardedAd;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        MobileAds.Initialize(initStatus => {
            LoadBanner();
            LoadInterstitial();
            LoadRewarded();
        });

        
    }

    // ------------- BANNER -------------
    public void LoadBanner()
    {
        if (bannerView != null)
            bannerView.Destroy();

        bannerView = new BannerView(bannerId, AdSize.Banner, AdPosition.Bottom);
        bannerView.LoadAd(new AdRequest());
    }
    public void ShowBanner()
    {
        if (bannerView == null)
        {
            LoadBanner();
        }
        else
        {
            bannerView.Show();
        }
    }
    public void HideBanner()
    {
        if (bannerView != null)
        {
            bannerView.Hide();
        }
    }

    // ----------- INTERSTITIAL --------
    public void LoadInterstitial()
    {
        InterstitialAd.Load(interstitialId, new AdRequest(),
            (InterstitialAd ad, LoadAdError error) =>
            {
                if (error != null || ad == null)
                {
                    Debug.LogError("Interstitial yüklenemedi: " + error);
                    return;
                }

                interstitialAd = ad;
            });
    }

    public void ShowInterstitial()
    {
        if (interstitialAd != null)
        {
            interstitialAd.Show();
            LoadInterstitial();
        }
    }

    // ------------ REWARDED ------------
    public void LoadRewarded()
    {
        RewardedAd.Load(rewardedId, new AdRequest(),
            (RewardedAd ad, LoadAdError error) =>
            {
                if (error != null || ad == null)
                {
                    Debug.LogError("Rewarded yüklenemedi: " + error);
                    return;
                }

                rewardedAd = ad;
            });
    }

    public void ShowRewarded()
    {
        if (rewardedAd != null)
        {
            rewardedAd.Show((Reward reward) =>
            {
                Debug.Log("Ödül kazandý!");
            });

            LoadRewarded();
        }
    }
}
