using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationsScript : MonoBehaviour
{
    [SerializeField] List<AnimationClip> anims;

    GameObject _canvasObj;
    Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _canvasObj = gameObject;
    }

    private void OnEnable()
    {
        _animator.Play(anims[1].name);
    }

    public void CloseWindow()
    {
        StartCoroutine(ShowCloseAnim());
    }

    IEnumerator ShowCloseAnim()
    {
        _animator.Play(anims[0].name);
        yield return new WaitForSeconds(1f);
        _canvasObj.SetActive(false);
    }
}
