using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemType : MonoBehaviour
{
    public enum Type { coin, bullet, hearth}
    public Type itemType;
    public int amount = 1;
    [Range(0, 1)]
    public float soundVol = 0.8f;
    public AudioClip sound;

    [Header("OPTION")]
    public bool gravity = false;
    public float timeLiveAfterSpawned = 6;
    public Vector2 forceSpawn = new Vector2(-5, 5);

    [Header("*** COLLECT FX ***")]
    public GameObject coinCollectedFX;
    public GameObject heartCollectedFX;
    public GameObject bulletCollectedFX;

    Rigidbody2D rig;
    bool isCollected = false;
    bool allowCollect = false;

    public void Init(bool useGravity, Vector2 pushForce)
    {
        gravity = useGravity;
        if (pushForce != Vector2.zero)
            forceSpawn = pushForce;
    }

    IEnumerator Start()
    {
        if (gravity)
        {
            var rig = gameObject.AddComponent<Rigidbody2D>();
            rig.velocity = new Vector2(Random.Range(-forceSpawn.x, forceSpawn.x), forceSpawn.y);
            rig.fixedAngle = true; 
            GetComponent<Collider2D>().isTrigger = false;
            yield return new WaitForSeconds(0.1f);

            while(rig.velocity.y > 0) { yield return null; }
            allowCollect = true;
            yield return new WaitForSeconds(timeLiveAfterSpawned);
            Destroy(gameObject);
        }
        else
        {
            GetComponent<Collider2D>().isTrigger = true;
            allowCollect = true;
        }
    }

    //called by Player
    public void Collect()
    {
        if (!allowCollect || isCollected)
            return;

        isCollected = true;

        switch (itemType)
        {
            case Type.coin:
                if (coinCollectedFX != null)
                    SpawnSystemHelper.GetNextObject(coinCollectedFX, true, transform.position);
                GlobalValue.SavedCoins += amount;
                break;
            case Type.bullet:
                if (bulletCollectedFX != null)
                    SpawnSystemHelper.GetNextObject(bulletCollectedFX, true, transform.position);
                GameManager.Instance.AddBullet(amount);
                break;
            case Type.hearth:
                if (heartCollectedFX != null)
                    SpawnSystemHelper.GetNextObject(heartCollectedFX, true, transform.position);
                GameManager.Instance.Player.GiveHealth(amount, gameObject);
                break;
        }

        SoundManager.PlaySfx(sound, soundVol);

        Destroy(gameObject);
    }
}
