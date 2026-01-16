using UnityEngine;

public class PlayerSkin : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Sprite[] skins;

    public void ApplySkin()
    {
        int skinIndex = PlayerPrefs.GetInt("SelectedSkin", 0);

        if (skins.Length > 0 && skinIndex < skins.Length)
        {
            spriteRenderer.sprite = skins[skinIndex];
        }
    }

    void Start()
    {
        ApplySkin(); // kör också när scenen laddas
    }
}


