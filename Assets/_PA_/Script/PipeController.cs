using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeController : MonoBehaviour
{
    public bool use = true;
    public Transform nextPoint;
    public LayerMask playerLayer;
    public AudioClip soundIn, soundOut;

    private void Awake()
    {

        if (!use)
            Destroy(this);
    }

    private void Start()
    {

        ControllerInput.inputEvent += ControllerInput_inputEvent;
    }

    private void ControllerInput_inputEvent(Vector2 direction)
    {
        if (direction.y == -1)
        {
            var hit = Physics2D.CircleCast(transform.position, 0.1f, direction * -1, 2, playerLayer);
            //if (Physics.SphereCast(transform.position, 0.1f, direction * -1, out hit, 2, playerLayer))
            if (hit.collider !=null)
            {
                if (Mathf.Abs(hit.collider.gameObject.transform.position.x - transform.position.x) <= 0.4f)
                {
                    GameManager.Instance.Player.transform.position = (new Vector2(transform.position.x, hit.collider.transform.position.y));
                    StartCoroutine(ActionCo());
                }
            }
        }
    }

    private void OnDisable()
    {
        ControllerInput.inputEvent -= ControllerInput_inputEvent;
    }

    IEnumerator ActionCo()
    {
        SoundManager.PlaySfx(soundIn);
        ControllerInput.Instance.ShowController(false);
        GameManager.Instance.SetGameState(GameManager.GameState.Waiting);
        GameManager.Instance.Player.SetInThePipe(true, Vector2.down);

        yield return new WaitForSeconds(1f);
        GameManager.Instance.Player.SetInThePipe(false, Vector2.zero);
        GameManager.Instance.Player.transform.position = nextPoint.position;
        SoundManager.PlaySfx(soundOut);
        yield return new WaitForSeconds(0.5f);
        GameManager.Instance.SetGameState(GameManager.GameState.Playing);
        ControllerInput.Instance.ShowController(true);
    }

    private void OnDrawGizmos()
    {
        if (!use || Application.isPlaying)
            return;

        Gizmos.color = Color.white;
        nextPoint.gameObject.SetActive(true);
        Gizmos.DrawLine(transform.position, nextPoint.position);
    }
}
