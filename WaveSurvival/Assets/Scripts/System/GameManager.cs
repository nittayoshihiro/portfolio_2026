using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public enum GameState
    {
        Title,
        Playing,
        Result
    }

    [SerializeField] private GameObject _titlePanel;
    [SerializeField] private GameObject _resultPanel;
    [SerializeField] private GameObject _gameUI;
    [SerializeField] private TMP_Text _scoreText;
    [SerializeField] private TMP_Text _waveText;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _enemyDeathSE;
    private GameState _currentState;
    public GameState CurrentGameState() { return _currentState; }
    private int _score = 0;
    private int _currentWave = 1;
   


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // 2つ目以降を消す
        }
    }

    void Start()
    {

    }


    void Update()
    {

    }

    public void StartGame()
    {
        _currentState = GameState.Playing;

        _titlePanel.SetActive(false);
        _resultPanel.SetActive(false);
        _gameUI.SetActive(true);

        Time.timeScale = 1f;
    }

    public void AddScore(int amount)
    {
        _score += amount;
        _scoreText.text = "Score: " + _score;
    }

    public void NextWave()
    {
        _currentWave++;
        _waveText.text = "Wave: " + _currentWave;
    }

    public void GameOver()
    {
        _currentState = GameState.Result;

        _resultPanel.SetActive(true);
        _gameUI.SetActive(false);

        Time.timeScale = 0f;
    }

    public void Retry()
    {
        Time.timeScale = 1f;

        // 敵全削除
        foreach (var enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            Destroy(enemy);
        }

        // プレイヤーHPリセット
        PlayerController player = FindObjectOfType<PlayerController>();
        player.ResetPlayer();

        _score = 0;
        _scoreText.text = "Score: 0";

        _currentState = GameState.Playing;

        _resultPanel.SetActive(false);
        _gameUI.SetActive(true);
    }

    public void PlayEnemyDeath()
    {
        _audioSource.PlayOneShot(_enemyDeathSE);
    }

}
