using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;

public class MainManager : MonoBehaviour
{
    public Brick BrickPrefab;
    public int LineCount = 6;
    public Rigidbody Ball;

    public Text ScoreText;
    public GameObject GameOverText;
    
    private bool m_Started = false;    
    private bool m_GameOver = false;

    public Text playerNameText;
    public Text highScoreText;

    private int currentScore = 0;
    private int highScore;
    private string highScorePlayerName;



    // Start is called before the first frame update
    void Start()
    {
        LoadHighScore();

        const float step = 0.6f;
        int perLine = Mathf.FloorToInt(4.0f / step);
        
        int[] pointCountArray = new [] {1,1,2,2,5,5};
        for (int i = 0; i < LineCount; ++i)
        {
            for (int x = 0; x < perLine; ++x)
            {
                Vector3 position = new Vector3(-1.5f + step * x, 2.5f + i * 0.3f, 0);
                var brick = Instantiate(BrickPrefab, position, Quaternion.identity);
                brick.PointValue = pointCountArray[i];
                brick.onDestroyed.AddListener(AddPoint);
            }
        }

        string playerName = PlayerPrefs.GetString("PlayerName", "Guest");
        playerNameText.text = "Player: " + highScorePlayerName;

        highScore = PlayerPrefs.GetInt("HighScore", 0);
        highScorePlayerName = PlayerPrefs.GetString("HighScorePlayerName", "None");
        highScoreText.text = "High Score: " + highScore;
    }

    private void Update()
    {
        if (!m_Started)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                m_Started = true;
                float randomDirection = Random.Range(-1.0f, 1.0f);
                Vector3 forceDir = new Vector3(randomDirection, 1, 0);
                forceDir.Normalize();

                Ball.transform.SetParent(null);
                Ball.AddForce(forceDir * 2.0f, ForceMode.VelocityChange);
            }
        }
        else if (m_GameOver)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }


    void AddPoint(int point)
    {
        currentScore += point;

        if (currentScore > highScore)
        {
            highScore = currentScore;
            highScorePlayerName = PlayerPrefs.GetString("PlayerName", "Guest");
            highScoreText.text = "High Score: " + highScore;

            PlayerPrefs.SetInt("HighScore", highScore);
            PlayerPrefs.SetString("HighScorePlayerName", highScorePlayerName);
        }

        ScoreText.text = $"Score : {currentScore}";
        highScoreText.text = "High Score: " + Mathf.Max(currentScore, PlayerPrefs.GetInt("HighScore", 0));
        PlayerPrefs.SetInt("HighScore", Mathf.Max(currentScore, PlayerPrefs.GetInt("HighScore", 0)));

        SaveHighScore();
    }

    public void GameOver()
    {
        m_GameOver = true;
        GameOverText.SetActive(true);
    }

    [System.Serializable]
    public class HighScoreData
    {
        public int score;
        public string playerName;
    }
    private void LoadHighScore()
    {
        if (File.Exists("highScore.json"))
        {
            string json = File.ReadAllText("highScore.json");
            HighScoreData highScoreData = JsonUtility.FromJson<HighScoreData>(json);

            highScore = highScoreData.score;
            highScorePlayerName = highScoreData.playerName;
            highScoreText.text = "High Score: " + highScore;
        }
    }

    private void SaveHighScore()
    {
        HighScoreData highScoreData = new HighScoreData
        {
            score = highScore,
            playerName = highScorePlayerName
        };

        string json = JsonUtility.ToJson(highScoreData);
        File.WriteAllText("highScore.json", json);
    }
}
