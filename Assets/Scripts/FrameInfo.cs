﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEditor;

/// <summary>
/// defines a single frame in the animation. all measurements are in whole pixels. data should best represent the program's needs.
/// contains some addition data such as the frame sprite
/// </summary>
public class FrameInfo
{

    public FrameInfo(Texture2D texture , string path, GaeAnimationInfo animationInfo)
    {
        this.animationInfo = animationInfo;
        this.texture = texture;
        this.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), 1);
        this.path = path;
        this.hand1PositionX = 0;
        this.hand1PositionY = 0;
        this.hand2PositionX = 0;
        this.hand2PositionY = 0;
        this.offsetX = 0;
        this.offsetY = 0;
        this.isTwoHanded = false;
    }
    public FrameInfo(Texture2D texture, Sprite sprite, string path, GaeAnimationInfo animationInfo)
    {
        this.animationInfo = animationInfo;
        this.texture = texture;
        this.sprite = sprite;
        this.path = path;
        this.hand1PositionX = 0;
        this.hand1PositionY = 0;
        this.hand2PositionX = 0;
        this.hand2PositionY = 0;
        this.offsetX = 0;
        this.offsetY = 0;
        this.isTwoHanded = false;
       
    }
    public FrameInfo(Texture2D texture, Sprite sprite, float hand1X, float hand1Y, float hand2X, float hand2Y, float offsetX, float offsetY, bool isTwoHanded,string path , GaeAnimationInfo animationInfo)
    {
        this.animationInfo = animationInfo;
        this.texture = texture;
        this.sprite = sprite;
        this.hand1PositionX = hand1X;
        this.hand1PositionY = hand1Y;
        this.hand2PositionX = hand2X;
        this.hand2PositionY = hand2Y;
        this.offsetX = offsetX;
        this.offsetY = offsetY;
        this.isTwoHanded = isTwoHanded;
        this.path = path;
    }
    public string path;
    public Texture2D texture;
    public Sprite sprite;
    public float offsetX;
    public float offsetY;
    public float hand1PositionX;
    public float hand1PositionY;
    public float hand2PositionX;
    public float hand2PositionY;
    public float muzzleflashPositionX = 0;
    public float muzzleflashPositionY = 0;
    public bool isTwoHanded
    {
        get
        {
            return animationInfo.IsTwoHanded;
        }
        set 
        {
            animationInfo.IsTwoHanded = value;
        }
    }


    public GaeAnimationInfo animationInfo;
}

