using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MatchingItem : MonoBehaviour
{
    [SerializeField] private Image imgLoading;
    [SerializeField] private RawImage imgIconCountry;
    [SerializeField] private TextMeshProUGUI txtName;

    private void Start()
    {
        imgLoading.gameObject.SetActive(true);
        imgIconCountry.gameObject.SetActive(false);
        txtName.text = "Waiting...";
    }

    public void Init(FishPlayerData fishPlayerData)
    {
        if (fishPlayerData == null)
        {
            Debug.LogError("MatchingItem Init null");
            return;
        }
        
        imgLoading.gameObject.SetActive(false);
        var texture = Resources.Load<Texture2D>($"flags/{fishPlayerData.countryCode}");
        if (texture == null)
        {
            Debug.LogError("texture null " + fishPlayerData.countryCode);
            return;
        }
        imgIconCountry.texture = texture;
        imgIconCountry.SetNativeSize();
        imgIconCountry.gameObject.SetActive(true);
        txtName.text = fishPlayerData.name;
    }
}