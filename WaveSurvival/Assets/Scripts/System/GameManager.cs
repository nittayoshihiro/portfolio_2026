using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.Audio;

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
    [SerializeField] private AudioClip _enemyDeathSE;
    [SerializeField] private AudioMixer _mixer;
    [SerializeField] private PlayerController _player;
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
        _player.ResetPlayer();

        _score = 0;
        _scoreText.text = "Score: 0";

        _currentState = GameState.Playing;

        _resultPanel.SetActive(false);
        _gameUI.SetActive(true);
    }

    public void PlayEnemyDamage()
    {

        SEManager.Instance.PlaySE(_enemyDeathSE);
    }

    public void SetBGMVolume(float value)
    {
        _mixer.SetFloat("BGMVolume", Mathf.Log10(value) * 20);
    }

    public void SetSEVolume(float value)
    {
        _mixer.SetFloat("SEVolume", Mathf.Log10(value) * 20);
    }

}
