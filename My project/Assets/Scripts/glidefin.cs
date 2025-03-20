using UnityEngine;
using System.Collections;

public class Glidefin : MonoBehaviour
{
    public float moveSpeed = 3.0f; 
    public float swimDuration = 5.0f;
    public float huntDuration = 3.0f; 
    public float maxIdleDuration = 2.0f; 
    public float idleSpinSpeed = 100.0f; 

    private float stateTimer;
    private GlidefinState currentState;
    private Vector3 targetPosition;
    private int dartfishCollisionCount = 0; 

    private float rotationSpeed = 10f;
    private float actualRotationSpeed;
    private float minAngle = -10;
    private float maxAngle = 10;
    private float angleTracker = 0;

    private enum GlidefinState
    {
        Swimming,
        Hunting,
        Idle,
        Death
    }

    void Start()
    {
        InitializeGlidefin();
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
            case GlidefinState.Swimming:
                Swim();
                if (stateTimer <= 0)
                {
                    stateTimer = swimDuration; 
                    targetPosition = GetRandomPosition(); 
                }
                break;
            case GlidefinState.Hunting:
                Hunt();
                if (stateTimer <= 0)
                {
                    StartState(GlidefinState.Swimming);
                }
                break;
            case GlidefinState.Idle:
                Spin();
                if (stateTimer <= 0)
                {
                    StartState(GlidefinState.Swimming);
                }
                break;
            case GlidefinState.Death:
                // Do nothing, Glidefin is in the Death state
                break;
        }
    }

    void StartState(GlidefinState newState)
    {
        currentState = newState;

        switch (newState)
        {
            case GlidefinState.Swimming:
                stateTimer = swimDuration;
                targetPosition = GetRandomPosition();
                break;
            case GlidefinState.Hunting:
                stateTimer = huntDuration;
                break;
            case GlidefinState.Idle:
                stateTimer = Random.Range(0.5f, maxIdleDuration);
                break;
            case GlidefinState.Death:
                ChangeToDeathSprite(); 
                StartCoroutine(DeathRoutine());
                break;
        }
    }

    void ChangeToDeathSprite()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            Sprite deathSprite = Resources.Load<Sprite>("glidefin_death");
            if (deathSprite != null)
            {
                spriteRenderer.sprite = deathSprite;
            }
            else
            {
                Debug.LogWarning("Death sprite not found in Resources folder.");
            }
        }
        else
        {
            Debug.LogWarning("SpriteRenderer component not found on Glidefin.");
        }
    }

    IEnumerator DeathRoutine()
    {
        yield return new WaitForSeconds(2.0f);
        Destroy(gameObject); 
    }

    void Swim()
    {
        float distanceToTarget = Vector2.Distance(transform.position, targetPosition);
        if (distanceToTarget > 0.1f)
        {
            GlidefinMovement();
        }
        else
        {
            targetPosition = GetRandomPosition();
            LookAtTarget(targetPosition);
        }

        if (Random.Range(0f, 1f) < Time.deltaTime / Random.Range(5f, 10f))
        {
            StartState(GlidefinState.Idle);
        }
    }

    void GlidefinMovement()
    {
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        Squiggle();
    }

    void Squiggle()
    {
        if (angleTracker > maxAngle)
            actualRotationSpeed = -rotationSpeed;
        if (angleTracker < minAngle)
            actualRotationSpeed = rotationSpeed;

        transform.rotation = Quaternion.Euler(new Vector3(0, 0, transform.rotation.eulerAngles.z + actualRotationSpeed * Time.deltaTime));
        angleTracker += actualRotationSpeed * Time.deltaTime;
    }

    void LookAtTarget(Vector3 targetPoint)
    {
        transform.up = new Vector3(targetPoint.x, targetPoint.y, 0) - transform.position;
    }

    void Spin()
    {
        transform.Rotate(Vector3.forward, idleSpinSpeed * Time.deltaTime);
    }

    void Hunt()
    {
        // Implement hunting behavior if needed
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Dartfish"))
        {
            Destroy(other.gameObject); 
            dartfishCollisionCount++; 

            if (dartfishCollisionCount >= 3)
            {
                StartState(GlidefinState.Death); 
            }
            else
            {
                StartState(GlidefinState.Hunting);
            }
        }
    }

    Vector3 GetRandomPosition()
    {
        float screenX = Random.Range(0.0f, Screen.width);
        float screenY = Random.Range(0.0f, Screen.height);
        Vector3 screenPosition = new Vector3(screenX, screenY, Camera.main.nearClipPlane + 10.0f);
        return Camera.main.ScreenToWorldPoint(screenPosition);
    }

    public void InitializeGlidefin()
    {
        StartState(GlidefinState.Swimming);
    }
}