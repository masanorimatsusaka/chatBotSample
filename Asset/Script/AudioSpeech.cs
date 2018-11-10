using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VRM;

public class AudioSpeech : MonoBehaviour {
      TakeToSpeechSample takeToSpeech;
    AnimationClip clip;
    private VRMBlendShapeProxy proxy;
    private Animator animatior;
    
    private float laughterThreshold = 0.5f;
    private float laughterMultiplier = 1.5f;
    private int laughterBlendTarget = OVRLipSync.VisemeCount;
    private OVRLipSyncContextBase lipsyncContext;
    private AudioSource audioSource;
    // Use this for initialization
    void Start () {
        takeToSpeech = this.GetComponent<TakeToSpeechSample>();
        lipsyncContext = this.GetComponent<OVRLipSyncContextBase>();
        lipsyncContext.Smoothing = 70;
        animatior = this.GetComponent<Animator>();
        audioSource = this.GetComponent<AudioSource>();
        proxy = this.GetComponent<VRMBlendShapeProxy>();
    }
	public void PlaySpeech(string text)
    {
        takeToSpeech.SendClip(text);
    }
    private void Update()
    {
        if (takeToSpeech.clipFlag)
        {
            takeToSpeech.clipFlag = false;
            float[] rawData = new float[takeToSpeech.www.bytes.Length / 2 * takeToSpeech.clip.channels];
            takeToSpeech.clip.GetData(rawData, 0);
            audioSource.clip = takeToSpeech.clip;
            audioSource.Play();
            int i = Random.Range(0, 3);
            animatior.SetBool("action" + i, true);
        }
        OVRLipSync.Frame frame = lipsyncContext.GetCurrentPhonemeFrame();
        if (frame != null)
        {
            SetVisemeToMorphTarget(frame);
        }
    }
    /// <summary>
    /// VRMBlendShapeProxy に対応させる
    /// </summary>
    void SetVisemeToMorphTarget(OVRLipSync.Frame frame)
    {
        int LipType = 0;
        float LipValue = 0.0f;
        float Value;
        // 0 は「無音時に1を返す処理」の為、iに0を含めない
        for (int i = 1; i < 15; i++)
        {
            Value = frame.Visemes[i];
            //1番大きい値の時にLipTypeを更新
            if (LipValue < Value && i != 0)
            {
                LipValue = Value;
                LipType = i;
            }
        }
        switch (LipType)
        {
            case 10:
                proxy.SetValue(BlendShapePreset.A, LipValue);
                break;
            case 12:
                proxy.SetValue(BlendShapePreset.I, LipValue);
                break;
            case 14:
                proxy.SetValue(BlendShapePreset.U, LipValue);
                break;
            case 11:
                proxy.SetValue(BlendShapePreset.E, LipValue);
                break;
            case 13:
                proxy.SetValue(BlendShapePreset.O, LipValue);
                break;
            default:
                proxy.SetValue(BlendShapePreset.A, 0);
                proxy.SetValue(BlendShapePreset.I, 0);
                proxy.SetValue(BlendShapePreset.U, 0);
                proxy.SetValue(BlendShapePreset.E, 0);
                proxy.SetValue(BlendShapePreset.O, 0);
                break;
        }
    }
}
