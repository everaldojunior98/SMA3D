using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    [RequireComponent(typeof(Text))]
    public class SetTranslation : MonoBehaviour
    {
        //Key for translation
        public string Key;
        
        void Update()
        {
            if (TranslationManager.Instance != null)
                transform.GetComponent<Text>().text = TranslationManager.Instance.GetTranslation(Key);
        }
    }
}
