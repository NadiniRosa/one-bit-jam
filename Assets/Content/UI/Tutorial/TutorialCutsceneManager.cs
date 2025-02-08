using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class TutorialCutsceneManager : MonoBehaviour
{
    [Header("Tutorial Images")]
    public Image tutorialImage;
    public Sprite[] tutorialSprites;

    [Header("Tutorial Texts")]
    public TMP_Text tutorialText;
    public string[] tutorialTexts;

    public float typingSpeed = 0.05f;
    private int currentIndex = 0;
    private bool isTyping = false;

    void Start()
    {
        if (tutorialSprites.Length > 0)
        {
            tutorialImage.sprite = tutorialSprites[currentIndex];
            StartCoroutine(TypeText());
        }
    }

    public void NextImage()
    {
        if (isTyping)
        {
            StopAllCoroutines();
            tutorialText.text = tutorialTexts[currentIndex];
            isTyping = false;
            return;
        }

        if (tutorialSprites.Length == 0 || tutorialTexts.Length == 0) return;

        if (currentIndex < tutorialSprites.Length - 1)
        {
            currentIndex++;
            tutorialImage.sprite = tutorialSprites[currentIndex];

            StopAllCoroutines();
            StartCoroutine(TypeText());
        }
        else
        {
            SceneManager.LoadScene("Game");
        }
    }

    private IEnumerator TypeText()
    {
        isTyping = true;
        tutorialText.text = "";
        string fullText = tutorialTexts[currentIndex];

        foreach (char letter in fullText)
        {
            tutorialText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }
}
