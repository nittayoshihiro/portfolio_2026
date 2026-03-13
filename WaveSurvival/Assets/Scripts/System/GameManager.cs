using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.Audio;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    private GameManager() { }

    public enum GameState
    {
        Title,
        Playing,
        WaveClear,
        Result
    }

    [SerializeField] private GameObject _titlePanel;
    [SerializeField] private GameObject _resultPanel;
    [SerializeField] private GameObject _wavePanel;
    [SerializeField] private GameObject _clickNextText;
    [SerializeField] private GameObject _gameUI;
    [SerializeField] private GameObject _surviveText;
    [SerializeField] private TMP_Text _scoreText;
    [SerializeField] private TMP_Text _waveText;
    [SerializeField] private TMP_Text _waveTimerText;
    [SerializeField] private TMP_Text _waveClearText;
    [SerializeField] private TMP_Text _bonusText;
    [SerializeField] private TMP_Text _resultText;
    [SerializeField] private TMP_Text _resultScoreText;
    [SerializeField] private TMP_Text _resultRankText;
    [SerializeField] private AudioClip _enemyDeathSE;
    [SerializeField] private AudioMixer _mixer;
    [SerializeField] private PlayerController _player;
    private GameState _currentState;
    public GameState CurrentGameState() { return _currentState; }
    private int _score = 0;
    private int _currentWave = 0;
    private float _waveClearInputDelay = 1.5f;
    private float _waveClearTimer = 0f;


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
        switch (_currentState)
        {
            case GameState.Title:
                break;
            case GameState.Playing:
                _waveTimerText.text = WaveManager.Instance.GetWaveTimer().ToString("F2");
                break;
            case GameState.WaveClear:
                _waveClearTimer += Time.unscaledDeltaTime;

                if (_waveClearTimer > _waveClearInputDelay)
                {
                    _clickNextText.SetActive(true);
                    if (Input.GetMouseButtonDown(0))
                    {
                        StartNextWave();
                        _clickNextText.SetActive(false);
                    }
                }
                break;
            case GameState.Result:
                break;
            default:
                break;
        }
    }

    public void ShowSurviveMessage()
    {
        _surviveText.SetActive(true);
    }

    public void WaveClear(int timeBonus)
    {
        _currentState = GameState.WaveClear;

        int clearBonus = 500;

        int hpBonus = _player.GetHP() * 50;

        int totalBonus = clearBonus + hpBonus + timeBonus;

        AddScore(totalBonus);

        _waveClearText.text = "WAVE "+_currentWave+" CLEAR";
        _bonusText.text =
            "CLEAR +" + clearBonus +
            "\nHP BONUS +" + hpBonus +
            "\nTIME BONUS +" + timeBonus;

        _wavePanel.SetActive(true);
        _waveClearTimer = 0f;

        Time.timeScale = 0.0f;
    }

    void StartNextWave()
    {

        Time.timeScale = 1f;
        _surviveText.SetActive(false);
        _wavePanel.SetActive(false);

        _currentState = GameState.Playing;

        WaveManager.Instance.StartNextWaveFromGameManager();
    }

    public void StartGame()
    {
        _currentState = GameState.Playing;

        _titlePanel.SetActive(false);
        _resultPanel.SetActive(false);
        _wavePanel.SetActive(false);
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
        _resultText.text = "Game Over";
        StartCoroutine(CountUpScore(_score));
        _resultRankText.text = "Rank  "+GetRank(_score);
        _surviveText.SetActive(false);
        _gameUI.SetActive(false);

        Time.timeScale = 0f;
    }

    public void GameClear()
    {
        _currentState = GameState.Result;

        _resultPanel.SetActive(true);
        _resultText.text = "Game Clear";
        StartCoroutine(CountUpScore(_score));
        _resultRankText.text = "Rank  " + GetRank(_score);
        _surviveText.SetActive(false);
        _gameUI.SetActive(false);

        Time.timeScale = 0f;
    }

    public void Retry()
    {
        Time.timeScale = 1f;

        // プレイヤーHPリセット
        _player.ResetPlayer();

        _score = 0;
        _scoreText.text = "Score: 0";

        _currentState = GameState.Playing;
        _currentWave = 0;
        WaveManager.Instance.ResetWave();
        WaveManager.Instance.StartWave();

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

    IEnumerator CountUpScore(int finalScore)
    {
        int displayedScore = 0;

        while (displayedScore < finalScore)
        {
            displayedScore += Mathf.Max(1, finalScore / 200);

            if (displayedScore > finalScore)
                displayedScore = finalScore;

            _resultScoreText.text = "Score: " + displayedScore.ToString("D4");

            yield return null;
        }
    }

    private string GetRank(int score)
    {
        if (score >= 4500) return "S";
        if (score >= 4000) return "A";
        if (score >= 3500) return "B";
        return "C";
    }

}
