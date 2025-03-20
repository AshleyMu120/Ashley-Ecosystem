using UnityEngine;

public class Kelp : MonoBehaviour
{
    public float swaySpeed = 1.0f; 
    public float swayAmount = 0.5f;
    public float growDuration = 2.0f; 
    public float stableDuration = 10.0f; 
    public float witherDuration = 5.0f;

    private Vector3 initialPosition;
    private float stateTimer;
    private KelpState currentState;
    private Vector3 initialScale;
    private Vector3 targetScale;
    private bool canWither = false; 
    private SpriteRenderer spriteRenderer; 
    private enum KelpState
    {
        Growing,
        Stable,
        Withering
    }

    void Start()
    {
        initialPosition = transform.position;
        initialScale = transform.localScale;
        targetScale = initialScale * 2.0f;
        currentState = KelpState.Growing;
        stateTimer = growDuration;
        gameObject.tag = "Kelp"; 

        spriteRenderer = GetComponent<SpriteRenderer>(); 
    }

    void Update()
    {
        Sway();
        UpdateState();
    }

    void Sway()
    {
        float sway = Mathf.Sin(Time.time * swaySpeed) * swayAmount;
        transform.position = new Vector3(initialPosition.x + sway, initialPosition.y, initialPosition.z);
    }

    void UpdateState()
    {
        stateTimer -= Time.deltaTime;

        switch (currentState)
        {
            case KelpState.Growing:
                Grow();
                if (stateTimer <= 0)
                {
                    StartState(KelpState.Stable);
                }
                break;
            case KelpState.Stable:
                if (stateTimer <= 0 && canWither)
                {
                    StartState(KelpState.Withering);
                }
                break;
            case KelpState.Withering:
                Wither();
                if (stateTimer <= 0)
                {
                    Destroy(gameObject); 
                }
                break;
        }
    }

    void StartState(KelpState newState)
    {
        currentState = newState;

        switch (newState)
        {
            case KelpState.Growing:
                stateTimer = growDuration;
                break;
            case KelpState.Stable:
                stateTimer = stableDuration;
                break;
            case KelpState.Withering:
                stateTimer = witherDuration;

                Sprite kelpDeathSprite = Resources.Load<Sprite>("kelp_death");
                if (spriteRenderer != null && kelpDeathSprite != null)
                {
                    spriteRenderer.sprite = kelpDeathSprite; 
                } // <-- Added missing closing brace for the if block
                break; // <-- Properly placed break statement
        }
    }

    void Grow()
    {
        float progress = 1.0f - (stateTimer / growDuration);
        transform.localScale = Vector3.Lerp(initialScale, targetScale, progress);
    }

    void Wither()
    {
        float progress = stateTimer / witherDuration;
        transform.localScale = Vector3.Lerp(Vector3.zero, targetScale, progress);
    }

    public void AllowWithering()
    {
        if (currentState == KelpState.Stable)
        {
            StartState(KelpState.Withering); 
        }
        canWither = true;
    }
}