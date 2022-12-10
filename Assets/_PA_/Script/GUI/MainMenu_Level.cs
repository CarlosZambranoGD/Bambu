using UnityEngine;
using System.Collections;

using UnityEngine.UI;

public class MainMenu_Level : MonoBehaviour
{
    public int levelNumber = 0;
    public bool isBossLevel = false;
    public GameObject imgLock, imgOpen, imgPass, rotateFX, bossIcon;

    public Text TextLevel;
    public bool disableStarGroup = false;
    public GameObject StarGroup;
    public GameObject Star1;
    public GameObject Star2;
    public GameObject Star3;

    void Start()
    {
        imgLock.SetActive(false);
        imgOpen.SetActive(false);
        imgPass.SetActive(false);
        rotateFX.SetActive(false);
        bossIcon.SetActive(isBossLevel);
        TextLevel.gameObject.SetActive(!isBossLevel);

        if (GetComponent<Animator>())
            GetComponent<Animator>().enabled = levelNumber == GlobalValue.LevelHighest;
        GetComponent<Button>().interactable = levelNumber <= GlobalValue.LevelHighest;

        if (levelNumber <= GlobalValue.LevelHighest)
        {

            TextLevel.gameObject.SetActive(true);
            TextLevel.text = levelNumber.ToString();
            if (!disableStarGroup)
                StarGroup.SetActive(true);

            imgOpen.SetActive(levelNumber == GlobalValue.LevelHighest);
            rotateFX.SetActive(levelNumber == GlobalValue.LevelHighest);
            imgPass.SetActive(levelNumber < GlobalValue.LevelHighest);
        }
        else
        {
            TextLevel.gameObject.SetActive(false);
            imgLock.SetActive(true);

            StarGroup.SetActive(false);
        }

        CheckStars();
    }

    private void CheckStars()
    {
        Star1.SetActive(GlobalValue.IsScrollLevelAte(levelNumber, 1));
        Star2.SetActive(GlobalValue.IsScrollLevelAte(levelNumber, 2));
        Star3.SetActive(GlobalValue.IsScrollLevelAte(levelNumber, 3));

        if (!disableStarGroup)
            StarGroup.SetActive(Star1.activeInHierarchy || Star2.activeInHierarchy || Star3.activeInHierarchy);
    }

    public void LoadScene()
    {
        GlobalValue.levelPlaying = levelNumber;
        MainMenuHomeScene.Instance.LoadScene("Level " + GlobalValue.levelPlaying);
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
        {
            bossIcon.SetActive(isBossLevel);
            TextLevel.gameObject.SetActive(!isBossLevel);
            TextLevel.text = levelNumber + "";
        }
    }
}