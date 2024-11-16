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
        int requiredCards = rows * columns; // Total number of cards needed
        List<int> cardIDs = new List<int>();

        // Limit the number of card faces used
        int uniqueCards = Mathf.Min(cardFaces.Count, requiredCards / 2);

        // Create card pairs based on the unique cards needed
        for (int i = 0; i < uniqueCards; i++)
        {
            cardIDs.Add(i);
            cardIDs.Add(i);
        }

        // Shuffle the card IDs
        for (int i = 0; i < cardIDs.Count; i++)
        {
            int temp = cardIDs[i];
            int randomIndex = Random.Range(i, cardIDs.Count);
            cardIDs[i] = cardIDs[randomIndex];
            cardIDs[randomIndex] = temp;
        }

        // Create only the required number of cards
        for (int i = 0; i < requiredCards && i < cardIDs.Count; i++)
        {
            GameObject newCard = Instantiate(cardPrefab, cardGrid);
            Card card = newCard.GetComponent<Card>();
            card.SetCard(cardFaces[cardIDs[i]], cardIDs[i]);
            cards.Add(card);
        }

        // Adjust grid layout to balance spacing
        AdjustGridLayout();

        // Wait for Unity to complete layout calculations
        StartCoroutine(DisableGridLayoutAtEndOfFrame());

        return cards;
    }

    private void AdjustGridLayout()
    {
        GridLayoutGroup gridLayout = cardGrid.GetComponent<GridLayoutGroup>();

        RectTransform gridRect = cardGrid.GetComponent<RectTransform>();
        float cardGridWidth = gridRect.rect.width;
        float cardGridHeight = gridRect.rect.height;

        // Adjust cell size dynamically based on rows, columns, and spacing
        float spacing = 10; // Adjustable spacing between cards
        float padding = 20; // Adjustable padding around the grid

        gridLayout.spacing = new Vector2(spacing, spacing);
        gridLayout.padding = new RectOffset((int)padding, (int)padding, (int)padding, (int)padding);

        float cellWidth = (cardGridWidth - gridLayout.padding.left - gridLayout.padding.right - (columns - 1) * spacing) / columns;
        float cellHeight = (cardGridHeight - gridLayout.padding.top - gridLayout.padding.bottom - (rows - 1) * spacing) / rows;

        // Apply the calculated cell size
        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayout.constraintCount = columns;
        gridLayout.cellSize = new Vector2(cellWidth, cellHeight);
    }

    private IEnumerator DisableGridLayoutAtEndOfFrame()
    {
        yield return new WaitForEndOfFrame();
        
        GridLayoutGroup gridLayout = cardGrid.GetComponent<GridLayoutGroup>();
        if (gridLayout != null)
        {
            gridLayout.enabled = false;
        }
    }
}
