using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    public CardGenerator cardGenerator; // Reference to the CardGenerator script

    private List<Card> cards = new List<Card>();
    private List<Card> flippedCards = new List<Card>();
    private bool isCheckingMatch = false;

    void Start()
    {
        // Subscribe to the UIManager event for grid selection
        UIManager.Instance.OnGridSelected += StartGame;
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
            // Notify UIManager to update score
            UIManager.Instance.UpdateScore(1);

            firstCard.SetMatched();
            secondCard.SetMatched();

            yield return new WaitForSeconds(0.5f);
            firstCard.gameObject.SetActive(false);
            secondCard.gameObject.SetActive(false);

            // Check if all cards are matched
            if (CheckWinCondition())
            {
                UIManager.Instance.TriggerWinSequence();
                yield break;
            }
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

    private bool CheckWinCondition()
    {
        // All cards are matched if no active cards remain
        foreach (Card card in cards)
        {
            if (!card.IsMatched())
            {
                return false;
            }
        }
        return true;
    }
}
