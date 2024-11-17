using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; } // Singleton instance

    public GameObject gridSelectionPanel; // Panel for selecting the grid
    public GameObject gameUI; // Main game UI
    public TMP_Dropdown gridSizeDropdown; // TMP Dropdown for grid size selection
    public Button startButton; // Button to start the game
    public Button saveButton; // Button to save the game
    public Button loadButton; // Button to load the game
    public GameObject winningPanel; // The panel displayed when the player wins
    public TextMeshProUGUI scoreText; // UI element for displaying the score
    public GameObject warningText; // Element for warnning  text
    public float restartDelay = 3f; // Delay before restarting the scene

    public event Action<int, int> OnGridSelected; // Event to notify the selected grid size

    private Dictionary<string, Vector2Int> gridOptions = new Dictionary<string, Vector2Int>
    {
        { "2x2", new Vector2Int(2, 2) },
        { "2x3", new Vector2Int(2, 3) },
        { "3x3", new Vector2Int(3, 3) },
        { "3x4", new Vector2Int(3, 4) },
        { "4x5", new Vector2Int(4, 5) }
    };

    private int score = 0; // Internal score tracking

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
        // Populate the dropdown
        PopulateDropdown();

        // Set default UI state
        ShowGridSelection();

        // Hide the winning panel initially
        winningPanel.transform.localScale = Vector3.zero;
        winningPanel.SetActive(false);

        // Hook up button listeners
        startButton.onClick.AddListener(OnStartButtonClicked);
        saveButton.onClick.AddListener(() => CardManager.Instance.SaveGame());
        loadButton.onClick.AddListener(() => CardManager.Instance.LoadGame());
    }

    private void PopulateDropdown()
    {
        gridSizeDropdown.ClearOptions();

        // Populate TMP_Dropdown with grid options
        var options = new List<string>(gridOptions.Keys);
        gridSizeDropdown.AddOptions(options);

        // Set default selection
        gridSizeDropdown.value = 0;
    }

    private void OnStartButtonClicked()
    {
        // Get the selected grid size
        string selectedOption = gridSizeDropdown.options[gridSizeDropdown.value].text;
        Vector2Int gridSize = gridOptions[selectedOption];

        // Notify CardManager
        OnGridSelected?.Invoke(gridSize.x, gridSize.y);

        // Show the game UI
        ShowGameUI();
    }

    public void ShowGameUI()
    {
        gridSelectionPanel.SetActive(false);
        gameUI.SetActive(true);
    }

    public void ShowGridSelection()
    {
        gridSelectionPanel.SetActive(true);
        gameUI.SetActive(false);
    }

    public void UpdateScore(int increment)
    {
        score += increment;
        scoreText.text = "Score: " + score;
    }
    public int GetScore()
    {
        return score;
    }
    public void ToggleWarning(bool flag)
    {
        warningText.SetActive(flag);
    }
    public void TriggerWinSequence()
    {
        // Show the winning panel with a tween
        winningPanel.SetActive(true);
        winningPanel.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);

        // Restart the scene after a delay
        StartCoroutine(RestartSceneAfterDelay());
    }

    private IEnumerator RestartSceneAfterDelay()
    {
        yield return new WaitForSeconds(restartDelay);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
