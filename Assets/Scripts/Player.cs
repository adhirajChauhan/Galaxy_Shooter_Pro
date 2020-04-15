using System.Collections;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float _speed = 2f;
    private float _speedMultiplier = 2;
    [SerializeField]
    private GameObject _laserPrefab;
    [SerializeField]
    private GameObject _tripleShotPrefab;
    [SerializeField]
    private float _firerate = 0.5f;
    private float _canFire = -1f;
    [SerializeField]
    private int _lives = 3;
    private SpawnManager _spawnManager;
    
    private bool _isTripleShotActive = false;
    private bool _isSpeedBoostActive = false;
    private bool _isSheildActive = false;

    [SerializeField]
    private GameObject _sheildVisualizer;

    [SerializeField]
    private GameObject _leftEngine, _rightEngine;

    [SerializeField]
    private UIManager _uiManager;

    [SerializeField]
    private int _score;

    [SerializeField]
    private AudioClip _laserAudio;
    private AudioSource _audioSource;
    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(0, 0, 0);
        _spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        _audioSource = GetComponent<AudioSource>();

        if (_spawnManager == null)
        {
            Debug.LogError("Spawn manager is NULL");
        }

        if (_uiManager == null)
        {
            Debug.LogError("UI Manager is NULL");
        }

        if (_audioSource == null)
        {
            Debug.LogError("Audio Source is NULL");
        }
        else
        {
            _audioSource.clip = _laserAudio;
        }
    }

    // Update is called once per frame
    void Update() {
#if UNITY_ANDROID

        if (Input.GetKeyDown(KeyCode.Space) || CrossPlatformInputManager.GetButtonDown("Fire") && Time.time > _canFire)
        {
            FireLaser();
        }
        #endif

    
        calculateMovement();

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0) && Time.time > _canFire)
        {
            FireLaser();
        }
    }
    
    void calculateMovement()
    {
        float horizontalInput = CrossPlatformInputManager.GetAxis("Horizontal"); // Input.GetAxis("Horizontal");
        float verticalInput = CrossPlatformInputManager.GetAxis("Vertical"); // Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(horizontalInput, verticalInput, 0);

        transform.Translate(direction * _speed * Time.deltaTime);

        if (transform.position.y >= 0)
        {
            transform.position = new Vector3(transform.position.x, 0, 0);
        }
        else if (transform.position.y <= -3.8f)
        {
            transform.position = new Vector3(transform.position.x, -3.8f, 0);
        }

        if (transform.position.x < -11f)
        {
            transform.position = new Vector3(11f, transform.position.y, 0);
        }
        else if (transform.position.x > 11f)
        {
            transform.position = new Vector3(-11f, transform.position.y, 0);
        }
    }

    void FireLaser()
    {
        _canFire = Time.time + _firerate;

        if (_isTripleShotActive == true)
        {
            Instantiate(_tripleShotPrefab, transform.position + new Vector3(-0.3f, 0.5f, 0), Quaternion.identity);
        }
        else
        {
            Instantiate(_laserPrefab, transform.position + new Vector3(0, 1.05f, 0), Quaternion.identity);
        }
        _audioSource.Play();
    }

    public void Damage()
    {
        if (_isSheildActive == true)
        {
            _isSheildActive = false;
            _sheildVisualizer.SetActive(false);
            return;
        }
            _lives--;

        if (_lives == 2)
        {
            _leftEngine.SetActive(true);
        }
        else if(_lives == 1)
        {
            _rightEngine.SetActive(true);
        }

        _uiManager.UpdateLives(_lives);

            if (_lives < 1)
            {
                _spawnManager.onPlayerDeath();
                Destroy(this.gameObject);
            }
        

    }

    public void TripleShotActive()
    {
        _isTripleShotActive = true;
        StartCoroutine(TripleShotPowerDownRoutine());
    }

    IEnumerator TripleShotPowerDownRoutine()
    {
        yield return new WaitForSeconds(5.0f);
        _isTripleShotActive = false;
    }

    public void SpeedBoostActive()
    {
        _isSpeedBoostActive = true;
        _speed *= _speedMultiplier;
        StartCoroutine(SpeedBoostPowerDownRoutine());
    }

    IEnumerator SpeedBoostPowerDownRoutine()
    {
        yield return new WaitForSeconds(5f);
        _isSpeedBoostActive = false;
        _speed /= _speedMultiplier;
    }

    public void SheildActive()
    {
        _isSheildActive = true;
        _sheildVisualizer.SetActive(true);
    }

    public void AddingScore(int points)
    {
        _score += points;
        _uiManager.UpdateScore(_score);
    }

}
