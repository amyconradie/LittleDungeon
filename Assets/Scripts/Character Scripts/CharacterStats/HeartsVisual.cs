using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeartsVisual : MonoBehaviour
{

    private int START_HEARTS_AMOUNT, MAX_HEART_AMOUNT;
    private float heartSpacingAmount = 2.0f;
    private float xStart;

    public int currentHeartSegments;

    [SerializeField] private Sprite heartSprite_empty;
    [SerializeField] private Sprite heartSprite_half;
    [SerializeField] private Sprite heartSprite_full;


    private List<HeartImage> heartImageList;
    public HeartSystem heartSystem;



    private void Awake()
    {
        heartImageList = new List<HeartImage>();
    }

    private void Update()
    {
        if (heartSystem!= null)
        {
            currentHeartSegments = heartSystem.GetCurrentHealth();

        }
    }

    public void SetUpHeartsVisual(int heartAmount, int maxHearts)
    {
        HeartSystem heartSystem = new HeartSystem(heartAmount, maxHearts);
        SetHeartSystem(heartSystem);
    }


    public void SetHeartSystem(HeartSystem heartSystem)
    {
        this.heartSystem = heartSystem;

        List<HeartSystem.Heart> heartList = heartSystem.GetHeartList();

        
        xStart = heartList.Count - 1.0f;
        Vector2 heartAnchoredPosition = new Vector2(xStart, 0.5f);

        for (int i = 0; i < heartList.Count; i++)
        {
            HeartSystem.Heart heart = heartList[i];
            CreateHeartImage(heartAnchoredPosition).SetHeartFragments(heart.GetFragmentAmount());
            heartAnchoredPosition -= new Vector2(heartSpacingAmount, 0);
        }

        // event listeners
        heartSystem.OnDamaged += HeartSystem_OnDamaged;
        heartSystem.OnHealed += HeartSystem_OnHealed;
        heartSystem.OnDead += HeartSystem_OnDead;

    }

    private void AddHeart()
    {
        //retrieve a copy of the list of hearts in the heart system
        List<HeartSystem.Heart> heartList = this.heartSystem.GetHeartList();

        if (heartList.Count == MAX_HEART_AMOUNT)
        {
            // adda heart to the heart system
            HeartSystem.Heart heart = this.heartSystem.AddHeart(this.heartSystem.heartList);
            
            //get appropriate position
            Vector2 heartAnchoredPosition = new Vector2(xStart + heartList.Count * heartSpacingAmount, 0.5f);

            CreateHeartImage(heartAnchoredPosition).SetHeartFragments(heart.GetFragmentAmount());
        }
    }

    private void HeartSystem_OnDamaged(object sender, System.EventArgs e)
    {
        // Heart system was damaged
        RefreshAllHearts();
    }

    private void HeartSystem_OnHealed(object sender, System.EventArgs e)
    {
        // Heart System was healed
        RefreshAllHearts();
    }


    private void HeartSystem_OnDead(object sender, System.EventArgs e)
    {
        // Heart System is Dead

        // do something?
    }


    private void RefreshAllHearts()
    {
        List<HeartSystem.Heart> heartList = heartSystem.GetHeartList();

        for (int i = 0; i < heartImageList.Count; i++)
        {
            HeartImage heartImage = heartImageList[i];
            HeartSystem.Heart heart = heartList[i];
            heartImage.SetHeartFragments(heart.GetFragmentAmount());
        }
    }


    private HeartImage CreateHeartImage(Vector2 anchoredPosition)
    {
        // Create Game Object
        GameObject heartGameObject = new GameObject("Heart", typeof(Image));

        // Set as child of this transformation
        heartGameObject.transform.SetParent(transform, false);

        // Locate and Size Game Object
        heartGameObject.GetComponent<RectTransform>().anchoredPosition = anchoredPosition;
        heartGameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(2, 2);

        // Set heart sprite
        Image heartImageUI = heartGameObject.GetComponent<Image>();
        heartImageUI.sprite = heartSprite_full;

        HeartImage heartImage = new HeartImage(this, heartImageUI);

        heartImageList.Add(heartImage);

        return heartImage;
    }


    // single heart
    public class HeartImage
    {

        private Image heartImage;
        private HeartsVisual heartsVisual;

        public HeartImage(HeartsVisual heartsVisual, Image heartImage)
        {
            this.heartsVisual = heartsVisual;
            this.heartImage = heartImage;
        }

        public void SetHeartFragments(int fragments)
        {
            switch (fragments)
            {
                case 0:
                    heartImage.sprite = heartsVisual.heartSprite_empty;
                    break;
                case 1:
                    heartImage.sprite = heartsVisual.heartSprite_half;
                    break;
                case 2:
                    heartImage.sprite = heartsVisual.heartSprite_full;
                    break;
            }
        }

    }

}
