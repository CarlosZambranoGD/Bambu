using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportPoint : MonoBehaviour {
	public bool use = true;
	public Teleport Teleport;

    private void Awake()
    {
		if (!use)
			gameObject.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D other){
		if (!GameManager.Instance.Player.isPlaying)
			return;
		
		if (other.GetComponent<Player> ()) {
			Teleport.TeleportPlayer (transform.position);
			return;
		}
	}
}
