using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    public CardGenerator cardGenerator; // Reference to the CardGenerator script
    public UIManager uiManager; // Reference to the UIManager script
    public TextMeshProUGUI scoreText; // UI element for the score

    private List<Card> cards = new List<Card>();
    private List<Card> flippedCards = new List<Card>();
    private int score = 0;
    private bool isCheckingMatch = false;

    void Start()
    {
        // Subscribe to the UIManager event for grid selection
        uiManager.OnGridSelected += StartGame;
    }

    void StartGame(int rows, int columns)
    {
        // Set rows and columns in CardGenerator
        cardGenerator.rows = rows;
        cardGenerator.columns = columns;

        // Generate cards
        cards = cardGenerator.GenerateCards();

        // Start the game logic
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
