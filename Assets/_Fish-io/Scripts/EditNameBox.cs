using LTABase.DesignPattern;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class EditNameBox : MonoBehaviour
{
     private static GameObject instance;
    public UnityAction moreActionOff;
    private UnityAction _actionHide;
    [SerializeField] private InputField inputName;
    //[SerializeField] private Image imgCheck;
    [SerializeField] private Button btDone;
    [SerializeField] private Material mtGray;
    [SerializeField] private Sprite[] sprCheck;

    protected void Start()
    {
        inputName.text = DataManager.playerName;
        btDone.onClick.AddListener(OnClickDone);
        inputName.onEndEdit.AddListener(val =>
        {
            // TouchScreenKeyboard.Status.Done: Keyboard disappeared when something like "Done" button in mobilekeyboard
            // TouchScreenKeyboard.Status.Canceled: Keyboard disappeared when "Back" Hardware Button Pressed in Android
            // TouchScreenKeyboard.Status.LostFocus: Keyboard disappeared when some area except keyboard and input area
#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.Return))
            {
                
                Debug.Log("Keyboard is Disappear pressed by Return or Done mobilekeyboard button");
                if (string.IsNullOrEmpty(inputName.text)) DataManager.playerName = "Player";
                else
                {
                    DataManager.playerName = inputName.text;
                }    
                
                Observer.Instance.Notify(ObserverName.ON_CHANGE_NAME);
            }
#else
            if (inputName.touchScreenKeyboard.status == TouchScreenKeyboard.Status.Done)
            {
                Debug.Log("Keyboard is Disappear pressed by Return or Done mobilekeyboard button");
                if (string.IsNullOrEmpty(inputName.text)) DataManager.playerName = "Player";
                else
                {
                    DataManager.playerName = inputName.text;
                }
                Observer.Instance.Notify(ObserverName.ON_CHANGE_NAME);
            }
#endif
        });
    }

    protected void OnStart()
    {
        Init();
    }

    private void Init()
    {
        //inputName.text = DataManager.NamePlayer;
        //imgCheck.sprite = sprCheck[0];
    }
    
    public void OnValueChangeInputName()
    {
        var str = inputName.text;
        inputName.text = Regex.Replace(str, @"[^0-9a-zA-Z_]+", string.Empty);
        //var imgButtonDone = btDone.GetComponent<Image>();
        bool isValid = !string.IsNullOrEmpty(inputName.text) && str.Length >= 5 && str.Length <= 25;
        //if(imgButtonDone != null) imgButtonDone.material = isValid ? null : mtGray;
        //imgCheck.sprite = isValid ? sprCheck[0] : sprCheck[1];
    }

    private void OnClickDone()
    {
        if (!string.IsNullOrEmpty(inputName.text) && inputName.text.Length >= 1 && inputName.text.Length <= 25)
        {
            Debug.LogError("SendRequest UpdateName");
            var realName = inputName.text;
            DataManager.playerName = realName;
            //GameUtils.RaiseMessage(new HomeMessages.UpdateUserProfile() { DisplayName = realName});
            //UpdatePlayerName(realName);
        }
    }
    
    private void UpdatePlayerName(string displayName)
    {
        
        //LoadDataController.Instance.playerDataInfo.username = displayName;
        //DataManager.SetDataPlayer();
        //UserInfoController.Instance.UpdateName();
        gameObject.SetActive(false);
        // var request = new UpdateUserTitleDisplayNameRequest
        // {
        //     DisplayName = displayName
        // };
        // PlayFabClientAPI.UpdateUserTitleDisplayName(request, OnUpdatePlayerNameSuccess, OnUpdatePlayerNameFailure);
    }
    
    // private void OnUpdatePlayerNameSuccess(UpdateUserTitleDisplayNameResult result)
    // {
    //     Debug.Log("Congratulations, OnUpdatePlayerNameSuccess !!!");
        
    //     Hide();
    // }

    // private void OnUpdatePlayerNameFailure(PlayFabError error)
    // {
    //     Debug.Log("OnUpdatePlayerNameFailure !!!");
    //     Debug.LogError("Here's some debug information:");
    //     Debug.LogError(error.GenerateErrorReport());

    //     ConfirmBox.Setup().AddMessageOK("Noti", Localization.Get("lb_update_name_fail"), Localization.Get("lb_ok"), null);
    // }
}
