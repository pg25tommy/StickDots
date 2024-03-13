using UnityEngine;
using System.Collections;

public class EmojiMenu : MonoBehaviour
{
    [SerializeField] private GameObject[] buttonList;
    [SerializeField] private float activationInterval = 0.5f;

    public void ButtonPopup()
    {
        StartCoroutine(ActivateButtonsWithInterval());
    }

    // Time interval between button activation (for animations)
    private IEnumerator ActivateButtonsWithInterval()
    {
        if (buttonList != null)
        {
            foreach (GameObject button in buttonList)
            {
                if (button != null)
                {
                    button.SetActive(!button.activeSelf);
                    yield return new WaitForSeconds(activationInterval);
                }
            }
        }
    }
}
