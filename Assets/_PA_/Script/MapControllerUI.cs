using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MapControllerUI : MonoBehaviour {
	public RectTransform BlockLevel;
	public int howManyBlocks = 3;
	public float step = 720f;
    public int levelPerBlock = 6;
	public Image[] Dots;
	private float newPosX = 0;

	int currentPos = 0;
	public AudioClip music;

    public Button btnNext, btnPre;

    void OnEnable()
    {
        if (SoundManager.Instance)
            SoundManager.PlayMusic(music);
        Debug.LogWarning("ON ENALBE");

        UpdateLevelInfor();

        if (GlobalValue.LevelHighest != 999)     // 999 mean use Unlock All level test
        {
            if ((GlobalValue.LevelHighest % levelPerBlock) == 0)
                SetCurrentWorld(Mathf.Clamp((GlobalValue.LevelHighest / (levelPerBlock)), 0, howManyBlocks));
            else
                SetCurrentWorld(Mathf.Clamp((GlobalValue.LevelHighest / (levelPerBlock)) + 1, 0, howManyBlocks));
        }
    }
   
    void Start () {
        SetDots();
    }

    void SetDots()
    {
        foreach(var obj in Dots)
        {
            obj.color = new Color(1, 1, 1, 0.5f);
            obj.rectTransform.sizeDelta = new Vector2(28, 28);
        }
        
        Dots[currentPos].color = Color.yellow;
        Dots[currentPos].rectTransform.sizeDelta = new Vector2(38, 38);

        btnNext.interactable = currentPos < howManyBlocks - 1;
        btnPre.interactable = currentPos > 0;
    }

    public void SetCurrentWorld(int world)
    {
        currentPos = (world - 1);
        newPosX = 0;
        newPosX -= step * (world - 1);
        newPosX = Mathf.Clamp(newPosX, -step * (howManyBlocks - 1), 0);

        SetMapPosition();
        SetDots();
    }

    public void SetMapPosition()
    {
        BlockLevel.anchoredPosition = new Vector2(newPosX, BlockLevel.anchoredPosition.y);
    }

    bool allowPressButton = true;
    public void Next()
    {
        if (allowPressButton)
        {
            StartCoroutine(NextCo());
        }
    }

    IEnumerator NextCo()
    {
        allowPressButton = false;

        SoundManager.Click();

        if (newPosX != (-step * (howManyBlocks - 1)))
        {
            currentPos++;

            newPosX -= step;
            newPosX = Mathf.Clamp(newPosX, -step * (howManyBlocks - 1), 0);
            
        }
        else
        {
            allowPressButton = true;
            yield break;
        }

        BlackScreenSprite.instance.Show(0.1f);

        yield return new WaitForSeconds(0.1f);
        SetMapPosition();
        BlackScreenSprite.instance.Hide(0.1f);

        SetDots();

        allowPressButton = true;

    }

    public void Pre()
    {
        if (allowPressButton)
        {
            StartCoroutine(PreCo());
        }
    }

    IEnumerator PreCo()
    {
        allowPressButton = false;
        SoundManager.Click();
        if (newPosX != 0)
        {
            currentPos--;

            newPosX += step;
            newPosX = Mathf.Clamp(newPosX, -step * (howManyBlocks - 1), 0);


        }
        else
        {
            allowPressButton = true;
            yield break;
        }

        BlackScreenSprite.instance.Show(0.1f);

        yield return new WaitForSeconds(0.1f);
        SetMapPosition();
        BlackScreenSprite.instance.Hide(0.1f);

        SetDots();

        allowPressButton = true;
    }

	public void UnlockAllLevels(){
		GlobalValue.LevelHighest = (GlobalValue.LevelHighest + 1000);
		UnityEngine.SceneManagement.SceneManager.LoadScene (UnityEngine.SceneManagement.SceneManager.GetActiveScene ().buildIndex);
		SoundManager.Click ();
	}

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
        {
            UpdateLevelInfor();
        }
    }

    void UpdateLevelInfor()
    {
        var levels = transform.GetComponentsInChildren<MainMenu_Level>();
        for (int i = 1; i <= levels.Length; i++)
        {
            levels[i - 1].levelNumber = i;
            levels[i - 1].gameObject.name = "Level " + i;
        }

        GlobalValue.totalLevels = levels.Length;
        Debug.Log("Total level = " + GlobalValue.totalLevels);
    }
}
