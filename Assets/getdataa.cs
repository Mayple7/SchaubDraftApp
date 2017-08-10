using System.Collections;
using System.Collections.Generic;
using System;
using System.Net;
using System.Net.Security;
using OAuth;
using System.IO;
using UnityEngine;

public class getdataa : MonoBehaviour {

	// Use this for initialization
	void Start () {
        string consumerKey = "";
        string consumerSecret = "";
        //var uri = new Uri("https://yboss.yahooapis.com/ysearch/web?callback=json2&q=flu");
        var uri = new Uri("https://fantasysports.yahooapis.com/fantasy/v2/users;use_login=1/games;game_keys=nfl/teams");
        string url, param;
        var oAuth = new OAuthBase();
        var nonce = oAuth.GenerateNonce();
        var timeStamp = oAuth.GenerateTimeStamp();
        var signature = oAuth.GenerateSignature(uri, consumerKey,
        consumerSecret, string.Empty, string.Empty, "GET", timeStamp, nonce,
        OAuthBase.SignatureTypes.HMACSHA1.ToString(), out url, out param);

        //  using (WebRequest.Create(string.Format("{0}?{1}&oauth_signature={2}",
        //  url, param, signature)).GetResponse()) { }
        ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(delegate { return true; });
        var data = String.Format("{0}?{1}&oauth_signature={2}", url, param, signature);
        var requestParametersUrl = String.Format("{0}?{1}&oauth_signature={2}", url, param, signature);
        var request = WebRequest.Create(requestParametersUrl);
        using (var response = request.GetResponse())
        using (Stream dataStream = response.GetResponseStream())
        using (StreamReader reader = new StreamReader(dataStream))
        {
            data = reader.ReadToEnd();
            Debug.Log(data);
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
