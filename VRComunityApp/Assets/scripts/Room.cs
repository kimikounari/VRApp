using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;

public class Room : MonoBehaviour
{
    public Text buttonText;
    private RoomInfo info;

    public void RegisterRoomDetails(RoomInfo info)//ルーム情報を登録し、ボタンテキストを設定する
    {
        this.info = info;
        buttonText.text = "入室";
    }

    public void OpenRoom()
    {
        PhotonManager.instance.JoinRoom(info);//ルームに参加する
    }
}

