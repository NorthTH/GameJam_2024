using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Player
{
    public int PlayerNo { get; private set; }
    public int PickCount => pickCount;
    const int InitPickCount = 1;
    int pickCount;

    public bool IsDead { get; private set; }
    bool isSkillUsed = false;

    UnityAction<GameManager.Skill> UseSkill;
    Button SkillButton;

    public Player(int no, UnityAction<GameManager.Skill> useSkill, Button skillButton)
    {
        PlayerNo = no;
        pickCount = InitPickCount;

        UseSkill = useSkill;
        SkillButton = skillButton;
    }

    public void Init()
    {
        IsDead = false;
        pickCount = InitPickCount;
        isSkillUsed = false;
        SkillButton.interactable = true;
    }

    public void ResetPickCount()
    {
        pickCount = InitPickCount;
    }

    public void AddPickCount(int count)
    {
        pickCount = count;
        Debug.Log( $"PickCount : {pickCount}");
    }

    public void Pick(Amulet talisman)
    {
        pickCount--;
        if (talisman.IsTrigger)
            IsDead = true;
    }

    public void UseSkill1()
    {
        if (isSkillUsed)
            return;
        UseSkill.Invoke(GameManager.Skill.AddPickCount);
        
    }

    public void SetSkillUsed()
    {
        SkillButton.interactable = false;
        isSkillUsed = true;
    }

    public void SetEnebleSkillButton(bool value)
    {
        SkillButton.gameObject.SetActive(value);
    }

}
