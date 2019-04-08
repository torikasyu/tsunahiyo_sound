using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;
public class GameManager : torikasyu.SingletonMonoBehaviour<GameManager>
{
    public AudioClip[] clips;

    public int YLimit { get; set; } = 7;
    public int ZLimit { get; set; } = 7;

    public float RightLimitPos { get; set; } = 15f;
    public float LeftLimitPos { get; set; } = -15f;

    public float EnemyBlockSpeed { get; set; } = 0.5f;
    public float ShootBlockSpeed { get; set; } = 15f;
    public float SpawnDistance { get; set; } = 10f;

    [SerializeField]
    GameObject enemyA;
    [SerializeField]
    GameObject DummyBlock;
    [SerializeField]
    GameObject BlockParent;
    [SerializeField]
    GameObject WallBlock;

    AudioSource audioSource;
    public bool canShoot = true;
    bool canSpawn = false;
    float distance = -1f;
    float distanceWall = 1.0f;

    public TMPro.TextMeshProUGUI ScoreText;
    public TMPro.TextMeshProUGUI HighScoreText;
    public TMPro.TextMeshProUGUI LevelText;
    public TMPro.TextMeshProUGUI InfoText;

    int score = 0;
    int level = 1;
    int exp = 0;

    int highScore;

    enum enumGameState
    {
        None,
        WaitforStart,
        Playing,
        GameOver
    }

    enumGameState gameState;
    enumGameState nextGameState;

    private float tempo = 125f;
    private float beatTime;
    public int beatCount { get; set; } = 0;

    void Start()
    {
        beatTime = (60f / tempo) / 2; // ÷2で8分音符　÷4で16分音符

        CreateWall(YLimit + 1f);
        CreateWall(-1f);
        CreateWallSide(ZLimit + 1f);
        CreateWallSide(-1f);

        gameState = enumGameState.None;
        nextGameState = enumGameState.WaitforStart;
        audioSource = GetComponent<AudioSource>();
        //audioSource.Play();
    }

    void CreateWall(float y)
    {
        var parent = new GameObject("WallParent");
        for (float x = LeftLimitPos; x <= RightLimitPos; x++)
        {
            for (float z = 0; z <= ZLimit; z++)
            {
                Instantiate(WallBlock, new Vector3(x, y, z), Quaternion.identity).transform.SetParent(parent.transform);
            }
        }
    }

    void CreateWallSide(float z)
    {
        var parent = new GameObject("SideWallParent");
        for (float x = LeftLimitPos; x <= RightLimitPos; x++)
        {
            for (float y = 0; y <= YLimit; y++)
            {
                Instantiate(WallBlock, new Vector3(x, y, z), Quaternion.identity).transform.SetParent(parent.transform);
            }
        }
    }

    public void PlaySound(int type)
    {
        audioSource.PlayOneShot(clips[type]);
    }

    void updateHighScore()
    {
        try
        {
            highScore = PlayerPrefs.GetInt("highscore");
        }
        catch
        {
            highScore = 0;
        }
    }

    bool isRunning = false;
    public void AddScore(int increment)
    {
        PlaySound(1);
        score += increment * level * 10;

        exp++;
        if (exp % 3 == 0)
        {
            level++;
            //SpawnDistance -= 1f;
            EnemyBlockSpeed += 0.15f;
        }

        ScoreText.text = "SCORE<br>" + score.ToString();
        LevelText.text = "LEVEL<br>" + level.ToString();

        if (!isRunning)
        {
            float currentSpeed = EnemyBlockSpeed;
            EnemyBlockSpeed = 0;
            StartCoroutine(SetSpeed(currentSpeed));
        }
    }

    IEnumerator SetSpeed(float speed)
    {
        isRunning = true;
        yield return new WaitForSeconds(1f);
        EnemyBlockSpeed = speed;
        isRunning = false;
    }

    public void GameOverProcess()
    {
        nextGameState = enumGameState.GameOver;
    }

    float elapsedTime = 0;
    // void FixedUpdate()
    // {
    //     if (gameState == enumGameState.Playing || gameState == enumGameState.GameOver)
    //     {
    //         elapsedTime += Time.deltaTime;
    //         beatCount = Mathf.FloorToInt(elapsedTime / beatTime);
    //         this.LevelText.text = beatCount.ToString();
    //     }
    // }
    DateTime startTime;
    TimeSpan diff;
    // Update is called once per frame
    void Update()
    {
        //仮置き
        if (gameState == enumGameState.Playing || gameState == enumGameState.GameOver)
        {
            diff = DateTime.Now - startTime;
            elapsedTime = (float)diff.TotalSeconds;
            beatCount = Mathf.FloorToInt(elapsedTime / beatTime);
            this.LevelText.text = beatCount.ToString();
        }

        switch (gameState)
        {
            case enumGameState.WaitforStart:
                if (Input.GetKeyDown(KeyCode.Return) || OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger))
                {
                    nextGameState = enumGameState.Playing;
                }
                break;
            case enumGameState.Playing:
                BlockSpawn();
                break;
            case enumGameState.GameOver:
                if (Input.GetKeyDown(KeyCode.Return) || OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger))
                {
                    //nextGameState = enumGameState.WaitforStart;
                    UnityEngine.SceneManagement.SceneManager.LoadScene("VR");
                }
                break;
            default:
                break;
        }

        switch (nextGameState)
        {
            case enumGameState.WaitforStart:
                updateHighScore();
                score = 0;
                level = 1;
                //EnemyBlockSpeed = 1.0f;


                InfoText.gameObject.SetActive(true);
                InfoText.text = "<color=yellow>TSUNAHIYO</color><br><br>TRIGGET TO START<br><br>TRIGGER : Shoot";
                ScoreText.text = "SCORE<br>" + score.ToString();
                LevelText.text = "LEVEL<br>" + level.ToString();
                HighScoreText.text = "<color=red>HIGH<br> SCORE</color><br>" + highScore.ToString();
                break;
            case enumGameState.Playing:
                InfoText.gameObject.SetActive(false);
                score = 0;
                level = 1;
                elapsedTime = 0;
                startTime = DateTime.Now;

                audioSource.Play();

                break;
            case enumGameState.GameOver:
                PlaySound(2);
                string highScroreMsg = "GAME OVER";

                if (highScore < score)
                {
                    PlayerPrefs.SetInt("highscore", score);
                    PlayerPrefs.Save();
                    highScroreMsg = "<color=red>HIGH SCORE</color>";
                }
                EnemyBlockSpeed = 0;

                InfoText.gameObject.SetActive(true);
                InfoText.text = highScroreMsg + "<br>HIT ENTER KEY";

                break;
            default:
                break;
        }

        if (nextGameState != enumGameState.None)
        {
            gameState = nextGameState;
            nextGameState = enumGameState.None;
        }

        /*
        //仮置き
        if (Input.GetMouseButtonDown(0))
        {
            var TouchPos = Input.mousePosition;
            var ray = Camera.main.ScreenPointToRay(TouchPos);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100f))
            {
                if (hit.transform.gameObject.CompareTag("Dummy"))
                {
                    if (hit.transform.gameObject.GetComponent<DummyBlock>().canDelete == true)
                    {
                        hit.transform.gameObject.GetComponent<DummyBlock>().ChangeFace();
                    }
                }
            }
        }
        */

    }

    int dummyCountMax = 3;
    int dummyCountMin = 0;

    //bool CheckDict(Dictionary<int, int> d)
    bool CheckDict(int[] d)
    {
        if (level == 1)
        {
            dummyCountMax = 1;
            dummyCountMin = 0;
        }
        else if (level == 2)
        {
            dummyCountMax = 1;
            dummyCountMin = 1;
        }
        else if (level < 5)
        {
            dummyCountMax = 2;
            dummyCountMin = 1;
        }
        else if (level < 10)
        {
            dummyCountMax = 3;
            dummyCountMin = 1;
        }
        else if (level < 20)
        {
            dummyCountMax = 3;
            dummyCountMin = 0;
        }

        int cnt = 0;
        //foreach (KeyValuePair<int, int> p in d)
        foreach (var item in d)
        {
            //if (p.Value == 0) cnt++;
            if (item == 0) cnt++;
        }

        return dummyCountMin <= cnt && cnt <= dummyCountMax;
    }

    void BlockSpawn()
    {
        if (canSpawn)
        {
            canSpawn = false;

            var parentObj = Instantiate(BlockParent, new Vector3(RightLimitPos, 0, 0), Quaternion.identity);
            int LuckCount = 0;

            //Dictionary<int, int> blockLine;
            int[] blockLine;
            //Dictionary<int, Dictionary<int, int>> blockList = new Dictionary<int, Dictionary<int, int>>();
            List<int[]> blockList = new List<int[]>();

            var currentY = YLimit;
            do
            {
                do
                {
                    //blockLine = new Dictionary<int, int>();
                    blockLine = new int[ZLimit + 1];
                    for (int i = ZLimit; i >= 0; i--)
                    {
                        //blockLine.Add(i, Random.Range(0, 10) > 1 ? 1 : 0);
                        blockLine[i] = UnityEngine.Random.Range(0, 10) > 1 ? 1 : 0;
                    }
                }
                while (!CheckDict(blockLine));

                //blockList.Add(currentZ, blockLine);
                blockList.Add(blockLine);

                currentY--;
            } while (currentY >= 0);

            /*
            List<int> numbers = new List<int>();
            Dictionary<int, int> blocks = new Dictionary<int, int>();

            for (int i = BottomLimitPos; i <= TopLimitPos; i++)
            {
                blocks.Add(i, 1);
                numbers.Add(i);
            }

            while (numbers.Count > 2)
            {
                int index = Random.Range(0, numbers.Count);
                int ransu = numbers[index];
                Debug.Log(ransu);
                blocks[ransu] = 0;
                numbers.RemoveAt(index);
            }
            */
            GameObject[] members = new GameObject[(ZLimit + 1) * (YLimit + 1)];
            GameObject tmp = null;
            int index = 0;

            //foreach (KeyValuePair<int, Dictionary<int, int>> line in blockList)
            //for (int lineNo = 0; lineNo < blockList.Count; lineNo++)
            int lineNo = YLimit;
            foreach (var line in blockList)
            {
                for (int k = line.Length - 1; k >= 0; k--)
                {
                    var value = line[k];

                    if (value == 1)
                    {
                        tmp = Instantiate(enemyA, new Vector3(RightLimitPos, lineNo, k), Quaternion.identity);
                        tmp.transform.SetParent(parentObj.transform);
                    }
                    else
                    {
                        LuckCount++;
                        tmp = Instantiate(DummyBlock, new Vector3(RightLimitPos, lineNo, k), Quaternion.identity);
                        tmp.transform.SetParent(parentObj.transform);
                    }

                    members[index] = tmp;
                    index++;
                }
                lineNo--;
            }

            if (LuckCount == 0)
            {
                Destroy(parentObj);
            }
            else
            {
                parentObj.GetComponent<BlockParent>().LuckCount = LuckCount;
                parentObj.GetComponent<BlockParent>().members = members;
            }

        }

        if (!canSpawn)
        {
            distance += EnemyBlockSpeed * Time.deltaTime;
            //print(distance);
            if (distance >= SpawnDistance)
            {
                canSpawn = true;
                distance = 0;
            }
        }
    }
}
