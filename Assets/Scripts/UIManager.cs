using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject gridSelectionPanel; // Panel for selecting the grid
    public GameObject gameUI; // Main game UI
    public TMP_Dropdown gridSizeDropdown; // TMP Dropdown for grid size selection
    public Button startButton; // Button to start the game

    public event Action<int, int> OnGridSelected; // Event to notify the selected grid size

    private Dictionary<string, Vector2Int> gridOptions = new Dictionary<string, Vector2Int>
    {
        { "4x4", new Vector2Int(4, 4) },
        { "5x4", new Vector2Int(5, 4) },
        { "2x2", new Vector2Int(2, 2) }
    };

    void Start()
    {
        // Populate the dropdown
        PopulateDropdown();

        // Set default UI state
        gridSelectionPanel.SetActive(true);
        gameUI.SetActive(false);

        // Hook up the start button
        startButton.onClick.AddListener(OnStartButtonClicked);
    }

    void PopulateDropdown()
    {
        gridSizeDropdown.ClearOptions();

        // Populate TMP_Dropdown with grid options
        var options = new List<string>(gridOptions.Keys);
        gridSizeDropdown.AddOptions(options);

        // Set default selection
        gridSizeDropdown.value = 0;
    }

    void OnStartButtonClicked()
    {
        // Get the selected grid size
        string selectedOption = gridSizeDropdown.options[gridSizeDropdown.value].text;
        Vector2Int gridSize = gridOptions[selectedOption];

        // Notify CardManager
        OnGridSelected?.Invoke(gridSize.x, gridSize.y);

        // Hide grid selection UI and show game UI
        gridSelectionPanel.SetActive(false);
        gameUI.SetActive(true);
    }
}
