using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardGenerator : MonoBehaviour
{
    public List<Sprite> cardFaces;
    public GameObject cardPrefab;
    public Transform cardGrid;

    private List<Card> cards = new List<Card>();

    public List<Card> GenerateCards()
    {
        List<int> cardIDs = new List<int>();

        // Generate unique card IDs
        for (int i = 0; i < cardFaces.Count; i++)
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

        // Create card instances
        foreach (int id in cardIDs)
        {
            GameObject newCard = Instantiate(cardPrefab, cardGrid);
            Card card = newCard.GetComponent<Card>();
            card.SetCard(cardFaces[id], id);
            cards.Add(card);
        }

        // Adjust grid layout
        AdjustGridLayout(cardIDs.Count);

        // Wait for Unity to complete layout calculations
        StartCoroutine(DisableGridLayoutAtEndOfFrame());

        return cards;
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
