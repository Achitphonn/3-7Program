using UnityEngine;

public class SkinManager : MonoBehaviour
{
    public static SkinManager Instance;

    public Sprite[] skins;   // alla skins
    public int selectedSkin = 0;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetSkin(int index)
    {
        selectedSkin = index;
        PlayerPrefs.SetInt("SelectedSkin", index);
    }

    public int GetSkin()
    {
        return PlayerPrefs.GetInt("SelectedSkin", 0);
    }
    public enum SkinType
    {
        skin1 = 0,
        skin2 = 1,
        skin3 = 2
    }

}

