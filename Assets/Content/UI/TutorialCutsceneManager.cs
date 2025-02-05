using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TutorialCutsceneManager : MonoBehaviour
{
    public Image tutorialImage;
    public Sprite[] tutorialSprites;

    private int currentIndex = 0;

    void Start()
    {
        if (tutorialSprites.Length > 0)
        {
            tutorialImage.sprite = tutorialSprites[currentIndex];
        }
    }

    public void NextImage()
    {
        if (tutorialSprites.Length == 0) return;

        if (currentIndex < tutorialSprites.Length - 1)
        {
            currentIndex++;
            tutorialImage.sprite = tutorialSprites[currentIndex];
        }
        else
        {
            SceneManager.LoadScene("Game");
        }
    }
}
