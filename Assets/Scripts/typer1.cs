using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class typer1 : MonoBehaviour
{
    [Header("UI Parents")]
    [SerializeField] private GameObject gameplayUIParent;  // Parent object containing all gameplay UI
    [SerializeField] private GameObject endGamePanel;      // End game UI panel
    
    [Header("Game UI")]
    public TMP_Text wordOutput1 = null;
    public TMP_Text wordOutput2 = null;
    public TMP_Text wordOutput3 = null;
    public TMP_Text timerText = null;
    public TMP_Text statusText = null;
    public TMP_Text inputTimerText = null;
    public TMP_Text scoreText = null; // Score during gameplay
    public TMP_Text endGameScoreText = null; // Score display on end game panel
    
    [Header("Selection Boxes")]
    public UnityEngine.UI.Image selectionBox1;
    public UnityEngine.UI.Image selectionBox2;
    public UnityEngine.UI.Image selectionBox3;
    
    [Header("Input Box Setup")]
    public GameObject letterBoxPrefab;  // Assign a TMP_Text prefab in inspector
    public Transform[] letterBoxContainers;  // Array of 3 container transforms for each word's boxes
    
    [Header("Word Bank (Difficulty Pools)")]
    [Tooltip("Words are organized by difficulty. Round 1 uses pool 1, round 2 uses pool 2, etc. Words won't repeat across rounds.")]
    private List<string>[] difficultyBank = new List<string>[]
    {
        // Round 1 - Basic Physics Terms (4-6 letters)
        new List<string> { "time", "volt", "length", "charge", "proton" },
        // Round 2 - Intermediate Terms (6-8 letters)
        new List<string> { "electron", "distance", "position", "ampere", "newton" },
        // Round 3 - Advanced Terms (8-10 letters)
        new List<string> { "velocity", "momentum", "impulse", "period", "frequency" },
        // Round 4 - Complex Terms (9-11 letters)
        new List<string> { "resistance", "centripetal", "insulator", "conductor", "acceleration" },
        // Round 5 - Expert Terms (>10 letters)
        new List<string> { "quantization", "resistivity", "equipotentials", "commutative", "fictitious" }
    };

    private string[] currentWords = new string[3];
    private string[] remainingWords = new string[3];
    private string[] hiddenWords = new string[3];
    private List<string> usedWords = new List<string>();
    private float displayTimer = 5f;
    private bool isWordHidden = false;
    private bool canType = false;
    private int selectedWordIndex = -1;
    private float inputTimer = 0f;
    private int score = 0;
    private int round = 1;
    private int maxRounds = 5;
    private bool inputPhase = false;

    [Header("Audio")]
    public AudioClip blipSound;
    private AudioSource audioSource;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        // Ensure gameplay UI is set up correctly
        if (gameplayUIParent != null)
        {
            // Set the gameplayUIParent's RectTransform to stretch and center
            if (gameplayUIParent.TryGetComponent<RectTransform>(out RectTransform parentRect))
            {
                parentRect.anchorMin = Vector2.zero;
                parentRect.anchorMax = Vector2.one;
                parentRect.offsetMin = Vector2.zero;
                parentRect.offsetMax = Vector2.zero;
            }
        }

        SetThreeUniqueWords();
        displayTimer = 5f;
        isWordHidden = false;
        // Randomize positions at game start
        RandomizeWordOutputPositions();
        UpdateAllWordDisplays();
        // hide timers at start
        if (timerText != null) timerText.gameObject.SetActive(false);
        if (inputTimerText != null) inputTimerText.gameObject.SetActive(false);
        // Initialize selection boxes
        InitializeSelectionBoxes();
    }

    private void InitializeSelectionBoxes()
    {
        // Set initial colors and hide boxes
        UnityEngine.UI.Image[] boxes = { selectionBox1, selectionBox2, selectionBox3 };
        foreach (var box in boxes)
        {
            if (box != null)
            {
                box.color = Color.black;
                box.gameObject.SetActive(false);
                // Ensure the Image component has raycast target enabled
                box.raycastTarget = true;
            }
        }
    }

    private void UpdateSelectionBoxes()
    {
        const float WIDTH_PER_LETTER = 40f;
        const float BOX_HEIGHT = 100f;  // Fixed height for the boxes
        const float PADDING = 20f;      // Extra padding on each side

        if (selectionBox1 != null && wordOutput1 != null)
        {
            // Update size and position
            float width = (currentWords[0].Length * WIDTH_PER_LETTER) + (PADDING * 2);
            selectionBox1.rectTransform.sizeDelta = new Vector2(width, BOX_HEIGHT);
            selectionBox1.rectTransform.position = wordOutput1.rectTransform.position;
            selectionBox1.gameObject.SetActive(isWordHidden && hiddenWords[0].Length > 0);
            selectionBox1.color = selectedWordIndex == 0 ? Color.yellow : Color.black;
            
            Canvas.ForceUpdateCanvases();
            if (selectedWordIndex == 0 || hiddenWords[0].Length == 0)
            {
                wordOutput1.transform.SetAsLastSibling();
            }
            else
            {
                selectionBox1.transform.SetAsLastSibling();
                wordOutput1.transform.SetAsLastSibling();
            }
        }
        
        if (selectionBox2 != null && wordOutput2 != null)
        {
            // Update size and position
            float width = (currentWords[1].Length * WIDTH_PER_LETTER) + (PADDING * 2);
            selectionBox2.rectTransform.sizeDelta = new Vector2(width, BOX_HEIGHT);
            selectionBox2.rectTransform.position = wordOutput2.rectTransform.position;
            selectionBox2.gameObject.SetActive(isWordHidden && hiddenWords[1].Length > 0);
            selectionBox2.color = selectedWordIndex == 1 ? Color.yellow : Color.black;
            
            Canvas.ForceUpdateCanvases();
            if (selectedWordIndex == 1 || hiddenWords[1].Length == 0)
            {
                wordOutput2.transform.SetAsLastSibling();
            }
            else
            {
                selectionBox2.transform.SetAsLastSibling();
                wordOutput2.transform.SetAsLastSibling();
            }
        }
        
        if (selectionBox3 != null && wordOutput3 != null)
        {
            // Update size and position
            float width = (currentWords[2].Length * WIDTH_PER_LETTER) + (PADDING * 2);
            selectionBox3.rectTransform.sizeDelta = new Vector2(width, BOX_HEIGHT);
            selectionBox3.rectTransform.position = wordOutput3.rectTransform.position;
            selectionBox3.gameObject.SetActive(isWordHidden && hiddenWords[2].Length > 0);
            selectionBox3.color = selectedWordIndex == 2 ? Color.yellow : Color.black;
            
            Canvas.ForceUpdateCanvases();
            if (selectedWordIndex == 2 || hiddenWords[2].Length == 0)
            {
                wordOutput3.transform.SetAsLastSibling();
            }
            else
            {
                selectionBox3.transform.SetAsLastSibling();
                wordOutput3.transform.SetAsLastSibling();
            }
        }
    }

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private List<TMP_Text>[] letterBoxes = new List<TMP_Text>[3];

    private void SetThreeUniqueWords()
    {
        // Do NOT clear usedWords here — we want previously used words to be excluded from future rounds.
        // Clear existing letter boxes
        ClearLetterBoxes();

        for (int i = 0; i < 3; i++)
        {
            string word = GetUniqueWord();
            currentWords[i] = word;
            remainingWords[i] = word;
            hiddenWords[i] = word;
            CreateLetterBoxesForWord(i, word);
        }
        RandomizeWordOutputPositions();
    }
    
    private void ClearLetterBoxes()
    {
        for (int i = 0; i < 3; i++)
        {
            if (letterBoxes[i] == null)
                letterBoxes[i] = new List<TMP_Text>();
                
            foreach (var box in letterBoxes[i])
            {
                if (box != null)
                    Destroy(box.gameObject);
            }
            letterBoxes[i].Clear();
        }
    }

    private void CreateLetterBoxesForWord(int wordIndex, string word)
    {
        if (letterBoxPrefab == null || letterBoxContainers == null || 
            wordIndex >= letterBoxContainers.Length || letterBoxContainers[wordIndex] == null)
            return;

        for (int i = 0; i < word.Length; i++)
        {
            GameObject boxObj = Instantiate(letterBoxPrefab, letterBoxContainers[wordIndex]);
            TMP_Text boxText = boxObj.GetComponent<TMP_Text>();
            if (boxText != null)
            {
                boxText.text = "_";
                letterBoxes[wordIndex].Add(boxText);
            }
        }
    }

    private string GetUniqueWord()
    {
        List<string> availableWords = new List<string>();

        // Start with the pool that matches the current round (round 1 -> index 0).
        int startPool = Mathf.Clamp(round - 1, 0, difficultyBank.Length - 1);

        // Search from the current difficulty up to the hardest pool to find unused words.
        for (int pool = startPool; pool < difficultyBank.Length; pool++)
        {
            foreach (var word in difficultyBank[pool])
            {
                if (!usedWords.Contains(word.ToLower()))
                    availableWords.Add(word);
            }
            if (availableWords.Count > 0)
                break; // prefer words from the nearest difficulty pool
        }

        // If none found in current+harder pools, try any pool (including easier ones) for leftovers
        if (availableWords.Count == 0)
        {
            for (int pool = 0; pool < difficultyBank.Length; pool++)
            {
                foreach (var word in difficultyBank[pool])
                {
                    if (!usedWords.Contains(word.ToLower()))
                        availableWords.Add(word);
                }
                if (availableWords.Count > 0)
                    break;
            }
        }

        if (availableWords.Count == 0)
            return "default";

        int randomIndex = Random.Range(0, availableWords.Count);
        string selectedWord = availableWords[randomIndex].ToLower();
        usedWords.Add(selectedWord);
        return selectedWord;
    }

    private void UpdateAllWordDisplays()
    {
        UpdateSingleWordDisplay(wordOutput1, 0);
        UpdateSingleWordDisplay(wordOutput2, 1);
        UpdateSingleWordDisplay(wordOutput3, 2);
    }

    private void UpdateSingleWordDisplay(TMP_Text output, int index)
    {
        if (output == null) return;
        if (isWordHidden)
        {
            if (hiddenWords[index].Length == 0)
            {
                // Word is completed, show full word
                output.text = currentWords[index];
            }
            else
            {
                // Word is partially typed
                int typedLength = currentWords[index].Length - hiddenWords[index].Length;
                string revealedPart = currentWords[index].Substring(0, typedLength);
                string hiddenPart = new string('_', hiddenWords[index].Length);
                output.text = revealedPart + hiddenPart;
            }
        }
        else
        {
            output.text = remainingWords[index];
        }
    }

    private void UpdateTimerDisplay()
    {
        if (timerText == null) return;
        if (!isWordHidden)
        {
            if (!timerText.gameObject.activeSelf) timerText.gameObject.SetActive(true);
            timerText.text = $"{displayTimer:0.0}s";
            if (displayTimer <= 0f)
            {
                // hide immediately when time reaches zero
                timerText.gameObject.SetActive(false);
            }
        }
        else
        {
            if (timerText.gameObject.activeSelf) timerText.gameObject.SetActive(false);
        }
    }

    private void UpdateStatusDisplay()
    {
        if (statusText != null)
        {
            if (!isWordHidden)
            {
                statusText.text = "MEMORIZE";
                Color memorizeColor;
                // Parse the hex color #C82909 and assign it only here
                if (ColorUtility.TryParseHtmlString("#C82909", out memorizeColor))
                {
                    statusText.color = memorizeColor;
                }
            }
            else if (canType)
            {
                statusText.text = "TYPE NOW!";
                Color memorizeColor;
                // Parse the hex color #C82909 and assign it only here
                if (ColorUtility.TryParseHtmlString("#09C826", out memorizeColor))
                {
                    statusText.color = memorizeColor;
                }
            }
            else
            {
                statusText.text = "WAIT...";
                statusText.color = Color.red;
            }
        }
    }

    private void ShowWord()
    {
        isWordHidden = false;
        UpdateAllWordDisplays();
        UpdateStatusDisplay();
    }

    private void HideWord()
    {
        isWordHidden = true;
        inputPhase = true;
        inputTimer = 10f;
        UpdateAllWordDisplays();
        UpdateStatusDisplay();
        // show input timer when input phase starts
        if (inputTimerText != null) inputTimerText.gameObject.SetActive(true);
        UpdateInputTimerDisplay();
    }

    // Update is called once per frame
    private void Update()
    {
        // Handle box selection first
        if (inputPhase && canType && Input.GetMouseButtonDown(0)) // Left mouse button
        {
            // Use UI Raycasting to detect clicks
            UnityEngine.EventSystems.PointerEventData eventData = new UnityEngine.EventSystems.PointerEventData(UnityEngine.EventSystems.EventSystem.current);
            eventData.position = Input.mousePosition;
            List<UnityEngine.EventSystems.RaycastResult> results = new List<UnityEngine.EventSystems.RaycastResult>();
            UnityEngine.EventSystems.EventSystem.current.RaycastAll(eventData, results);

            foreach (var result in results)
            {
                // Check if we clicked any of our selection boxes
                if (result.gameObject == selectionBox1?.gameObject)
                {
                    selectedWordIndex = 0;
                    UpdateSelectionBoxes();
                    break;
                }
                else if (result.gameObject == selectionBox2?.gameObject)
                {
                    selectedWordIndex = 1;
                    UpdateSelectionBoxes();
                    break;
                }
                else if (result.gameObject == selectionBox3?.gameObject)
                {
                    selectedWordIndex = 2;
                    UpdateSelectionBoxes();
                    break;
                }
            }
        }

        // Regular update logic
        if (!isWordHidden)
        {
            displayTimer -= Time.deltaTime;
            UpdateTimerDisplay();
            if (displayTimer <= 0)
            {
                HideWord();
                canType = true;
                // ensure memorization timer hidden at the moment it hits 0
                if (timerText != null) timerText.gameObject.SetActive(false);
            }
        }
        else if (inputPhase)
        {
            inputTimer -= Time.deltaTime;
            UpdateInputTimerDisplay();
            if (inputTimer <= 0)
            {
                inputPhase = false;
                canType = false;
                // hide input timer when time's up
                if (inputTimerText != null) inputTimerText.gameObject.SetActive(false);
                round++;
                if (round <= maxRounds)
                {
                    SetThreeUniqueWords();
                    displayTimer = 5f;
                    isWordHidden = false;
                    UpdateAllWordDisplays();
                }
                else
                {
                    ShowGameOver();
                }
            }
        }
        UpdateStatusDisplay();
        UpdateSelectionBoxes();
        if (canType && inputPhase)
        {
            CheckInput();
        }
    }

    private void CheckInput()
    {
        if(Input.anyKeyDown)
        {
           string keysPressed = Input.inputString;

           if(keysPressed.Length ==1)
               EnterLetter(keysPressed);
           }
        }

    private void EnterLetter(string typedLetter)
    {
        if (selectedWordIndex < 0 || selectedWordIndex > 2) return;
        if (hiddenWords[selectedWordIndex].Length > 0 && hiddenWords[selectedWordIndex][0].ToString() == typedLetter)
        {
            hiddenWords[selectedWordIndex] = hiddenWords[selectedWordIndex].Remove(0, 1);
            UpdateAllWordDisplays();
            if (hiddenWords[selectedWordIndex].Length == 0)
            {
                score++;
                UpdateScoreDisplay();
                PlayBlipSound();
            }
            if (IsWordComplete())
            {
                inputPhase = false;
                canType = false;
                // Hide input timer when all words are complete
                if (inputTimerText != null) inputTimerText.gameObject.SetActive(false);
                round++;
                if (round <= maxRounds)
                {
                    SetThreeUniqueWords();
                    displayTimer = 5f;
                    isWordHidden = false;
                    // Randomize positions at start of memorization phase
                    RandomizeWordOutputPositions();
                    UpdateAllWordDisplays();
                }
                else
                {
                    ShowGameOver();
                }
            }
        }
    }

    private void PlayBlipSound()
    {
        if (blipSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(blipSound);
        }
    }

    private bool IsWordComplete()
    {
        for (int i = 0; i < 3; i++)
        {
            if (hiddenWords[i].Length > 0)
            {
                return false;
            }
        }
        return true;
    }

    private void RandomizeWordOutputPositions()
    {
        TMP_Text[] outputs = { wordOutput1, wordOutput2, wordOutput3 };
        
        // Define the bounds
        const float MIN_X = -150f;
        const float MAX_X = 250f;
        const float MIN_Y = -437f;
        const float MAX_Y = 200f;
        
        // Minimum distances between words on each axis
        const float MIN_DISTANCE_X = 50f;
        const float MIN_DISTANCE_Y = 150f;
        
        // Generate random positions with minimum distance between them
        Vector2[] randomPositions = new Vector2[3];
        
        for (int i = 0; i < 3; i++)
        {
            bool validPosition = false;
            Vector2 newPos = Vector2.zero;
            
            // Try to find a valid position that's not too close to other words
            int maxAttempts = 50; // Prevent infinite loop
            int attempts = 0;
            
            while (!validPosition && attempts < maxAttempts)
            {
                // Generate random position within bounds
                newPos = new Vector2(
                    Random.Range(MIN_X, MAX_X),
                    Random.Range(MIN_Y, MAX_Y)
                );
                
                // Check distances from all previous positions on both axes
                validPosition = true;
                for (int j = 0; j < i; j++)
                {
                    float xDistance = Mathf.Abs(newPos.x - randomPositions[j].x);
                    float yDistance = Mathf.Abs(newPos.y - randomPositions[j].y);
                    
                    // If either X or Y distance is too small, position is invalid
                    if (xDistance < MIN_DISTANCE_X || yDistance < MIN_DISTANCE_Y)
                    {
                        validPosition = false;
                        break;
                    }
                }
                attempts++;
            }
            
            randomPositions[i] = newPos;
        }
        
        // Shuffle the positions randomly
        for (int i = randomPositions.Length - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            Vector2 temp = randomPositions[i];
            randomPositions[i] = randomPositions[randomIndex];
            randomPositions[randomIndex] = temp;
        }
        
        // Apply random positions to the outputs
        for (int i = 0; i < outputs.Length; i++)
        {
            if (outputs[i] != null)
            {
                RectTransform rt = outputs[i].rectTransform;
                // Set the anchor to the center of the screen
                rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
                rt.pivot = new Vector2(0.5f, 0.5f);
                // Apply the random position
                rt.anchoredPosition = randomPositions[i];
            }
        }
    }



    private void OnWordOutputClicked(int index)
    {
        selectedWordIndex = index;
        UpdateSelectionBoxes();
    }

    private void UpdateInputTimerDisplay()
    {
        if (inputTimerText != null)
        {
            inputTimerText.text = $"{Mathf.Max(0, inputTimer):0.0}s";
        }
    }

    private void UpdateScoreDisplay()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {score}";
        }
    }

    private void ShowGameOver()
    {
        // Hide all gameplay UI at once
        if (gameplayUIParent != null)
        {
            gameplayUIParent.SetActive(false);
        }
        // Store final score in the ScoreManager for the ScoreUI to read
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.SetFinalScore(score);
        }

        // Show end game UI and update score display
        if (endGamePanel != null)
        {
            endGamePanel.SetActive(true);
            if (endGameScoreText != null)
            {
                endGameScoreText.text = $"Final Score: {score}";
            }
        }
        
        // Reset game state variables
        isWordHidden = false;
        canType = false;
        inputPhase = false;
        selectedWordIndex = -1;
    }
}

