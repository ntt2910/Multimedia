using UnityEngine;
using UnityEngine.UI;

public class PlaySoundOnClick : MonoBehaviour
{
    //[SerializeField] private ActionClick actionClick;
    [SerializeField] private AudioClip sound;
    [SerializeField] private bool isEnableSoundToggle = true;

    private void Awake()
    {
        var button = GetComponent<Button>();
        if (button != null)
                button.onClick.AddListener(PlaySound);

        if (isEnableSoundToggle)
        {
            var toggle = GetComponent<Toggle>();
            if (toggle != null)
                toggle.onValueChanged.AddListener((v) => { PlaySound(); });
        }
    }

    public void PlaySound()
    {
        GameController.instance.musicManager.PlayOneShot(sound, MusicManager.SourceAudio.Sound);
        //AnalyticsController.LogActionClick(actionClick);
        //Debug.Log(StringHelper.StringColor(ColorString.green, gameObject.name));
    }
}
public enum NameSound
{
    //Out battle
    Click,
    
    //In Battle
    StartGame,
    Match1,
    TileHitEnemy,
    CollectMana,
    TileDragon,
    TileThunder,
    PreSpawnSkillHero,
    PreSpawnSkillBoss,
    MatchFail,
    FullManaHero,
    HeroDie,
    EnemyDie,
    BossDie,
    Victory,
    Defeat,
    Loot,
    PreSpawnBoss
}
