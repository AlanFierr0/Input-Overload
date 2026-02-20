using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Muestra la salud del jugador como corazones visuales usando dos sprites diferentes.
/// </summary>
public class LivesUI : MonoBehaviour
{
    [Header("Heart Sprites")]
    public Sprite heartSpriteAlive;
    public Sprite heartSpriteDead;
    public float heartSize = 40f;
    public float spacing = 10f;

    private Health playerHealth;
    private Image[] hearts;
    private RectTransform containerRect;

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("LivesUI: No se encontró el jugador con tag 'Player'.");
            enabled = false;
            return;
        }

        playerHealth = player.GetComponent<Health>();
        if (playerHealth == null)
        {
            Debug.LogError("LivesUI: No se encontró Health en el jugador.");
            enabled = false;
            return;
        }

        containerRect = GetComponent<RectTransform>();
        if (containerRect == null)
        {
            Debug.LogError("LivesUI: Este script debe estar en un GameObject con RectTransform.");
            enabled = false;
            return;
        }

        // Auto-cargar sprites si no fueron asignados
        if (heartSpriteAlive == null)
        {
            heartSpriteAlive = Resources.Load<Sprite>("UI/HeartAlive");
            if (heartSpriteAlive == null)
                Debug.LogWarning("LivesUI: No se encontró sprite en Resources/UI/HeartAlive.");
        }

        if (heartSpriteDead == null)
        {
            heartSpriteDead = Resources.Load<Sprite>("UI/HeartDead");
            if (heartSpriteDead == null)
                Debug.LogWarning("LivesUI: No se encontró sprite en Resources/UI/HeartDead.");
        }

        CreateHearts(playerHealth.maxHealth);
        playerHealth.OnHealthChanged += UpdateHearts;
        UpdateHearts();
    }

    void OnDestroy()
    {
        if (playerHealth != null)
            playerHealth.OnHealthChanged -= UpdateHearts;
    }

    private void CreateHearts(int count)
    {
        // Limpiar hijos anteriores
        foreach (Transform child in containerRect)
        {
            Destroy(child.gameObject);
        }

        hearts = new Image[count];

        for (int i = 0; i < count; i++)
        {
            GameObject heartGO = new GameObject($"Heart_{i}");
            heartGO.transform.SetParent(containerRect, false);

            Image heartImage = heartGO.AddComponent<Image>();
            heartImage.sprite = heartSpriteAlive;

            RectTransform rt = heartGO.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(heartSize, heartSize);
            rt.anchoredPosition = new Vector2(i * (heartSize + spacing), 0);

            hearts[i] = heartImage;
        }
    }

    private void UpdateHearts()
    {
        if (hearts == null || playerHealth == null) return;

        for (int i = 0; i < hearts.Length; i++)
        {
            // Corazón rojo si está vivo, gris si está muerto
            hearts[i].sprite = (i < playerHealth.currentHealth) ? heartSpriteAlive : heartSpriteDead;
        }
    }
}
