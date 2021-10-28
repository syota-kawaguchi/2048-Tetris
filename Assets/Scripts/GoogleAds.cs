using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;

public class GoogleAds : MonoBehaviour
{
    //string appId = "";
    string adUnitId = "ca-app-pub-5728922507498585/7667680068";

    int adsSizeX = 540;
    float adsAspect = 50 / 320;

    void Start()
    {
        MobileAds.Initialize(InitializationStatus => {});
        RequestBanner();
    }

    private void RequestBanner() {
        var adsSize = new AdSize(adsSizeX, Mathf.RoundToInt(adsSizeX * adsAspect));

        var bannerView = new BannerView(adUnitId, AdSize.Banner, AdPosition.Bottom);

        var request = new AdRequest.Builder().Build();

        bannerView.LoadAd(request);
    }
}
