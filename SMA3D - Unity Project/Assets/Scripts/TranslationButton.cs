using Assets.Scripts;
using UnityEngine;
using UnityEngine.UI;

public class TranslationButton : MonoBehaviour
{
    public string Language;
    void Start()
    {
        if (this.GetComponent<Button>() != null)
        {
            var button = this.GetComponent<Button>();
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() =>FindObjectOfType<TranslationManager>().ChangeLanguage(Language));
        }
    }
}
