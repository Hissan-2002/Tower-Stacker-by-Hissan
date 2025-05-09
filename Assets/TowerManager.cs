using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TowerManager : MonoBehaviour
{
    [Header("Tower Settings")]
    [SerializeField] private GameObject blockPrefab;
    [SerializeField] private Vector3 spawnOffset = new Vector3(0, 1, 0); // Upward offset
    [SerializeField] private float swayForce = 5f;
    [SerializeField] private float maxTiltAngle = 30f;

    [Header("UI Settings")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private GameObject gameOverPanel;

    private GameObject currentBlock;
    private int score = 0;
    private bool gameOver = false;

    private CameraFollow cameraFollow;

    private void Start()
    {
        // Find the CameraFollow component on Main Camera
        cameraFollow = Camera.main.GetComponent<CameraFollow>();

        SpawnBlock();
    }

    private void Update()
    {
        if (gameOver)
            return;

        HandleTouchInput();
        ApplySway();

        // Update score display
        scoreText.text = "Score: " + score;
    }

    private void HandleTouchInput()
    {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            SpawnBlock();
        }
    }

    private void SpawnBlock()
    {
        Vector3 spawnPosition;

        if (currentBlock == null)
        {
            // First block: spawn slightly above the ground
            spawnPosition = new Vector3(0, 1, 0);
        }
        else
        {
            spawnPosition = currentBlock.transform.position + spawnOffset;
        }

        currentBlock = Instantiate(blockPrefab, spawnPosition, Quaternion.identity);

        RandomizeBlock(currentBlock);

        // Update CameraFollow to follow the latest block
        if (cameraFollow != null)
        {
            cameraFollow.SetTarget(currentBlock.transform);
        }

        // Increment Score
        score++;
    }

    private void RandomizeBlock(GameObject block)
    {
        float randomScale = Random.Range(0.8f, 1.2f);
        block.transform.localScale = new Vector3(randomScale, randomScale, randomScale);

        Renderer renderer = block.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = Random.ColorHSV();
        }
    }

    private void ApplySway()
    {
        if (currentBlock == null)
            return;

        Rigidbody rb = currentBlock.GetComponent<Rigidbody>();
        if (rb == null)
            return;

        float swayMultiplier = 1f + (score * 0.05f); // Slight increase per block
        Vector3 randomForce = new Vector3(
            Mathf.PerlinNoise(Time.time, 0f) - 0.5f,
            0f,
            Mathf.PerlinNoise(0f, Time.time) - 0.5f
        );

        rb.AddForce(randomForce * swayForce * swayMultiplier * Time.deltaTime, ForceMode.Force);

        // Game Over Check
        float tilt = Vector3.Angle(Vector3.up, currentBlock.transform.up);
        if (tilt > maxTiltAngle)
        {
            TriggerGameOver();
        }
    }

    private void TriggerGameOver()
    {
        gameOver = true;
        gameOverPanel.SetActive(true);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
