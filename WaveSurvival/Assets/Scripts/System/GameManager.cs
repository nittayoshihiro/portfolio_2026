using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private TMP_Text _scoreText;
    [SerializeField] private TMP_Text _waveText;
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
            Destroy(gameObject); // 2Ç¬ñ⁄à»ç~Çè¡Ç∑
        }
    }

    void Start()
    {

    }


    void Update()
    {

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
}
