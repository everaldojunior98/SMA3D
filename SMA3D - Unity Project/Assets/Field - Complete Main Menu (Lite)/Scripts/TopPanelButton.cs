using UnityEngine;

public class TopPanelButton : MonoBehaviour {

    private Animator buttonAnimator;

    void Start()
    {
        buttonAnimator = this.GetComponent<Animator>();
    }

    public void HoverButton()
    {
        try
        {
            if (buttonAnimator.GetCurrentAnimatorStateInfo(0).IsName("TB Hover to Pressed"))
            {
                // do nothing because it's clicked
            }

            else
            {
                buttonAnimator.Play("TB Hover");
            }
        }
        catch
        {
            
        }

    }

    public void NormalizeButton()
    {
        try
        {
            if (buttonAnimator.GetCurrentAnimatorStateInfo(0).IsName("TB Hover to Pressed"))
            {
                // do nothing because it's clicked
            }

            else
            {
                buttonAnimator.Play("TB Hover to Normal");
            }
        }
        catch
        {
            
        }
    }
}
