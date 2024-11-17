using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardGenerator : MonoBehaviour
{
    public List<Sprite> cardFaces; // List of available card face images
    public GameObject cardPrefab; // Card prefab
    public Transform cardGrid; // Parent grid for the cards

    public int rows = 4; // Number of rows for the grid
    public int columns = 4; // Number of columns for the grid

    private List<Card> cards = new List<Card>();

    public List<Card> GenerateCards()
    {
        List<Card> newCards = new List<Card>();

        // Clear existing children in the grid
        foreach (Transform child in cardGrid)
        {
            Destroy(child.gameObject);
        }

        // Adjust grid layout
        AdjustGridLayout(rows * columns);

        // Generate card IDs
        List<int> cardIDs = new List<int>();
        for (int i = 0; i < rows * columns / 2; i++)
        {
            cardIDs.Add(i);
            cardIDs.Add(i); // Add each ID twice for matching pairs
        }

        // Shuffle card IDs
        for (int i = 0; i < cardIDs.Count; i++)
        {
            int temp = cardIDs[i];
            int randomIndex = UnityEngine.Random.Range(i, cardIDs.Count);
            cardIDs[i] = cardIDs[randomIndex];
            cardIDs[randomIndex] = temp;
        }

        // Instantiate cards
        foreach (int id in cardIDs)
        {
            GameObject newCard = Instantiate(cardPrefab, cardGrid);
            Card card = newCard.GetComponent<Card>();
            card.SetCard(cardFaces[id], id);
            newCards.Add(card);
        }

        StartCoroutine(DisableGridLayoutAtEndOfFrame());
        return newCards;
    }

    
    private void AdjustGridLayout(int cardCount)
    {
        GridLayoutGroup gridLayout = cardGrid.GetComponent<GridLayoutGroup>();

        int columns = Mathf.CeilToInt(Mathf.Sqrt(cardCount));
        int rows = Mathf.CeilToInt((float)cardCount / columns);

        RectTransform gridRect = cardGrid.GetComponent<RectTransform>();
        float cardGridWidth = gridRect.rect.width;
        float cardGridHeight = gridRect.rect.height;

        gridLayout.spacing = new Vector2(10, 10);
        gridLayout.padding = new RectOffset(10, 10, 10, 10);

        float cellWidth = (cardGridWidth - gridLayout.padding.left - gridLayout.padding.right - (columns - 1) * gridLayout.spacing.x) / columns;
        float cellHeight = (cardGridHeight - gridLayout.padding.top - gridLayout.padding.bottom - (rows - 1) * gridLayout.spacing.y) / rows;

        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayout.constraintCount = columns;
        gridLayout.cellSize = new Vector2(cellWidth, cellHeight);
    }
    public IEnumerator DisableGridLayoutAtEndOfFrame()
    {
        yield return new WaitForEndOfFrame();

        
        GridLayoutGroup gridLayout = cardGrid.GetComponent<GridLayoutGroup>();
        if (gridLayout != null)
        {
            gridLayout.enabled = false;
        }
    }
    public List<Card> GenerateCardsFromSavedState(List<CardState> savedCardStates)
    {
        List<Card> newCards = new List<Card>();

        // Clear existing children in the grid
        foreach (Transform child in cardGrid)
        {
            Destroy(child.gameObject);
        }

        // Adjust grid layout
        AdjustGridLayout(savedCardStates.Count);

        // Generate cards based on saved states
        foreach (var cardState in savedCardStates)
        {
            GameObject newCard = Instantiate(cardPrefab, cardGrid);
            Card card = newCard.GetComponent<Card>();
            card.SetCard(cardFaces[cardState.cardID], cardState.cardID);
            newCards.Add(card);
        }

        return newCards;
    }

}
