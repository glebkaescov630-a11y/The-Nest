using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StoryManager : MonoBehaviour
{
    public Sprite[] storyImages; 
    public Image displayImage;   
    private int currentIndex = 0;

    void Start()
    {
        displayImage.sprite = storyImages[currentIndex];
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
        {
            ShowNextImage();
        }
    }

    void ShowNextImage()
    {
        currentIndex++;

        if (currentIndex < storyImages.Length)
        {
            displayImage.sprite = storyImages[currentIndex];
        }
        else
        {
            SceneManager.LoadScene("SampleScene");
        }
    }
}