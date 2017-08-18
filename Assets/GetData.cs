using System;
using System.Net;
using OAuth;
using System.IO;
using UnityEngine;

namespace GetData
{
    class Program
    {
        static void Main(string[] args)
        {
            string consumerKey = "";
            string consumerSecret = "";
            var uri = new Uri("https://yboss.yahooapis.com/ysearch/web?callback=json2&q=flu");
            string url, param;
            var oAuth = new OAuthBase();
            var nonce = oAuth.GenerateNonce();
            var timeStamp = oAuth.GenerateTimeStamp();
            var signature = oAuth.GenerateSignature(uri, consumerKey,
            consumerSecret, string.Empty, string.Empty, "GET", timeStamp, nonce,
            OAuthBase.SignatureTypes.HMACSHA1.ToString(), out url, out param);

            //using (WebRequest.Create(string.Format("{0}?{1}&oauth_signature={2}",
            // url, param, signature)).GetResponse()) { }
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
    }
}