using UnityEngine;
using UnityEngine.UI;

public class IAPBox : BaseBox
{
    private static GameObject instance;
    [SerializeField] private Text notify;
    [SerializeField] private Button oKBtn;
    
    protected override void Awake()
    {
        oKBtn.onClick.AddListener(OnClickOkBtn);
    }

    private void OnClickOkBtn()
    {
        base.Hide();
    }
    
    public override void Show()
    {
        base.Show();
        if (DataManager.RemoveAds > 0)
        {
            Debug.Log("Success");
            notify.text = "Purchase Successful";
        }
        else
        {
            Debug.Log("Fail");
            notify.text = "Purchase Failed";
        }
    }
    
    public static IAPBox Setup()
    {
        if (instance == null)
        {
            instance = Instantiate(Resources.Load(PathPrefabs.IAP_BOX) as GameObject);
        }
        instance.SetActive(true);

        return instance.GetComponent<IAPBox>();
    }
}
