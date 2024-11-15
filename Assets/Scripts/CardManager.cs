using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardManager : MonoBehaviour
{
    public List<Sprite> cardFaces;
    public GameObject cardPrefab;
    public Transform cardGrid;
    private List<Card> cards = new List<Card>();
    private List<Card> flippedCards = new List<Card>();
    public TextMeshProUGUI scoreText;
    private int score = 0;
    private bool isCheckingMatch = false;

    void Start()
    {
        GenerateCards();
        StartCoroutine(FlipAllCardsAtStart());
    }

    void GenerateCards()
    {
        List<int> cardIDs = new List<int>();
        for (int i = 0; i < cardFaces.Count; i++)
        {
            cardIDs.Add(i);
            cardIDs.Add(i);
        }

        for (int i = 0; i < cardIDs.Count; i++)
        {
            int temp = cardIDs[i];
            int randomIndex = Random.Range(i, cardIDs.Count);
            cardIDs[i] = cardIDs[randomIndex];
            cardIDs[randomIndex] = temp;
        }

        foreach (int id in cardIDs)
        {
            GameObject newCard = Instantiate(cardPrefab, cardGrid);
            Card card = newCard.GetComponent<Card>();
            card.SetCard(cardFaces[id], id);
            cards.Add(card);
        }

        AdjustGridLayout(cardIDs.Count);

    }

    IEnumerator FlipAllCardsAtStart()
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

    void AdjustGridLayout(int cardCount)
    {
        GridLayoutGroup gridLayout = cardGrid.GetComponent<GridLayoutGroup>();

        // Calculate number of columns and rows for a balanced grid
        int columns = Mathf.CeilToInt(Mathf.Sqrt(cardCount));
        int rows = Mathf.CeilToInt((float)cardCount / columns);

        // Get the size of the card grid container
        RectTransform gridRect = cardGrid.GetComponent<RectTransform>();
        float cardGridWidth = gridRect.rect.width;
        float cardGridHeight = gridRect.rect.height;

        // Set spacing and padding (adjust values as needed)
        gridLayout.spacing = new Vector2(10, 10);  // Spacing between cards
        gridLayout.padding = new RectOffset(10, 10, 10, 10);  // Padding around grid

        // Calculate cell size based on the container size, padding, and spacing
        float cellWidth = (cardGridWidth - gridLayout.padding.left - gridLayout.padding.right - (columns - 1) * gridLayout.spacing.x) / columns;
        float cellHeight = (cardGridHeight - gridLayout.padding.top - gridLayout.padding.bottom - (rows - 1) * gridLayout.spacing.y) / rows;

        // Apply calculated values
        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayout.constraintCount = columns;
        gridLayout.cellSize = new Vector2(cellWidth, cellHeight);

    }



    public void AddFlippedCard(Card card)
    {
        // Avoid adding cards that are already flipped or matched
        if (flippedCards.Contains(card) || card.IsFlipped() || card.IsMatched())
            return;

        flippedCards.Add(card);

        // Trigger match check only if there are exactly two unmatched cards
        if (flippedCards.Count == 2 && !isCheckingMatch)
        {
            StartCoroutine(CheckMatch());
        }
    }

    IEnumerator CheckMatch()
    {
        isCheckingMatch = true;

        // Wait a short time to allow the user to see the second flipped card
        yield return new WaitForSeconds(0.5f);

        Card firstCard = flippedCards[0];
        Card secondCard = flippedCards[1];
        Debug.Log(firstCard.cardID + " - " + secondCard.cardID);
        if (firstCard.cardID == secondCard.cardID)
        {
            // Match found - mark as matched and update score
            score++;
            scoreText.text = "Score: " + score;
            firstCard.SetMatched();
            secondCard.SetMatched();

            // Deactivate matched cards after a short delay
            yield return new WaitForSeconds(0.5f);
            firstCard.gameObject.SetActive(false);
            secondCard.gameObject.SetActive(false);
        }
        else
        {
            // No match - flip both cards back
            yield return new WaitForSeconds(0.5f);
            firstCard.FlipFaceDown();
            secondCard.FlipFaceDown();
        }

        // Clear the first two cards from the flipped list after processing
        flippedCards.RemoveAt(0);
        flippedCards.RemoveAt(0);

        isCheckingMatch = false;

        // If more cards are still flipped, check the next pair
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