using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureChest : MonoBehaviour, ICanTakeDamage
{
    public int chestID = 1;
    public int rewardedCoin = 30;
    public int rewardedLive = 3;
    public Sprite changeSprite;
    public AudioClip sound;
    bool isWorked = false;

    private void Start()
    {
        if (GlobalValue.levelPlaying!=-1 && GlobalValue.isChestCollected(chestID))      //no check when testing a level
        {
            isWorked = true;
            GetComponent<Collider2D>().enabled = false;
            GetComponent<SpriteRenderer>().sprite = changeSprite;
        }
    }

    #region ICanTakeDamage implementation
    public void TakeDamage(int damage, Vector2 force, GameObject instigator, Vector3 hitPoint)
    {
        if (isWorked)
            return;

        isWorked = true;
        GetComponent<Collider2D>().enabled = false;
        SoundManager.PlaySfx(sound);
        GetComponent<SpriteRenderer>().sprite = changeSprite;

        StartCoroutine(OpenChestCo());
    }

    IEnumerator OpenChestCo()
    {
        yield return new WaitForSeconds(0.5f);
        TreasureUI.Instance.ShowRewarded(rewardedCoin, rewardedLive);

        GlobalValue.SetCheckCollected(chestID);     //prevent player earn the chest again
    }

    #endregion
}
