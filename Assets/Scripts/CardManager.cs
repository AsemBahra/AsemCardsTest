using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.SceneManagement;

public class CardManager : MonoBehaviour
{
    public static CardManager Instance { get; private set; } // Singleton instance
    public CardGenerator cardGenerator;

    private List<Card> cards = new List<Card>();
    private List<Card> flippedCards = new List<Card>();
    private bool isCheckingMatch = false;

    private const string SaveFileName = "GameSave.json"; // Save file name

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Subscribe to UIManager event for grid selection
        UIManager.Instance.OnGridSelected += StartGame;
    }

    public void StartGame(int rows, int columns)
    {
        cardGenerator.rows = rows;
        cardGenerator.columns = columns;

        cards = cardGenerator.GenerateCards();

        // Notify UIManager to switch to the game UI
        UIManager.Instance.ShowGameUI();

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
            // Match found
            UIManager.Instance.UpdateScore(1);
            firstCard.SetMatched();
            secondCard.SetMatched();
            AudioManager.Instance.PlayMatchSound();

            yield return new WaitForSeconds(0.5f);

            firstCard.gameObject.SetActive(false);
            secondCard.gameObject.SetActive(false);

            if (CheckWinCondition())
            {
                UIManager.Instance.TriggerWinSequence();
                AudioManager.Instance.PlayGameOverSound();

                yield break;
            }
        }
        else
        {
            // No match
            yield return new WaitForSeconds(0.5f);
            AudioManager.Instance.PlayMismatchSound();

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
        isCheckingMatch = false;
    }

    private bool CheckWinCondition()
    {
        foreach (Card card in cards)
        {
            if (!card.IsMatched())
            {
                return false;
            }
        }
        return true;
    }

    public void SaveGame()
    {
        GameState gameState = new GameState
        {
            score = UIManager.Instance.GetScore(), // Save the current score
        };

        foreach (Card card in cards)
        {
            gameState.cards.Add(new CardState
            {
                cardID = card.cardID,
                isMatched = card.IsMatched(),
                isFlipped = card.IsFlipped()
            });
        }

        string json = JsonUtility.ToJson(gameState, true);
        File.WriteAllText(Path.Combine(Application.persistentDataPath, SaveFileName), json);

        Debug.Log($"Game saved to {Path.Combine(Application.persistentDataPath, SaveFileName)}");

        // Restart the scene after saving
        StartCoroutine(RestartScene());
    }

    public void LoadGame()
    {
        string path = Path.Combine(Application.persistentDataPath, SaveFileName);
        if (!File.Exists(path))
        {
            UIManager.Instance.ToggleWarning(true);
            Debug.LogWarning("No save file found!");
            return;
        }

        StartCoroutine(LoadGameRoutine());

        IEnumerator LoadGameRoutine()
        {
            string json = File.ReadAllText(path);
            GameState gameState = JsonUtility.FromJson<GameState>(json);

            // Restore the score
            
            UIManager.Instance.UpdateScore(gameState.score); // Refresh the score display

            // Clear existing cards and destroy their GameObjects
            foreach (Card card in cards)
            {
                Destroy(card.gameObject);
            }
            cards.Clear();
            flippedCards.Clear();

            // Generate cards based on the saved layout
            cards = cardGenerator.GenerateCardsFromSavedState(gameState.cards);

            // Notify UIManager to switch to the game UI
            UIManager.Instance.ShowGameUI();

            // Disable the GridLayoutGroup after the frame ends
            yield return cardGenerator.DisableGridLayoutAtEndOfFrame();

            // Restore card states
            for (int i = 0; i < cards.Count && i < gameState.cards.Count; i++)
            {
                Card card = cards[i];
                CardState cardState = gameState.cards[i];

                card.cardID = cardState.cardID; // Ensure cardID is restored

                if (cardState.isMatched)
                {
                    card.SetMatched();
                    card.gameObject.SetActive(false);
                }
                else if (cardState.isFlipped)
                {
                    card.FlipFaceUp();
                }
                else
                {
                    card.FlipFaceDown();
                }
            }
            StartCoroutine(FlipAllCardsAtStart());

            Debug.Log("Game loaded successfully!");
        }
    }

    private IEnumerator RestartScene()
    {
        yield return new WaitForSeconds(1f); // Optional delay for user experience
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}

[System.Serializable]
public class GameState
{
    public int score;
    public List<CardState> cards; // List of card states

    public GameState()
    {
        cards = new List<CardState>();
    }
}

[System.Serializable]
public class CardState
{
    public int cardID; // ID of the card
    public bool isMatched; // Whether the card is matched
    public bool isFlipped; // Whether the card is flipped
}
