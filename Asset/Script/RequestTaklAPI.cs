using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;
using System.Collections;
using System.Text;

public class RequestTaklAPI : MonoBehaviour
{

    string url = "https://api.a3rt.recruit-tech.co.jp/talk/v1/smalltalk";
    string apikey = "API-Key";
    public string query = "";
    public Text text;
    public AudioSpeech audioSpeech;
    public void  MessageStart(string query)
    {
        this.query = query;
        StartCoroutine("StartMessage");
    }

    public IEnumerator StartMessage()
    {
        // ChatAPIに送る情報を入力
        WWWForm form = new WWWForm();
        form.AddField("apikey", apikey);
        form.AddField("query", query, Encoding.UTF8);

        // 通信
        using (UnityWebRequest request = UnityWebRequest.Post(url, form))
        {

            yield return request.Send();

            if (request.isError)
            {
                Debug.Log(request.error);
            }
            else
            {
                try
                {
                    // 取得したものをJsonで整形
                    string itemJson = request.downloadHandler.text;
                    JsonNode jsnode = JsonNode.Parse(itemJson);
                    // Jsonから会話部分だけ抽出してTextに代入
                    if (text.text != null)
                    {
                        text.text = jsnode["results"][0]["reply"].Get<string>();
                        audioSpeech.PlaySpeech(text.text);
                    }
                    Debug.Log(jsnode["results"][0]["reply"].Get<string>());
                }
                catch (Exception e)
                {
                    // エラーが出たらこれがログに吐き出される
                    Debug.Log("JsonNode:" + e.Message);
                }
            }
        }
        this.enabled = false;
    }
}

