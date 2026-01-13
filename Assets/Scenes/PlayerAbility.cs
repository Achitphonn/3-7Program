using UnityEngine;

public class PlayerAbility : MonoBehaviour
{
    [Header("Smoke Dash / Invulnerability")]
    public float invulnerabilityDuration = 1.0f;   
    public float cooldownDuration = 5.0f;         
    public GameObject cloudEffect;                 

    float invulnTimer = 0f;
    float cooldownTimer = 0f;

    public bool IsInvulnerable => invulnTimer > 0f;
    public float CooldownRemaining => Mathf.Max(0f, cooldownTimer);
    public float CooldownDuration => cooldownDuration;

    void Update()
    {
        if (GameManager.Instance && GameManager.Instance.IsPlaying)
        {
           
            if (Input.GetKeyDown(KeyCode.Space) && CooldownRemaining <= 0f)
            {
                Activate();
            }
        }

        
        if (invulnTimer > 0f)
        {
            invulnTimer -= Time.deltaTime;
            if (invulnTimer <= 0f) SetCloud(false);
        }

        if (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;
        }
    }

    void Activate()
    {
        invulnTimer = invulnerabilityDuration;
        cooldownTimer = cooldownDuration;
        SetCloud(true);
    }

    void SetCloud(bool on)
    {
        if (cloudEffect) cloudEffect.SetActive(on);
    }
}


