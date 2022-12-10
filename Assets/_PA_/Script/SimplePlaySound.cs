using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimplePlaySound : MonoBehaviour
{
    public AudioClip sound;
    private void Start()
    {
        SoundManager.PlaySfx(sound);
    }
}
