using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameUI : MonoBehaviour {

    public Image fadePlane;
    public GameObject gameOverUI;

    public RectTransform newWaveBanner;
    public Text newWaveTitle;
    public Text newWaveEnemyCount;
    public Text scoreUI;
    public Text gameOverScoreUI;
    public Text gameOverHighScoreUI;
    public Text HighScoreFlashingTextUI;
    public Text gunAmounation;
    public Text hurtCounter;
    public Text itemDropCounter;
    public RectTransform healthBar;
    public Transform currentEquippedGun;
    public Transform currentEquippedMelee;
    public Vector3 currentEquippedMeleeRotation;
    public RectTransform informationPannel;
    public Vector2 informationPannelOffset;
    public KeyCode informationButton;
    public KeyCode quitButton;

    Spawner spawner;
    Player player;
    GunController gunController;
    KeepPlayerInLevel keepInLevel;

    Transform currnetWeaponIcon;
    Vector2 initialInformationPannelPosition;
    Vector2 informationPannelTarget;
    float informationPannelSpeed=5;
    bool isInLevel;


    void Start() {
        isInLevel = true;
        HighScoreFlashingTextUI.gameObject.SetActive(false);
        initialInformationPannelPosition = informationPannel.anchoredPosition;
        informationPannelTarget = initialInformationPannelPosition;
        player = FindObjectOfType<Player>();
        gunController = FindObjectOfType<GunController>();
        gunController.OnGunChange += OnGunSwitch;
        keepInLevel = FindObjectOfType<KeepPlayerInLevel>();
        keepInLevel.OnEnterLevel += OnEnterLevel;
        keepInLevel.OnLeaveLevel += OnLeaveLevel;
        hurtCounter.gameObject.SetActive(false);
        player.OnDeath += OnGameOver;
        MeleeIcon();
        WeaponIcon();
    }

    private void OnEnterLevel()
    {
        isInLevel = true;
        hurtCounter.gameObject.SetActive(false);
    }

    private void OnLeaveLevel()
    {
        isInLevel = false;        
    }

    private void Awake()
    {
        spawner = FindObjectOfType<Spawner>();
        spawner.OnNewWave += OnNewWave;
    }

    private void Update()
    {
        scoreUI.text = ScoreKeeping.kills + "";
        float healthPercent = 0;
        if (player != null){
            healthPercent = player.health / player.startingHealth;            
        }
        healthBar.localScale = new Vector3(healthPercent, 1, 1);
        gunAmounation.text = gunController.equippedGun.projectilesRemainingInMag + " / " + gunController.equippedGun.magazinesRemaining;

        if (!isInLevel) {
            hurtCounter.gameObject.SetActive(true);
            hurtCounter.text = keepInLevel.timer.ToString("F1");
        }

        //Show information pannel        
        float slowFactor = 3;
        if (Input.GetKeyDown(informationButton)){
            informationPannelTarget = informationPannelOffset + initialInformationPannelPosition;
            Time.timeScale /= slowFactor;
            informationPannelSpeed *= slowFactor;
        }
        else if (Input.GetKeyUp(informationButton)){
            informationPannelTarget = initialInformationPannelPosition;
            Time.timeScale *= slowFactor;
            informationPannelSpeed /= slowFactor;
        }
        if (informationPannelTarget != null)
            informationPannel.anchoredPosition = Vector2.Lerp(informationPannel.anchoredPosition, informationPannelTarget, Time.deltaTime * informationPannelSpeed);


        itemDropCounter.text = spawner.respawnerTimer.ToString("F0");

        if (Input.GetKeyDown(quitButton)) {
            player.OnDeathTrigger();
            ReturnToMenu();
        }
    }


    private void OnGunSwitch()
    {
        if (currnetWeaponIcon != null) {
            Destroy(currnetWeaponIcon.gameObject);
            WeaponIcon();
        }
        if (currnetWeaponIcon == null) print("currnetWeaponIcon == null");
        
        
    }

    void MeleeIcon()
    {
        if (player.startingMeleeWeapon != null) {
            currentEquippedMelee = Instantiate(player.startingMeleeWeapon.transform, currentEquippedMelee.position,Quaternion.identity, currentEquippedMelee);
            foreach (MeshRenderer mesh in currentEquippedMelee.GetComponentsInChildren<MeshRenderer>())
            {
                mesh.gameObject.layer = 12;
            }
            currentEquippedMelee.localScale *= 30;
            currentEquippedMelee.localRotation = Quaternion.Euler(currentEquippedMeleeRotation);
        }
    }

    void WeaponIcon(){
        if (gunController.GetEquippedGunIcon() != null) {
            currnetWeaponIcon = Instantiate(gunController.GetEquippedGunIcon(), currentEquippedGun);
            currnetWeaponIcon.localRotation = Quaternion.identity;
            currnetWeaponIcon.localPosition = Vector3.zero;
            currnetWeaponIcon.localScale *= 50;
        }     
    }

    void OnNewWave(int waveNumber) {
        string enemyCountString = (spawner.waves[waveNumber - 1].infinite) ? "Infinite" : spawner.waves[waveNumber - 1].enemyCount + "";
        string[] numbers = { "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten" };
        newWaveTitle.text = "- Wave " + numbers[waveNumber - 1] + " -";
        newWaveEnemyCount.text = "Enemies: " + enemyCountString;

        StopCoroutine("AnimateNewWaveBanner");
        StartCoroutine("AnimateNewWaveBanner");
    }

    IEnumerator AnimateNewWaveBanner() {
        float animatePercent = 0;
        float speed = 2.5f;
        float delayTime = 2;
        int dir = 1;

        float endDelayTime = Time.time + 1 / speed + delayTime;

        while (animatePercent >= 0) {
            animatePercent += Time.deltaTime * speed * dir;

            if (animatePercent >= 1) {
                animatePercent = 1;
                if (Time.time > endDelayTime) {
                    dir = -1;
                }
            }

            newWaveBanner.anchoredPosition = Vector2.up * Mathf.Lerp(-200,10,animatePercent);
            yield return null;

        }
    }

    void OnGameOver() {
        Cursor.visible = true;
        StartCoroutine(Fade(Color.clear, new Color(0,0,0, 0.985f),1));        
        healthBar.transform.parent.gameObject.SetActive(false);
        OnEnterLevel();
        itemDropCounter.gameObject.SetActive(false);
        gameOverUI.SetActive(true);
        gameOverScoreUI.text = ScoreKeeping.kills + "";
        gameOverHighScoreUI.text = ScoreKeeping.GetMostKills() + "";
        if (ScoreKeeping.IsHighestScore()){
            StartCoroutine(FlashingText());
        }
        else {

        }
    }

    IEnumerator FlashingText() {
        float flashDelay = .4f;
        while (true) {
            HighScoreFlashingTextUI.gameObject.SetActive(true);
            yield return new WaitForSeconds(flashDelay);
            HighScoreFlashingTextUI.gameObject.SetActive(false);
            yield return new WaitForSeconds(flashDelay);
        }
        

    }

    IEnumerator Fade(Color from, Color to, float time) {
        float speed = 1 / time;
        float percent = 0;
        while (percent <1) {
            percent += Time.deltaTime * speed;
            fadePlane.color = Color.Lerp(from,to,percent);
            yield return null;
        }
    }

    //UI Input
    public void StartnewGame() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name,LoadSceneMode.Single);
    }

    public void ReturnToMenu() {
        SceneManager.LoadScene("Menu", LoadSceneMode.Single);
    }
}
