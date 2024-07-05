using UnityEngine;
public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }

    public int avatarNumber;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // シーン遷移しても破棄されないように設定
        }
        else
        {
            Destroy(gameObject); // 既に存在する場合は、新しいインスタンスを破棄
        }
    }
}
