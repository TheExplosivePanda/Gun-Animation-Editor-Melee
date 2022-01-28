﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Globalization;
using UnityEngine.EventSystems;

public class AnimationPreviewSpriteController : MonoBehaviour
{
    public Image DisplaySprite;
    private int index = 0;

    private static readonly CultureInfo culture = CultureInfo.CreateSpecificCulture("en-US");

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    
    private RectTransform rectTransform;
    [SerializeField]
    private Canvas canvas;
    [SerializeField]
    private RectTransform hand1;
    [SerializeField]
    private RectTransform hand2;
    [SerializeField]
    private RectTransform gungeoneer;

    private Vector2 posOffset = new Vector2(0, 0);
    public void UpdateSprite()
    {
        if (currentAnimation != null && currentFrame != null && currentFrame.sprite != null)
        {

            DisplaySprite.sprite = currentFrame.sprite;

            DisplaySprite.SetNativeSize();

            Vector2 anchoredPos = DisplaySprite.rectTransform.anchoredPosition;
            //currently centers sprite, should actully offset sprite based on gungeoneer pos.
            Vector2 pos = new Vector2(-DisplaySprite.sprite.rect.width / 2 * StaticRefrences.zoomScale, -DisplaySprite.sprite.rect.height / 2 * StaticRefrences.zoomScale) + posOffset;
            
            pos = new Vector2(Mathf.Round(pos.x / StaticRefrences.zoomScale) * StaticRefrences.zoomScale, Mathf.Round(pos.y / StaticRefrences.zoomScale) * StaticRefrences.zoomScale);
            pos = gungeoneer.anchoredPosition + new Vector2(currentFrame.offsetX*StaticRefrences.zoomScale, currentFrame.offsetY * StaticRefrences.zoomScale);
            DisplaySprite.rectTransform.anchoredPosition = pos;

            hand2.gameObject.SetActive(currentFrame.isTwoHanded);

            Vector2 handpos1 = new Vector2(pos.x + currentFrame.hand1PositionX * StaticRefrences.zoomScale, pos.y + currentFrame.hand1PositionY * StaticRefrences.zoomScale);
            Vector2 handpos2 = new Vector2(pos.x + currentFrame.hand2PositionX * StaticRefrences.zoomScale, pos.y + currentFrame.hand2PositionY * StaticRefrences.zoomScale);

            hand1.anchoredPosition = handpos1;
            hand2.anchoredPosition = handpos2;


            /*if (frameCounter != null)
            {
                frameCounter.text = (index + 1).ToString();
            }*/
        }
    }

    public void UpdateOffsets()
    {
        int zoom = StaticRefrences.zoomScale;
        currentFrame.offsetX = (hand1.anchoredPosition.x - gungeoneer.anchoredPosition.x) / zoom;
        currentFrame.offsetY = (hand1.anchoredPosition.y - gungeoneer.anchoredPosition.y) / zoom;
    }

    public void Close()
    {

    }

    IEnumerator frameCycleCoroutine;
    public void StartFrameCycle()
    {
        frameCycleCoroutine = CycleFrames();
        StartCoroutine(frameCycleCoroutine);
    }
    IEnumerator CycleFrames()
    {
        while(true)
        {
            yield return new WaitForSecondsRealtime(1f / 12f);
            NextFrame();
        }
    }


    public FrameInfo currentFrame
    {
        get
        {
            return currentAnimation.frames[index];
        }
    }
    public GaeAnimationInfo currentAnimation
    {
        get
        {
            return StaticRefrences.Instance.spriteController.currentAnimation;
        }
    }

    public void NextFrame()
    {
        if (currentAnimation != null)
        {
            MoveIndex(1);
        }
    }
    public void PreviousFrame()
    {

        if (currentAnimation != null)
        {
            MoveIndex(-1);
        }
    }
    private void MoveIndex(int i)
    {
        index += i;
        if (currentAnimation != null)
        {

            if (index < 0)
            {
                index = currentAnimation.frames.Length - 1;
            }
            else if (index >= currentAnimation.frames.Length)
            {
                index = 0;
            }
            UpdateSprite();
        }
    }
}