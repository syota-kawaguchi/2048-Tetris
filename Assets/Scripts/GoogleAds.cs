using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;

public class GoogleAds : MonoBehaviour
{
    string adUnitId = "ca-app-pub-5728922507498585/7667680068";

    void Start()
    {
        MobileAds.Initialize(InitializationStatus => {});
        RequestBanner();
    }

    private void RequestBanner() {
        var bannerView = new BannerView(adUnitId, AdSize.Banner, AdPosition.Bottom);

        var request = new AdRequest.Builder().Build();

        bannerView.LoadAd(request);
    }
}
