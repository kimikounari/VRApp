using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpawnManager : MonoBehaviour
{
    //スポーンポイントオブジェクト格納配列
    public Transform[] spawnPositons;

    public GameObject playerPrefab;
    private GameObject player;
    public float respawnInterval = 5f;
    public GameObject[] playerPrefabs;
    int avatarNumber;
    private void Start()
    {
        for (int i = 0; i < playerPrefabs.Length; i++)
        {
            // 各Prefabの名前とインデックスをコンソールに表示
            Debug.Log("Index: " + i + " - Name: " + playerPrefabs[i].name);
        }

        foreach (var pos in spawnPositons)
        {
            pos.gameObject.SetActive(false);
        }
        if (PhotonNetwork.IsConnected)
        {
            SpawnPlayer();
        }

    }

    public Transform GetSpawnPoint()
    {
        return spawnPositons[Random.Range(0, spawnPositons.Length)];
    }

    public void SpawnPlayer()
    {
        Debug.Log(DataManager.Instance.avatarNumber + "のオブジェクトをスポーン");
        Transform spawnPoint = GetSpawnPoint();
        player = PhotonNetwork.Instantiate(playerPrefabs[DataManager.Instance.avatarNumber - 1].name, spawnPoint.position, spawnPoint.rotation);
    }
}
