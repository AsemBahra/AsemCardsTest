using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // Use TextMeshProUGUI for UI text
using UnityEngine.UI;
using DG.Tweening; // Import DOTween for animations

public class Card : MonoBehaviour
{
    // Existing variables
    public int cardID;
    public Sprite cardBack;
    public Sprite cardFace;
    private Image cardImage;
    private bool isFlipped = false;
    private bool isMatched = false;

    private CardManager cardManager;

    void Start()
    {
        cardImage = GetComponent<Image>();
        cardImage.sprite = cardBack;
        cardManager = FindObjectOfType<CardManager>();
    }

    public void OnCardClicked()
    {
        if (isFlipped || isMatched) return; // Prevent flipping if already flipped or matched
        FlipCardAnimation();
        AudioManager.Instance.PlayFlipSound();
        cardManager.AddFlippedCard(this);
    }

    public void SetMatched()
    {
        isMatched = true;
    }

    public bool IsMatched()
    {
        return isMatched;
    }

    public bool IsFlipped()
    {
        return isFlipped;
    }

    public void FlipCardAnimation()
    {
        transform.DORotate(new Vector3(0, 90, 0), 0.2f).OnComplete(() =>
        {
            cardImage.sprite = isFlipped ? cardBack : cardFace;
            transform.DORotate(new Vector3(0, 0, 0), 0.2f);
            isFlipped = !isFlipped;
        });
    }

    public void SetCard(Sprite face, int id)
    {
        cardFace = face;
        cardID = id;
    }

    public void FlipFaceUp()
    {
        if (!isFlipped)
        {
            FlipCardAnimation();
        }
    }

    public void FlipFaceDown()
    {
        if (isFlipped && !isMatched)
        {
            FlipCardAnimation();
        }
    }
}
