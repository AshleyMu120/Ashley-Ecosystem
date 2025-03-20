using UnityEngine;

public class Dartfish : MonoBehaviour
{
    public float moveSpeed = 5.0f; // Base speed of the darting motion
    public float exploreDuration = 5.0f; // Duration of the exploring state
    public float forageDuration = 2.0f; // Duration of the foraging state
    public float circleSpeed = 2.0f; // Speed of circling around the kelp

    private float stateTimer;
    private DartfishState currentState;
    private Vector3 targetPosition;
    private Transform kelpTransform; // Reference to the kelp transform
    private Vector3 initialForagePosition; // Initial position when starting to forage
    private SpriteRenderer spriteRenderer; // Reference to the SpriteRenderer
    private AudioSource audioSource; // Reference to the AudioSource component

    private float currentAcceleration; // Randomized acceleration factor
    private float accelerationChangeInterval = 2.0f; // Time interval to change acceleration
    private float accelerationTimer; // Timer to track when to change acceleration

    private enum DartfishState
    {
        Exploring,
        Foraging,
        Eaten
    }

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>(); // Get the SpriteRenderer component
        audioSource = GetComponent<AudioSource>(); // Get the AudioSource component
        InitializeDartfish();
        currentAcceleration = Random.Range(0.5f, 1.5f); // Initialize with a random acceleration
        accelerationTimer = accelerationChangeInterval; // Set the timer for the first change
    }

    void Update()
    {
        UpdateState();
    }

    void UpdateState()
    {
        stateTimer -= Time.deltaTime;

        switch (currentState)
        {
            case DartfishState.Exploring:
                Explore();
                if (stateTimer <= 0)
                {
                    stateTimer = exploreDuration;
                    targetPosition = GetRandomPosition();
                }
                break;

            case DartfishState.Foraging:
                Forage();
                if (stateTimer <= 0)
                {
                    if (kelpTransform != null)
                    {
                        Kelp kelp = kelpTransform.GetComponent<Kelp>();
                        if (kelp != null)
                        {
                            kelp.AllowWithering();
                        }
                    }
                    StartState(DartfishState.Exploring);
                }
                break;

            case DartfishState.Eaten:
                Destroy(gameObject, 1.0f); // Destroy the Dartfish after 1 second
                break;
        }
    }

    void StartState(DartfishState newState)
    {
        currentState = newState;

        switch (newState)
        {
            case DartfishState.Exploring:
                stateTimer = exploreDuration;
                targetPosition = GetRandomPosition();

                // Change the sprite back to dartfish_1
                Sprite dartfishOriginalSprite = Resources.Load<Sprite>("dartfish_1");
                if (spriteRenderer != null && dartfishOriginalSprite != null)
                {
                    spriteRenderer.sprite = dartfishOriginalSprite;
                }
                else
                {
                    Debug.LogWarning("dartfish_1 sprite not found ");
                }
                break;

            case DartfishState.Foraging:
                stateTimer = forageDuration;
                kelpTransform = FindNearestKelp();
                if (kelpTransform != null)
                {
                    initialForagePosition = transform.position - kelpTransform.position;

                    // Change the sprite to dartfish_circling
                    Sprite dartfishCirclingSprite = Resources.Load<Sprite>("dartfish_circling");
                    if (spriteRenderer != null && dartfishCirclingSprite != null)
                    {
                        spriteRenderer.sprite = dartfishCirclingSprite;
                    }
                    else
                    {
                        Debug.LogWarning("dartfish_circling sprite not found!");
                    }
                }

                // Play the foraging sound
                if (audioSource != null && !audioSource.isPlaying)
                {
                    audioSource.Play();
                }
                break;

            case DartfishState.Eaten:
                // No additional logic for Eaten state
                break;
        }
    }

    void Explore()
    {
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            targetPosition = GetRandomPosition();
        }

        accelerationTimer -= Time.deltaTime;
        if (accelerationTimer <= 0)
        {
            currentAcceleration = Random.Range(0.5f, 1.5f);
            accelerationTimer = accelerationChangeInterval;
        }

        float burstDuration = 1.0f;
        float coastDuration = 0.5f;
        float cycleTime = burstDuration + coastDuration;

        float timeInCycle = Time.time % cycleTime;

        if (timeInCycle < burstDuration)
        {
            float burstSpeed = moveSpeed * currentAcceleration;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, burstSpeed * Time.deltaTime);
        }

        float rotationAngle = Mathf.Sin(Time.time * 1.5f) * 3.0f;
        transform.rotation = Quaternion.Euler(0, 0, rotationAngle);
    }

    void Forage()
    {
        if (kelpTransform != null)
        {
            float angle = circleSpeed * Time.time;
            float radius = initialForagePosition.magnitude;
            Vector3 offset = new Vector3(Mathf.Sin(angle), Mathf.Cos(angle), 0) * radius;
            transform.position = kelpTransform.position + offset;

            Kelp kelp = kelpTransform.GetComponent<Kelp>();
            if (kelp != null)
            {
                kelp.AllowWithering();
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Kelp"))
        {
            kelpTransform = other.transform;
            StartState(DartfishState.Foraging);
        }
        else if (other.CompareTag("Glidefin"))
        {
            StartState(DartfishState.Eaten);
        }
    }

    Transform FindNearestKelp()
    {
        GameObject[] kelps = GameObject.FindGameObjectsWithTag("Kelp");
        Transform nearestKelp = null;
        float minDistance = Mathf.Infinity;

        foreach (GameObject kelp in kelps)
        {
            float distance = Vector3.Distance(transform.position, kelp.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestKelp = kelp.transform;
            }
        }

        return nearestKelp;
    }

    Vector3 GetRandomPosition()
    {
        float screenX = Random.Range(0.0f, Screen.width);
        float screenY = Random.Range(0.0f, Screen.height);
        Vector3 screenPosition = new Vector3(screenX, screenY, Camera.main.nearClipPlane + 10.0f);
        return Camera.main.ScreenToWorldPoint(screenPosition);
    }

    public void InitializeDartfish()
    {
        StartState(DartfishState.Exploring);
        Debug.Log("Dartfish initialized at: " + transform.position);
    }
}