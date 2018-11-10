using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakeToSpeechSample : MonoBehaviour {
    public WWW www;
    private string text;
    public AudioClip clip;
    public bool clipFlag = false;
    public void SendClip(string text)
    {
        clipFlag = false;
        this.text = text;
        StartCoroutine("SendBinalyData");
    }

    IEnumerator SendBinalyData()
    {
        string apikey = "API-Key";
        string url = "https://api.apigw.smt.docomo.ne.jp/aiTalk/v1/textToSpeech?APIKEY=" + apikey;

        Dictionary<string, string> aiTalksParams = new Dictionary<string, string>();

        string postData = createSSML(text, aiTalksParams);
        byte[] data = System.Text.Encoding.UTF8.GetBytes(postData);

        Dictionary<string, string> headers = new Dictionary<string, string>();
        headers["Content-Type"] = "application/ssml+xml";
        headers["Accept"] = "audio/L16";
      //  headers["Content-Length"] = data.Length.ToString();
        www = new WWW(url, data, headers);
        yield return www;
        if (www.error != null)
        {
            Debug.LogError(www.error);
            yield break;
        }
        byte[] wavBytes = convertBytesEndian(www.bytes);
        AudioClipMaker audioClipMaker = new AudioClipMaker();
        clip = audioClipMaker.Create("name", wavBytes, 44, 16, www.bytes.Length/2, 1, 16000, false, false);
        clipFlag = true;
    }
    public string createSSML(string text, Dictionary<string, string> dic)
    {
        return "<?xml version=\"1.0\" encoding=\"utf-8\" ?><speak version=\"1.1\"><voice name=\"maki\"><prosody pitch=\"1.5\" rate=\"0.85\">" + text + " </prosody></voice></speak>";
    }
    private byte[] convertBytesEndian(byte[] bytes)
    {
        byte[] newBytes = new byte[bytes.Length];
        for (int i = 0; i < bytes.Length; i += 2)
        {
            newBytes[i] = bytes[i + 1];
            newBytes[i + 1] = bytes[i];
        }
        // 44byte付加したnewBytes
        newBytes = addWAVHeader(newBytes);
        return newBytes;
    }
    private byte[] addWAVHeader(byte[] bytes)
    {
        byte[] header = new byte[44];
        // サンプリングレート
        long longSampleRate = 16000;
        // チャンネル数
        int channels = 1;
        int bits = 16;
        // データ速度
        long byteRate = longSampleRate * (bits / 8) * channels;
        long dataLength = bytes.Length;
        long totalDataLen = dataLength + 36;
        // 最終的なWAVファイルのバイナリ
        byte[] finalWAVBytes = new byte[bytes.Length + header.Length];
        int typeSize = System.Runtime.InteropServices.Marshal.SizeOf(bytes.GetType().GetElementType());

        header[0] = convertByte("R");
        header[1] = convertByte("I");
        header[2] = convertByte("F");
        header[3] = convertByte("F");
        header[4] = (byte)(totalDataLen & 0xff);
        header[5] = (byte)((totalDataLen >> 8) & 0xff);
        header[6] = (byte)((totalDataLen >> 16) & 0xff);
        header[7] = (byte)((totalDataLen >> 24) & 0xff);
        header[8] = convertByte("W");
        header[9] = convertByte("A");
        header[10] = convertByte("V");
        header[11] = convertByte("E");
        header[12] = convertByte("f");
        header[13] = convertByte("m");
        header[14] = convertByte("t");
        header[15] = convertByte(" ");
        header[16] = 16;
        header[17] = 0;
        header[18] = 0;
        header[19] = 0;
        header[20] = 1;
        header[21] = 0;
        header[22] = (byte)channels;
        header[23] = 0;
        header[24] = (byte)(longSampleRate & 0xff);
        header[25] = (byte)((longSampleRate >> 8) & 0xff);
        header[26] = (byte)((longSampleRate >> 16) & 0xff);
        header[27] = (byte)((longSampleRate >> 24) & 0xff);
        header[28] = (byte)(byteRate & 0xff);
        header[29] = (byte)((byteRate >> 8) & 0xff);
        header[30] = (byte)((byteRate >> 16) & 0xff);
        header[31] = (byte)((byteRate >> 24) & 0xff);
        header[32] = (byte)((bits / 8) * channels);
        header[33] = 0;
        header[34] = (byte)bits;
        header[35] = 0;
        header[36] = convertByte("d");
        header[37] = convertByte("a");
        header[38] = convertByte("t");
        header[39] = convertByte("a");
        header[40] = (byte)(dataLength & 0xff);
        header[41] = (byte)((dataLength >> 8) & 0xff);
        header[42] = (byte)((dataLength >> 16) & 0xff);
        header[43] = (byte)((dataLength >> 24) & 0xff);

        System.Buffer.BlockCopy(header, 0, finalWAVBytes, 0, header.Length * typeSize);
        System.Buffer.BlockCopy(bytes, 0, finalWAVBytes, header.Length * typeSize, bytes.Length * typeSize);

        return finalWAVBytes;
    }

    private byte convertByte(string str)
    {
        return System.Text.Encoding.UTF8.GetBytes(str)[0];
    }
}
