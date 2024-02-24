using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class Amulet : BaseMonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    GameObject[] amulets;
    [SerializeField]
    Animator[] peelOffAnimators;
    [SerializeField]
    GameObject selectMark;

    Animator peelOffAnimator;

    public bool IsTrigger { get; private set; }
    public bool IsSelect { get; set; }

    UnityAction<Amulet> OnSelect;
    UnityAction<Amulet> OnConfirm;

    public void Init(bool isTrigger, UnityAction<Amulet> onSelect, UnityAction<Amulet> onConfirm)
    {
        int index = Random.Range(0, 2);
        amulets[index].SetActive(true);
        peelOffAnimator = peelOffAnimators[index];

        IsTrigger = isTrigger;

        OnSelect = onSelect;
        OnConfirm = onConfirm;
    }

    void Update()
    {
        selectMark.SetActive(IsSelect);
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        if (!IsSelect)
            Select();
        else
            Confirm();
    }

    void Select()
    {
        
        OnSelect.Invoke(this);


    }

    void Confirm()
    {
        OnConfirm.Invoke(this);
        PeelOff();
    }

    public void PeelOff()
    {
        AudioManager.Instance.PlaySe((SE)5);
        StartCoroutine(PeelOffAnimation());
        
    }

    IEnumerator PeelOffAnimation()
    {
        peelOffAnimator.Play("Take 001");
        AnimationClip[] peelOffClips = peelOffAnimator.runtimeAnimatorController.animationClips;
        yield return new WaitForSeconds(peelOffClips[0].length);

        Destroy(this.gameObject);
    }
}
