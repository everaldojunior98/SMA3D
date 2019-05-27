using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Assets.Scripts
{
    public class MenuManager : MonoBehaviour
    {
        //Image to create fade effect
        public Image FadeImage;

        void Start()
        {
            //When start create fade out
            FadeImage.gameObject.SetActive(true);
            FadeImage.CrossFadeAlpha(0, 2, true);
        }

        void Update()
        {
            //Change scene or quit application
            if (Input.GetKeyDown(KeyCode.Escape))
                if(SceneManager.GetActiveScene().buildIndex == 0)
                    Application.Quit();
                else
                    LoadScene(0);
        }

        public void LoadScene(int id)
        {
            //Start time to change scene
            StartCoroutine(Delay(id));
        }

        private IEnumerator Delay(int id)
        {
            //Create fade in effect and wait 2.3 seconds to change scene
            FadeImage.CrossFadeAlpha(1, 1, true);
            yield return new WaitForSeconds(2.3f);
            SceneManager.LoadScene(id);
        }

        public void OpenUrl(string url)
        {
            Application.OpenURL(url);
        }
    }
}
