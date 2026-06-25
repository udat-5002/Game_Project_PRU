// ═══════════════════════════════════════════════════════════════
// NGƯỜI ĐƯA THƯ - Unity C# Script Templates
// ═══════════════════════════════════════════════════════════════

// Instruction: Copy những script này vào Assets/Scripts/ trên Unity
// Sau đó điều chỉnh theo nhu cầu của project

// ═══════════════════════════════════════════════════════════════
// 1. PLAYER CONTROLLER
// ═══════════════════════════════════════════════════════════════

using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float walkSpeed = 4f;
    [SerializeField] private float runSpeed = 7f;
    [SerializeField] private float sprintSpeed = 9f;
    [SerializeField] private float acceleration = 5f;
    [SerializeField] private float groundDrag = 5f;
    
    [Header("Stamina")]
    [SerializeField] private float maxStamina = 100f;
    [SerializeField] private float staminaRecovery = 15f;
    [SerializeField] private float staminaCost = 20f;
    [SerializeField] private float recoveryDelay = 2f;
    
    [Header("Jumping")]
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float groundDrag_Jump = 0.5f;
    [SerializeField] private float airDrag = 2f;
    
    [Header("Ground Check")]
    [SerializeField] private float groundDragValue = 0.5f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckDistance = 0.2f;
    
    private Rigidbody rb;
    private Animator animator;
    private InputAction moveAction;
    private InputAction sprintAction;
    private InputAction jumpAction;
    
    private Vector3 moveDirection;
    private Vector2 inputVector;
    private float currentStamina;
    private float staminaRecoveryTimer;
    private bool isGrounded;
    private bool isSprinting;
    private bool isMoving;
    
    private enum MovementState { Idle, Walking, Running, Sprinting, Jumping, Falling }
    private MovementState currentState = MovementState.Idle;
    
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        currentStamina = maxStamina;
        
        // Setup Input System
        var input = GetComponent<PlayerInput>();
        moveAction = input.actions["Move"];
        sprintAction = input.actions["Sprint"];
        jumpAction = input.actions["Jump"];
    }
    
    private void Update()
    {
        // Ground check
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 
                                     groundCheckDistance, groundLayer);
        
        // Get input
        inputVector = moveAction.ReadValue<Vector2>();
        isMoving = inputVector.magnitude > 0.1f;
        isSprinting = sprintAction.IsPressed() && isGrounded;
        
        // Handle jump
        if (jumpAction.triggered && isGrounded)
        {
            Jump();
        }
        
        // Update stamina
        UpdateStamina();
        
        // Update state
        UpdateMovementState();
        
        // Move player
        MovePlayer();
        
        // Apply drag
        ApplyDrag();
    }
    
    private void MovePlayer()
    {
        // Calculate movement direction based on camera
        Vector3 forward = Camera.main.transform.forward;
        Vector3 right = Camera.main.transform.right;
        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();
        
        moveDirection = (forward * inputVector.y + right * inputVector.x).normalized;
        
        // Determine speed
        float targetSpeed = 0f;
        if (isMoving)
        {
            if (isSprinting && currentStamina > 0)
                targetSpeed = sprintSpeed;
            else if (isMoving)
                targetSpeed = walkSpeed; // Default walk
        }
        
        // Apply velocity
        Vector3 targetVelocity = moveDirection * targetSpeed;
        rb.velocity = Vector3.Lerp(rb.velocity, 
                                   new Vector3(targetVelocity.x, rb.velocity.y, targetVelocity.z),
                                   acceleration * Time.deltaTime);
    }
    
    private void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }
    
    private void UpdateStamina()
    {
        if (isSprinting && isMoving && currentStamina > 0)
        {
            currentStamina -= staminaCost * Time.deltaTime;
            staminaRecoveryTimer = recoveryDelay;
        }
        else
        {
            staminaRecoveryTimer -= Time.deltaTime;
            if (staminaRecoveryTimer <= 0)
            {
                currentStamina = Mathf.Min(currentStamina + staminaRecovery * Time.deltaTime, maxStamina);
            }
        }
    }
    
    private void UpdateMovementState()
    {
        if (!isGrounded)
        {
            currentState = MovementState.Falling;
        }
        else if (isSprinting && isMoving && currentStamina > 0)
        {
            currentState = MovementState.Sprinting;
        }
        else if (isMoving)
        {
            currentState = MovementState.Walking;
        }
        else
        {
            currentState = MovementState.Idle;
        }
        
        // Update animator
        animator.SetBool("IsMoving", isMoving);
        animator.SetBool("IsSprinting", isSprinting && currentStamina > 0);
        animator.SetBool("IsGrounded", isGrounded);
    }
    
    private void ApplyDrag()
    {
        if (isGrounded)
        {
            rb.drag = groundDrag;
        }
        else
        {
            rb.drag = airDrag;
        }
    }
    
    public float GetStaminaPercent() => currentStamina / maxStamina;
}

// ═══════════════════════════════════════════════════════════════
// 2. QUEST MANAGER
// ═══════════════════════════════════════════════════════════════

using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Quest
{
    public string questId;
    public string questName;
    public string description;
    public bool isMainQuest;
    public bool isCompleted;
    public int rewardGold;
    public float rewardExperience;
    
    public List<string> objectives = new List<string>();
    public List<bool> objectiveStatus = new List<bool>();
}

public class QuestManager : MonoBehaviour
{
    public static QuestManager instance;
    
    private Dictionary<string, Quest> allQuests = new Dictionary<string, Quest>();
    private List<Quest> activeQuests = new List<Quest>();
    private List<Quest> completedQuests = new List<Quest>();
    
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }
    
    public void AddQuest(Quest quest)
    {
        allQuests[quest.questId] = quest;
        activeQuests.Add(quest);
        Debug.Log("Quest added: " + quest.questName);
        
        // Fire event (implement event system later)
        // OnQuestAdded?.Invoke(quest);
    }
    
    public void CompleteObjective(string questId, int objectiveIndex)
    {
        if (allQuests.ContainsKey(questId))
        {
            Quest quest = allQuests[questId];
            if (objectiveIndex < quest.objectiveStatus.Count)
            {
                quest.objectiveStatus[objectiveIndex] = true;
                
                // Check if all objectives done
                bool allComplete = true;
                foreach (bool status in quest.objectiveStatus)
                {
                    if (!status)
                    {
                        allComplete = false;
                        break;
                    }
                }
                
                if (allComplete)
                {
                    CompleteQuest(questId);
                }
            }
        }
    }
    
    public void CompleteQuest(string questId)
    {
        if (allQuests.ContainsKey(questId))
        {
            Quest quest = allQuests[questId];
            quest.isCompleted = true;
            activeQuests.Remove(quest);
            completedQuests.Add(quest);
            Debug.Log("Quest completed: " + quest.questName);
            
            // Reward player
            // PlayerStats.AddGold(quest.rewardGold);
            // PlayerStats.AddExp(quest.rewardExperience);
        }
    }
    
    public List<Quest> GetActiveQuests() => activeQuests;
    public Quest GetQuest(string questId) => allQuests.ContainsKey(questId) ? allQuests[questId] : null;
}

// ═══════════════════════════════════════════════════════════════
// 3. DIALOGUE SYSTEM
// ═══════════════════════════════════════════════════════════════

using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class DialogLine
{
    public string characterName;
    public string textContent;
    [TextArea(2, 4)]
    public string fullText;
    public float displayTime = 3f;
    public AudioClip voiceClip;
}

[System.Serializable]
public class DialogOption
{
    public string optionText;
    public DialogLine responseLine;
    public int nextDialogIndex = -1;
}

public class DialogueSystem : MonoBehaviour
{
    [SerializeField] private Canvas dialogueCanvas;
    [SerializeField] private TextMeshProUGUI characterNameText;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private GameObject optionButtonPrefab;
    
    private Queue<DialogLine> dialogueQueue = new Queue<DialogLine>();
    private bool isDialogueActive = false;
    private AudioSource audioSource;
    
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        dialogueCanvas.enabled = false;
    }
    
    public void StartDialogue(DialogLine[] lines)
    {
        if (isDialogueActive) return;
        
        isDialogueActive = true;
        dialogueQueue.Clear();
        
        foreach (DialogLine line in lines)
        {
            dialogueQueue.Enqueue(line);
        }
        
        dialogueCanvas.enabled = true;
        DisplayNextLine();
    }
    
    private void DisplayNextLine()
    {
        if (dialogueQueue.Count == 0)
        {
            EndDialogue();
            return;
        }
        
        DialogLine currentLine = dialogueQueue.Dequeue();
        characterNameText.text = currentLine.characterName;
        
        StartCoroutine(TypeDialogue(currentLine));
    }
    
    private IEnumerator TypeDialogue(DialogLine line)
    {
        dialogueText.text = "";
        
        foreach (char c in line.fullText)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(0.05f);
        }
        
        if (line.voiceClip != null)
        {
            audioSource.PlayOneShot(line.voiceClip);
        }
        
        yield return new WaitForSeconds(line.displayTime);
        
        DisplayNextLine();
    }
    
    private void EndDialogue()
    {
        isDialogueActive = false;
        dialogueCanvas.enabled = false;
        dialogueQueue.Clear();
    }
}

// ═══════════════════════════════════════════════════════════════
// 4. NPC CONTROLLER
// ═══════════════════════════════════════════════════════════════

using UnityEngine;

public class NPCController : MonoBehaviour
{
    [SerializeField] private string npcId;
    [SerializeField] private string npcName;
    [SerializeField] private int age;
    [SerializeField] private DialogLine[] welcomeDialogue;
    [SerializeField] private Quest associatedQuest;
    
    private Animator animator;
    private bool hasInteracted = false;
    
    private void Start()
    {
        animator = GetComponent<Animator>();
    }
    
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Enable interaction prompt
            // InteractionUI.Show(npcName);
        }
    }
    
    private void OnTriggerExit(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            // InteractionUI.Hide();
        }
    }
    
    public void Interact()
    {
        if (!hasInteracted)
        {
            hasInteracted = true;
            animator.SetBool("IsInteracting", true);
            
            // Start dialogue
            DialogueSystem dialogueSystem = FindObjectOfType<DialogueSystem>();
            dialogueSystem.StartDialogue(welcomeDialogue);
            
            // If NPC has quest
            if (associatedQuest != null)
            {
                QuestManager.instance.AddQuest(associatedQuest);
            }
        }
    }
    
    public void DeliverLetter(Letter letter)
    {
        if (letter.recipientName == npcName)
        {
            // Play reaction animation
            animator.SetTrigger("ReceiveLetter");
            
            // Complete quest objective if applicable
            if (associatedQuest != null)
            {
                QuestManager.instance.CompleteObjective(associatedQuest.questId, 0);
            }
        }
    }
}

// ═══════════════════════════════════════════════════════════════
// 5. LETTER/MAIL SYSTEM
// ═══════════════════════════════════════════════════════════════

using UnityEngine;

[System.Serializable]
public class Letter
{
    public string letterId;
    public string senderName;
    public string recipientName;
    [TextArea(5, 10)]
    public string content;
    public bool isDelivered = false;
    public int recipientNpcId;
}

public class MailBag : MonoBehaviour
{
    [SerializeField] private int maxCapacity = 10;
    private Letter[] mail;
    private int currentCount = 0;
    
    private void Start()
    {
        mail = new Letter[maxCapacity];
    }
    
    public bool AddLetter(Letter letter)
    {
        if (currentCount < maxCapacity)
        {
            mail[currentCount] = letter;
            currentCount++;
            return true;
        }
        return false;
    }
    
    public Letter RemoveLetter(int index)
    {
        if (index >= 0 && index < currentCount)
        {
            Letter letter = mail[index];
            
            // Shift remaining letters
            for (int i = index; i < currentCount - 1; i++)
            {
                mail[i] = mail[i + 1];
            }
            currentCount--;
            
            return letter;
        }
        return null;
    }
    
    public Letter[] GetAllLetters()
    {
        Letter[] result = new Letter[currentCount];
        System.Array.Copy(mail, result, currentCount);
        return result;
    }
    
    public float GetCapacityPercent() => (float)currentCount / maxCapacity;
}

// ═══════════════════════════════════════════════════════════════
// 6. GAME MANAGER / SCENE CONTROLLER
// ═══════════════════════════════════════════════════════════════

using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    
    [SerializeField] private int currentChapter = 1;
    [SerializeField] private string sceneName;
    
    private PlayerController playerController;
    private QuestManager questManager;
    private bool isPaused = false;
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
        questManager = QuestManager.instance;
        sceneName = SceneManager.GetActiveScene().name;
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }
    
    public void TogglePause()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0f : 1f;
        // Show/hide pause menu
    }
    
    public void LoadScene(string sceneName)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }
    
    public void NextChapter()
    {
        currentChapter++;
        LoadScene($"Chapter_{currentChapter}");
    }
    
    public int GetCurrentChapter() => currentChapter;
}

// ═══════════════════════════════════════════════════════════════
// 7. STEALTH DETECTION SYSTEM (CHAPTER 2)
// ═══════════════════════════════════════════════════════════════

using UnityEngine;

public class GuardAI : MonoBehaviour
{
    [SerializeField] private float detectionRange = 20f;
    [SerializeField] private float fieldOfViewAngle = 60f;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private float patrolSpeed = 2f;
    
    private enum GuardState { Patrol, Alert, Combat }
    private GuardState currentState = GuardState.Patrol;
    private int currentPatrolPoint = 0;
    private Transform playerTransform;
    private float alertTimer = 0f;
    private float alertDuration = 30f;
    
    private void Start()
    {
        playerTransform = FindObjectOfType<PlayerController>().transform;
    }
    
    private void Update()
    {
        switch (currentState)
        {
            case GuardState.Patrol:
                Patrol();
                CheckForPlayer();
                break;
            case GuardState.Alert:
                AlertBehavior();
                break;
            case GuardState.Combat:
                CombatBehavior();
                break;
        }
    }
    
    private void Patrol()
    {
        if (patrolPoints.Length == 0) return;
        
        Transform targetPoint = patrolPoints[currentPatrolPoint];
        transform.position = Vector3.MoveTowards(transform.position, targetPoint.position,
                                                 patrolSpeed * Time.deltaTime);
        
        if (Vector3.Distance(transform.position, targetPoint.position) < 0.5f)
        {
            currentPatrolPoint = (currentPatrolPoint + 1) % patrolPoints.Length;
        }
    }
    
    private void CheckForPlayer()
    {
        float distToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        
        if (distToPlayer < detectionRange)
        {
            // Check if in line of sight
            Vector3 dirToPlayer = (playerTransform.position - transform.position).normalized;
            float angle = Vector3.Angle(transform.forward, dirToPlayer);
            
            if (angle < fieldOfViewAngle)
            {
                // Check for raycast obstruction
                RaycastHit hit;
                if (Physics.Raycast(transform.position, dirToPlayer, out hit))
                {
                    if (hit.transform == playerTransform)
                    {
                        currentState = GuardState.Combat;
                        TriggerAlarm();
                    }
                }
            }
        }
    }
    
    private void AlertBehavior()
    {
        alertTimer -= Time.deltaTime;
        
        if (alertTimer <= 0)
        {
            currentState = GuardState.Patrol;
        }
    }
    
    private void CombatBehavior()
    {
        // Chase player or move toward last known position
        transform.position = Vector3.MoveTowards(transform.position, playerTransform.position,
                                                 patrolSpeed * 1.5f * Time.deltaTime);
    }
    
    private void TriggerAlarm()
    {
        Debug.Log("ALARM TRIGGERED! Player detected!");
        // Call game manager to handle alarm
        // Spawn reinforcements, player fails mission
    }
}

// ═══════════════════════════════════════════════════════════════
// 8. SAVE/LOAD SYSTEM
// ═══════════════════════════════════════════════════════════════

using UnityEngine;
using System.IO;

[System.Serializable]
public class GameSave
{
    public int chapter;
    public float[] playerPosition;
    public List<string> completedQuests = new List<string>();
    public int currency;
}

public class SaveSystem : MonoBehaviour
{
    private string savePath;
    
    private void Awake()
    {
        savePath = Application.persistentDataPath + "/savegame.json";
    }
    
    public void SaveGame()
    {
        GameSave save = new GameSave();
        save.chapter = GameManager.instance.GetCurrentChapter();
        
        // Collect player data
        PlayerController player = FindObjectOfType<PlayerController>();
        save.playerPosition = new float[] { player.transform.position.x, 
                                           player.transform.position.y,
                                           player.transform.position.z };
        
        // Save to JSON
        string json = JsonUtility.ToJson(save, true);
        File.WriteAllText(savePath, json);
        Debug.Log("Game saved!");
    }
    
    public GameSave LoadGame()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            GameSave save = JsonUtility.FromJson<GameSave>(json);
            Debug.Log("Game loaded!");
            return save;
        }
        return null;
    }
}

// ═══════════════════════════════════════════════════════════════
// END OF SCRIPT TEMPLATES
// ═══════════════════════════════════════════════════════════════
