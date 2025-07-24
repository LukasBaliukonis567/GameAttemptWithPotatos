using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelUpUI : MonoBehaviour
{
    // No need for public GameObject levelUpScreen any more!

    public Button[] optionButtons;
    public TextMeshProUGUI[] optionTexts;

    void Awake()               // run before other scripts
    {
        gameObject.SetActive(false);   // hide panel at start
    }

    public void ShowLevelUpScreen(string[] options)
    {
        gameObject.SetActive(true);
        Time.timeScale = 0f;

        for (int i = 0; i < optionButtons.Length; i++)
        {
            int idx = i;
            optionTexts[i].text = options[i];
            optionButtons[i].onClick.RemoveAllListeners();
            optionButtons[i].onClick.AddListener(() => OnUpgradeSelected(options[idx]));
        }
    }

    void OnUpgradeSelected(string upg)
    {
        LevelUpManager.Instance.ApplyUpgrade(upg);
        gameObject.SetActive(false);
        Time.timeScale = 1f;
    }
}
