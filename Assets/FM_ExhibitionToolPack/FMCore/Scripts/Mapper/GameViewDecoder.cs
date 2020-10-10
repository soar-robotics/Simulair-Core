﻿using System.Collections;
using UnityEngine;
using System;
using UnityEngine.UI;

public class GameViewDecoder : MonoBehaviour
{
    public bool FastMode = false;
    public bool AsyncMode = false;

    [Range(0f,10f)]
    public float DecoderDelay = 0f;
    private float DecoderDelay_old = 0f;

    public Texture2D ReceivedTexture;
    public GameObject TestQuad;
    public RawImage TestImg;

    public UnityEventTexture2D OnReceivedTexture2D;

    // Use this for initialization
    void Start()
    {
        Application.runInBackground = true;
    }

    bool ReadyToGetFrame = true;

    //[Header("Pair Encoder & Decoder")]
    public int label = 1001;
	int dataID = 0;
    //int maxID = 1024;
    int dataLength = 0;
	int receivedLength = 0;

    byte[] dataByte;
    public bool GZipMode = false;

    public void Action_ProcessImageData(byte[] _byteData)
	{
        if (!enabled) return;
        if (_byteData.Length <= 8) return;

        int _label = BitConverter.ToInt32(_byteData, 0);
        if (_label != label) return;
        int _dataID = BitConverter.ToInt32(_byteData, 4);

        if (_dataID != dataID) receivedLength = 0;
        dataID = _dataID;
        dataLength = BitConverter.ToInt32(_byteData, 8);
        int _offset = BitConverter.ToInt32(_byteData, 12);

        GZipMode = _byteData[16] == 1;

        if (receivedLength == 0) dataByte = new byte[dataLength];
        receivedLength += _byteData.Length - 17;
        Buffer.BlockCopy(_byteData, 17, dataByte, _offset, _byteData.Length - 17);

        if (ReadyToGetFrame)
        {
            if (receivedLength == dataLength)
            {
                if (DecoderDelay_old != DecoderDelay)
                {
                    StopAllCoroutines();
                    DecoderDelay_old = DecoderDelay;
                }

                StartCoroutine(ProcessImageData(dataByte));
            }
        }
    }

    IEnumerator ProcessImageData(byte[] _byteData)
	{
        yield return new WaitForSeconds(DecoderDelay);
        ReadyToGetFrame = false;

        if (GZipMode) _byteData = _byteData.FMUnzipBytes();

#if UNITY_IOS && !UNITY_EDITOR
            FastMode = true;
#endif
#if UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN || UNITY_EDITOR_OSX || UNITY_EDITOR_WIN || UNITY_IOS || UNITY_ANDROID || WINDOWS_UWP
        if (FastMode)
        {
            //try AsyncMode, on supported platform
            if (AsyncMode && Loom.numThreads < Loom.maxThreads)
            {
                //has spare thread
                bool AsyncDecoding = true;
                byte[] RawTextureData = new byte[1];
                int _width = 0;
                int _height = 0;
                Loom.RunAsync(() =>
                {
                    _byteData.FMJPGToRawTextureData(ref RawTextureData, ref _width, ref _height);
                    AsyncDecoding = false;
                });
                while (AsyncDecoding) yield return null;
                try
                {
                    //check resolution
                    ReceivedTexture.FMMatchResolution(ref ReceivedTexture, _width, _height);
                    ReceivedTexture.LoadRawTextureData(RawTextureData);
                    ReceivedTexture.Apply();
                }
                catch (Exception e)
                {
                    Destroy(ReceivedTexture);
                    ReadyToGetFrame = true;
                    yield break;
                }
            }
            else
            {
                //no spare thread, run in main thread
                try
                {
                    ReceivedTexture.FMLoadJPG(ref ReceivedTexture, _byteData);
                }
                catch (Exception e)
                {
                    Destroy(ReceivedTexture);
                    ReadyToGetFrame = true;
                    yield break;
                }
            }
        }
        else
        {
            if (ReceivedTexture == null) ReceivedTexture = new Texture2D(0, 0);
            ReceivedTexture.LoadImage(_byteData);
        }
#else
            if (ReceivedTexture == null) ReceivedTexture = new Texture2D(0, 0);
            ReceivedTexture.LoadImage(_byteData);
#endif
        if (ReceivedTexture.width <= 8)
        {
            //throw new Exception("texture is smaller than 8 x 8, wrong data");
            Debug.LogError("texture is smaller than 8 x 8, wrong data");
            ReadyToGetFrame = true;
            yield break;
        }

        if (TestQuad != null) TestQuad.GetComponent<Renderer>().material.mainTexture = ReceivedTexture;
        if (TestImg != null) TestImg.texture = ReceivedTexture;
        OnReceivedTexture2D.Invoke(ReceivedTexture);

        ReadyToGetFrame = true;

		yield return null;
	}

    private void OnDisable()
    {
        StopAllCoroutines();
    }
}
