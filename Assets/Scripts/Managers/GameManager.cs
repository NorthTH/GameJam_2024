using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : BaseMonoBehaviour
{
    [SerializeField]
    GameObject talismanPrefab;
    [SerializeField]
    TextMeshProUGUI playerNoText;
    [SerializeField]
    TextMeshProUGUI turnText;
    [SerializeField]
    Button player1Skill1Btn;
    [SerializeField]
    Button player2Skill1Btn;
    [SerializeField]
    Animator openingAnimator;
    [SerializeField]
    Animator doorOpenAnimator;
    [SerializeField]
    Animator womenAnimator;
    
    [SerializeField]
    Animator endiingAnimator;
    [SerializeField]
    GameObject skillPanel;
    [SerializeField]
    Button okBtn;
    [SerializeField]
    Button cancelBtn;

    List<Player> playerList;
    const int MaxPlayer = 2;

    List<Amulet> talismanList;
    const int MaxTalisman = 24;

    int currentTurn = 0;

    bool skillDeciding = false;
    bool skillCancel = false;

    enum Phase { None, GameStart, BeginTurn, PlayerMove, Confirm, Judge, Switch, TurnShift, GameOver}
    Phase phase = Phase.None;

    public enum Skill {  AddPickCount }

    Player currentPlayer;
    int playerIndex;
    Amulet currentTalisman;


    // Start is called before the first frame update
    void Start()
    {
        playerList = new List<Player>();

        var Player1 = new Player(1, UseSkill, player1Skill1Btn);
        var Player2 = new Player(2, UseSkill, player2Skill1Btn);

        playerList.Add(Player1);
        playerList.Add(Player2);

        player1Skill1Btn.onClick.AddListener(() => Player1.UseSkill1());
        player2Skill1Btn.onClick.AddListener(() => Player2.UseSkill1());

        okBtn.onClick.AddListener(() =>
        {
            skillCancel = false;
            skillDeciding = false;
        });
        cancelBtn.onClick.AddListener(() => 
        {
            skillCancel = true;
            skillDeciding = false;
        });


        StartCoroutine(RandomSe());
        SetPhase(Phase.GameStart);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
    }

    IEnumerator RandomSe()
    {
        while(true)
        {
            float Interval = Random.Range(10f, 20f);
            yield return new WaitForSeconds(Interval);

            int se = Random.Range(8, 11);
            AudioManager.Instance.RandomSe((SE)se);
        }
    }

    void Init()
    {
        skillPanel.SetActive(false);
        currentTurn = 0;
        doorOpenAnimator.Play("Idle");

        playerList.ForEach(x => x.Init());

        currentPlayer = playerList[0];


        if (talismanList != default)
        {
            talismanList.ForEach(x => Destroy(x.gameObject));
        }

        talismanList = new ();

        int patarnNo = Random.Range(0, 8);
        var patarn = GetRandomPosPatarn(patarnNo);

        var pos = new Vector3 (Random.Range(patarn.MinX, patarn.MaxX), Random.Range(patarn.MinY, patarn.MaxY), patarn.Z);
        Quaternion rotation = Quaternion.Euler(0, 180, Random.Range(-30, 30));
        var talisman = InstantiateObj<Amulet>(talismanPrefab, pos, rotation);
        talisman.Init(true, OnSelect, OnConfirm);
        talismanList.Add(talisman);
        patarnNo++;

        for (int i = 1; i < MaxTalisman; i++)
        {
            patarn = GetRandomPosPatarn(patarnNo);
            rotation = Quaternion.Euler(0, 180, Random.Range(-30, 30));
            pos = new Vector3(Random.Range(patarn.MinX, patarn.MaxX), Random.Range(patarn.MinY, patarn.MaxY), patarn.Z);
            var talismanDupi = InstantiateObj<Amulet>(talismanPrefab, pos, rotation);
            talismanDupi.Init(false, OnSelect, OnConfirm);
            talismanList.Add(talismanDupi);

            patarnNo++;
        }

        RandomPosPatarn GetRandomPosPatarn(int no)
        {
            patarnNo = no % 8;
            patarn = new RandomPosPatarn(patarnNo);
            return patarn;
        }
    }

    void SetPhase(Phase nextPhase)
    {
        phase = nextPhase;
        switch (phase)
        {
            case Phase.GameStart:
                GameStart();
                break;
            case Phase.BeginTurn:
                Begin();
                break;
            case Phase.PlayerMove:
                PlayerMove();
                break;
            case Phase.Confirm:
           
                break;
            case Phase.Judge:
                Judge();
                break;
            case Phase.Switch:
                Switch();
                break;
            case Phase.TurnShift:
                TurnShift();
                break;
            case Phase.GameOver:
                GameOver();
                break;
        }   
    }

    void GameStart()
    {
        AudioManager.Instance.PlayBgm((BGM)1);
        Init();

        player1Skill1Btn.gameObject.SetActive(false);
        player2Skill1Btn.gameObject.SetActive(false);

        endiingAnimator.Play("Idle");
        womenAnimator.Play("Idle");

        StartCoroutine(Opening());

    }

    IEnumerator Opening()
    {
        openingAnimator.Play("");
        AnimationClip[] openingClips = openingAnimator.runtimeAnimatorController.animationClips;
        yield return new WaitForSeconds(openingClips[0].length);

        SetPhase(Phase.TurnShift);
    }

    void Begin()
    {
        currentPlayer.SetEnebleSkillButton(false);
        currentPlayer = playerList[playerIndex];
        currentPlayer.SetEnebleSkillButton(true);
        Debug.Log($"CurrentPlayer:{currentPlayer.PlayerNo}");
        playerNoText.text = ($"Player : {currentPlayer.PlayerNo}");

        SetPhase(Phase.PlayerMove);
    }

    void PlayerMove()
    {
        Debug.Log($"Player {currentPlayer.PlayerNo} PickCount : {currentPlayer.PickCount}");
    }

    void UseSkill(Skill skill)
    {
        AudioManager.Instance.PlaySe((SE)2);

        skillDeciding = true;
        skillPanel.SetActive(false);
        StartCoroutine(OpenSkillPanel());

        IEnumerator OpenSkillPanel()
        {
            skillPanel.SetActive(true);
            yield return new WaitUntil(() => !skillDeciding);
            skillPanel.SetActive(false);

            if (skillCancel)
            {
                yield return null;
            }
            else
            {
                AudioManager.Instance.PlaySe((SE)4);
                currentPlayer.SetSkillUsed();
                switch (skill)
                {
                    case Skill.AddPickCount:
                        AddPickCountToNextPlayer();
                        break;
                }
            }
        }
    }

    void AddPickCountToNextPlayer()
    {
        var nextPlyerIndex = playerIndex + 1;
        nextPlyerIndex = (nextPlyerIndex >= playerList.Count) ? 0 : nextPlyerIndex;
        var nextPlyer = playerList[nextPlyerIndex];
        nextPlyer.AddPickCount(3);
        Debug.Log($"Player {nextPlyer.PlayerNo} is AddPickCount");
    }

    void OnSelect(Amulet talisman)
    {
        if (phase != Phase.PlayerMove)
            return;
        AudioManager.Instance.PlaySe((SE)1);
        Debug.Log($"{talisman.IsTrigger} is Selecting ");
        talismanList.ForEach(x => x.IsSelect = false);
        talisman.IsSelect = true;
    }

    void OnConfirm(Amulet talisman)
    {
        currentTalisman = talisman;
        talismanList.Remove(talisman);

        currentPlayer.Pick(talisman);
        SetPhase(Phase.Judge);
    }

    void Judge()
    {
   
        Debug.Log($"{currentTalisman.IsTrigger} is Selected ");
        StartCoroutine(JudgeCoroutine());
    }

    IEnumerator JudgeCoroutine()
    {
        yield return new WaitForSeconds(0.75f);

        if(talismanList.Count == 16)
        {
            AudioManager.Instance.PlayBgm((BGM)2);
        }
        else if (talismanList.Count == 8)
        {
            AudioManager.Instance.PlayBgm((BGM)3);
        }
        else if (talismanList.Count == 4)
        {
            playerList.ForEach(x => x.SetSkillUsed());
        }

        if (currentPlayer.IsDead)
            SetPhase(Phase.GameOver);
        else
        {
            if (currentPlayer.PickCount <= 0)
            {
                currentPlayer.ResetPickCount();
                SetPhase(Phase.Switch);
            }
            else
            {
                SetPhase(Phase.PlayerMove);
            }
        }
    }

    void Switch()
    {
        playerIndex++;

        if(playerIndex == playerList.Count - 1)
        {
            SetPhase(Phase.BeginTurn);
        }
        else
        {
            SetPhase(Phase.TurnShift);
        }
    }

    void TurnShift()
    {
        playerIndex = 0;
        currentTurn++;

        Debug.Log($"Turn : {currentTurn}");
        turnText.text = ($"Turn : {currentTurn}");
        AudioManager.Instance.PlaySe((SE)3);

        SetPhase(Phase.BeginTurn);
    }

    void GameOver()
    {
        Debug.Log($"Player {currentPlayer.PlayerNo} is GameOver");
        talismanList.ForEach(x => x.PeelOff());
        player1Skill1Btn.gameObject.SetActive(false);
        player2Skill1Btn.gameObject.SetActive(false);

        StartCoroutine(GameOverCoroutine());
    }

    IEnumerator GameOverCoroutine()
    {
        AudioManager.Instance.StopBGM();
        yield return new WaitForSeconds(2f);
        doorOpenAnimator.Play("Take 001");
        AudioManager.Instance.PlaySe((SE)6);
        AnimationClip[] doorClips = doorOpenAnimator.runtimeAnimatorController.animationClips;
        yield return new WaitForSeconds(doorClips[0].length);

        womenAnimator.Play("Take 001");
        AudioManager.Instance.RandomSe((SE)11);
        AnimationClip[] wowenClips = doorOpenAnimator.runtimeAnimatorController.animationClips;
        yield return new WaitForSeconds(6.5f);

        AudioManager.Instance.RandomSe((SE)12);
        yield return new WaitForSeconds(4.5f);

        endiingAnimator.Play("Ending");
        AnimationClip[] endiingClips = endiingAnimator.runtimeAnimatorController.animationClips;
        yield return new WaitForSeconds(endiingClips[0].length);
        yield return new WaitForSeconds(5);

        talismanList = default;
        SetPhase(Phase.GameStart);
        endiingAnimator.Play("Idle");
        womenAnimator.Play("Idle");
    }
}

public class RandomPosPatarn
{
    public float MaxX, MinX;
    public float MaxY, MinY;
    public float Z;

    public RandomPosPatarn(int patarn)
    {
        switch(patarn)
        {
            case 0:
                MinX = -0.30f;
                MaxX = -0.15f;
                MinY = -0.15f;
                MaxY = 0f;
                Z = -0.02f;
                break;
            case 1:
                MinX = -0.30f;
                MaxX = -0.15f;
                MinY = 0;
                MaxY = 0.15f;
                Z = -0.02f;
                break;
            case 2:
                MinX = -0.15f;
                MaxX = 0f;
                MinY = -0.15f;
                MaxY = 0f;
                Z = -0.02f;
                break;
            case 3:
                MinX = -0.15f;
                MaxX = 0f;
                MinY = 0;
                MaxY = 0.15f;
                Z = -0.02f;
                break;
            case 4:
                MinX = 0f;
                MaxX = 0.15f;
                MinY = -0.15f;
                MaxY = 0f;
                Z = -0.02f;
                break;
            case 5:
                MinX = 0f;
                MaxX = 0.15f;
                MinY = 0;
                MaxY = 0.15f;
                Z = -0.02f;
                break;
            case 6:
                MinX = 0.15f;
                MaxX = 0.30f;
                MinY = -0.15f;
                MaxY = 0f;
                Z = -0.02f;
                break;
            case 7:
                MinX = 0.15f;
                MaxX = 0.30f;
                MinY = 0;
                MaxY = 0.15f;
                Z = -0.02f;
                break;
        }
    }
}
