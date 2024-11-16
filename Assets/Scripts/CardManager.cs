using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    public CardGenerator cardGenerator; // Reference to the CardGenerator script
    public TextMeshProUGUI scoreText;

    private List<Card> cards = new List<Card>();
    private List<Card> flippedCards = new List<Card>();
    private int score = 0;
    private bool isCheckingMatch = false;

    void Start()
    {
        // Generate cards using the CardGenerator
        cards = cardGenerator.GenerateCards();
        StartCoroutine(FlipAllCardsAtStart());
    }

    private IEnumerator FlipAllCardsAtStart()
    {
        foreach (Card card in cards)
        {
            card.FlipFaceUp();
        }
        yield return new WaitForSeconds(3f);
        foreach (Card card in cards)
        {
            card.FlipFaceDown();
        }
    }

    public void AddFlippedCard(Card card)
    {
        if (flippedCards.Contains(card) || card.IsFlipped() || card.IsMatched())
            return;

        flippedCards.Add(card);

        if (flippedCards.Count == 2 && !isCheckingMatch)
        {
            StartCoroutine(CheckMatch());
        }
    }

    private IEnumerator CheckMatch()
    {
        isCheckingMatch = true;

        yield return new WaitForSeconds(0.5f);

        Card firstCard = flippedCards[0];
        Card secondCard = flippedCards[1];

        if (firstCard.cardID == secondCard.cardID)
        {
            score++;
            scoreText.text = "Score: " + score;

            firstCard.SetMatched();
            secondCard.SetMatched();

            yield return new WaitForSeconds(0.5f);
            firstCard.gameObject.SetActive(false);
            secondCard.gameObject.SetActive(false);
        }
        else
        {
            yield return new WaitForSeconds(0.5f);
            firstCard.FlipFaceDown();
            secondCard.FlipFaceDown();
        }

        flippedCards.RemoveAt(0);
        flippedCards.RemoveAt(0);

        isCheckingMatch = false;

        if (flippedCards.Count >= 2)
        {
            StartCoroutine(CheckMatch());
        }
    }

    public bool IsCheckingMatch()
    {
        return isCheckingMatch;
    }
}
