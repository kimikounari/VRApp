//TODO PhotonManagerのメソッドを順番に解読して組み込む
//Completed:Awakeメソッド
//Completed:Startメソッド
//Completed:CloseMenuUIメソッド
//Completed:OnConnectedToMasterメソッド
//Completed:OnJoinedLobbyメソッド
//Completed:OnRoomListUpdateメソッド
//Completed:FindRoom
//Completed:SetNameメソッド
//TODO7:OnDisconnectedメソッド
//Completed:CreateRoomButtonメソッド
//Completed:JoinRoomメソッド
//Completed:OnJoinedRoomメソッド
//Completed:OnCreateRoomFailedメソッド
//TODO12:OnJoinRoomFailedメソッド
//TODO13:EnterButtonメソッド
//Completed:PostAvatarNumberメソッド

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using UnityEngine.Networking;
using TMPro;
public class PhotonManager : MonoBehaviourPunCallbacks
{

    public static PhotonManager instance;
    public GameObject loadingPanel;
    public Text loadingText;
    public GameObject buttons;
    Dictionary<string, RoomInfo> roomsList = new Dictionary<string, RoomInfo>();
    private bool setName;
    public GameObject nameInputPanel;
    //public Text placeholderText;
    [SerializeField] TMP_InputField nameInput;
    [SerializeField] Button buttonForStart;
    public Text buttonTestText;
    private string user_number = "";//UserID
    string username = "";//ユーザー名
    string apiUrl = "https://django-login-yggs.onrender.com/api/get-avatar-number/ ";//API
    public GameObject roomPanel;
    public Text roomName;
    private List<Text> allPlayerNames = new List<Text>();
    public Text playerNameText;
    public GameObject startButton;
    public GameObject playerNameContent;
    public string levelToPlay;
    [SerializeField] Button StartButton;
    private List<Room> allRoomButtons = new List<Room>();
    public Room originalRoomButton;
    public GameObject roomListPanel;
    [SerializeField] Button RoomFindButton;
    public GameObject errorPanel;
    [SerializeField] Button OnlineButton;
    [SerializeField] Button RoomButton;
    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        CloseMenuUI();//初期化処理を行い、全てのUIパネルを非表示
        loadingPanel.SetActive(true);
        loadingText.text = "ネットワークに接続中";
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();//Photonネットワークに接続
        }

         //ボタンのクリックイベントリスナーを追加
        buttonForStart.onClick.AddListener(SetName);
        
        nameInput.contentType = TMP_InputField.ContentType.DecimalNumber;//inputFieldを数字で入力
        //inputFieldForSpeedを押したとき
        nameInput.onSelect.AddListener(NameInputEnter);
        //inputFieldForSpeed の編集が終了したときに 
        nameInput.onEndEdit.AddListener(NameInputEdit);

        StartButton.onClick.AddListener(PlayGame);

        RoomFindButton.onClick.AddListener(FindRoom);

        OnlineButton.onClick.AddListener(LobbyMenuDisplay);

        RoomButton.onClick.AddListener(Room);

    }

    
    //入力フィールドが選択されたとき
    void NameInputEnter(string text) => nameInput.text = "";
    //入力フィールドが編集されたとき
    void NameInputEdit(string text) => user_number = text;

    void CloseMenuUI()//全てのUIパネルを非表示にする。
    {
        loadingPanel.SetActive(false);
        buttons.SetActive(false);
        roomPanel.SetActive(false);
        errorPanel.SetActive(false);
        roomListPanel.SetActive(false);
        nameInputPanel.SetActive(false);
    }

    public override void OnConnectedToMaster()//マスターサーバーへの接続が成功した時に呼び出され、ロビーに接続
    {
        PhotonNetwork.JoinLobby();
        loadingText.text = "ロビーへの参加";
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public void LobbyMenuDisplay()//ロビーのメニューを表示するためにUIパネルを更新
    {
        CloseMenuUI();
        buttons.SetActive(true);
    }

    private void ConfirmationName()
    {
        if (!setName)//ユーザーが名前を設定しているか確認
        {//設定していない
            CloseMenuUI();
            nameInputPanel.SetActive(true);//名前入力パネルを表示
            if (PlayerPrefs.HasKey("playerName"))
            {
                //placeholderText.text = PlayerPrefs.GetString("playerName");
                nameInput.text = PlayerPrefs.GetString("playerName");
            }
        }
        else//設定されている
        {
            PhotonNetwork.NickName = PlayerPrefs.GetString("playerName");//その名前を使用
        }
    }

    public override void OnJoinedLobby()//ロビーに接続した際
    {
        LobbyMenuDisplay();//、ロビーのメニューを表示し
        roomsList.Clear();//ルームリストをクリア
        //PhotonNetwork.NickName = Random.Range(0, 1000).ToString();
        ConfirmationName();//ユーザー名を確認
    }

        public void SetName()
    {
        StartCoroutine(PostAvatarNumber());
    }

    IEnumerator PostAvatarNumber()
    {
        WWWForm form = new WWWForm();
        form.AddField("userid", user_number);
        buttonTestText.text = user_number + "としてログイン";// 送信データのログ出力
        using (UnityWebRequest www = UnityWebRequest.Post(apiUrl, form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                buttonTestText.text = "IDがちがいます。";
            }
            else
            {
                UserData userData = JsonUtility.FromJson<UserData>(www.downloadHandler.text);
                username = userData.username;
                DataManager.Instance.avatarNumber = userData.avatart_number;
                if (!string.IsNullOrEmpty(user_number))
                {
                    PhotonNetwork.NickName = username;
                    PlayerPrefs.SetString("playerName", nameInput.text);
                    LobbyMenuDisplay();
                    setName = true;
                    user_number = "";
                    nameInput.text = "";
                    CreateRoomButton();
                }
            }
        }
    }

    public class UserData
    {
        public string username;
        public int avatart_number;
    }

    public void CreateRoomButton()//ルームの作成
    {
        string predefinedRoomName = "Room";
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = 6;
        PhotonNetwork.CreateRoom(predefinedRoomName, options);
        CloseMenuUI();
        loadingText.text = "ルーム作成中";
        loadingPanel.SetActive(true);
    }

    public void JoinRoom(RoomInfo roomInfo)
    {
        PhotonNetwork.JoinRoom(roomInfo.Name);
        CloseMenuUI();
        loadingText.text = "ルーム参加中";
        loadingPanel.SetActive(true);
    }

    //TODO
    public override void OnJoinedRoom()//ルームに参加したとき
    {
        loadingText.text = "OnJoinedRoomを呼びだした";
        CloseMenuUI();
        roomPanel.SetActive(true);
        //roomName.text = PhotonNetwork.CurrentRoom.Name;
        roomName.text = "待機ユーザー";
        GetAllPlayer();
        CheckRoomMaster();
    }

    //作成してない
    public void GetAllPlayer()
    {
        InitializePlayerList();
        PlayerDisplay();
    }

    void InitializePlayerList()
    {
        foreach (var rm in allPlayerNames)
        {
            Destroy(rm.gameObject);
        }
        allPlayerNames.Clear();
    }

    void PlayerDisplay()
    {
        foreach (var players in PhotonNetwork.PlayerList)
        {
            PlayerTextGeneration(players);
        }
    }

    void PlayerTextGeneration(Player players)
    {
        Text newPlayerText = Instantiate(playerNameText);
        newPlayerText.text = players.NickName;
        newPlayerText.transform.SetParent(playerNameContent.transform,false);
        newPlayerText.transform.localScale = Vector3.one; // ローカルスケールを1に設定
        allPlayerNames.Add(newPlayerText);
    }

    private void CheckRoomMaster()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            startButton.gameObject.SetActive(true);
        }
        else
        {
            startButton.gameObject.SetActive(false);
        }
    }

    public void PlayGame()
    {
        PhotonNetwork.LoadLevel(levelToPlay);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)//ネットワークロビーでルームリストが更新されたとき
    {
        RoomUIinitialization();
        UpdateRoomList(roomList);
    }

    void RoomUIinitialization()//ルームリストのUIを初期化
    {
        foreach (Room rm in allRoomButtons)
        {
            Destroy(rm.gameObject);//UI上の既存のルームボタンが破棄
        }
        allRoomButtons.Clear();//UIが新しいルーム情報で更新
    }

    public void UpdateRoomList(List<RoomInfo> roomList)
    {
        for (int i = 0; i < roomList.Count; i++)
        {
            RoomInfo info = roomList[i];//現在のルーム情報を格納

            if (info.RemovedFromList)//ルームが削除されたらそのルームを削除
            {
                roomsList.Remove(info.Name);
            }
            else//ルームが追加されたらそのルームを情報を追加
            {
                roomsList[info.Name] = info;
            }
        }

        RoomListDisplay(roomsList);
    }

    void RoomListDisplay(Dictionary<string, RoomInfo> cachedRoomList)
    {
        foreach (var roomInfo in cachedRoomList)
        {
            originalRoomButton.RegisterRoomDetails(roomInfo.Value);//ルーム情報を登録
            allRoomButtons.Add(originalRoomButton);
        }
    }

    public void FindRoom()
    {
        CloseMenuUI();
        roomListPanel.SetActive(true);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        CloseMenuUI();
        errorPanel.SetActive(true);
    }

    public void Room()
    {
       originalRoomButton.OpenRoom();
    }

    public void LeavRoom()//ユーザーが現在参加しているPhotonのルームを退出する
    {
        PhotonNetwork.LeaveRoom();
        CloseMenuUI();
        loadingText.text = "退出中";
        loadingPanel.SetActive(true);
    }

    public override void OnLeftRoom()//Photonネットワークのルームからユーザーが正常に退出したとき
    {
        LobbyMenuDisplay();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)//Photonネットワークのルームに新しいプレイヤーが参加したとき
    {
        PlayerTextGeneration(newPlayer);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)//Photonネットワークのルームからプレイヤーが退出したとき
    {
        GetAllPlayer();
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            startButton.gameObject.SetActive(true);
        }
    }

}
