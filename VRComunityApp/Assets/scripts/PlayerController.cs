using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class PlayerController : MonoBehaviourPunCallbacks
{
    public Transform viewPoint;//カメラの位置オブジェクト
    public float mouseSensitivity = 1f;//視点移動の速度
    private Vector2 mouseInput;//ユーザーのマウス入力を格納
    private float verticalMouseInput;//y軸の回転を格納　回転を制限したいから
    private Camera cam;//カメラ

    private Vector3 moveDir;//プレイヤーの入力を格納（移動）
    private Vector3 movement;//進む方向を格納する変数
    private float activeMoveSpeed = 1f;//実際の移動速度

    public Joystick joystick; // ジョイスティックスクリプトへの参照
    private bool isJoystickActive = false; // ジョイスティックがアクティブかどうかを追跡するフラグ

    SpawnManager spawnManager;//スポーンマネージャー管理
    public Animator animator;
    private void Awake()
    {
        //タグからSpawnManagerを探す
        spawnManager = GameObject.FindGameObjectWithTag("SpawnManager").GetComponent<SpawnManager>();
        joystick = FindObjectOfType<Joystick>();
    }
    private void Start()
    {
        //変数にメインカメラを格納
        cam = Camera.main;
        //TODO:プレイヤーのアバター番号に応じて、アバターを切り替える
        int avatarNumber = DataManager.Instance.avatarNumber;
        print(avatarNumber + "のアバターを生成します。");
        //ランダムな位置でスポーンさせる
        //transform.position = spawnManager.GetSpawnPoint().position;
    }

    private void Update()
    {
        if (!photonView.IsMine)
        {
            return;
        }
        if (!joystick.IsDragging)
        {
            PlayerRotate();
        }
        PlayerMove();
        AnimatorSet();
    }

    public void PlayerRotate()
    {
        //変数にユーザーのマウスの動きを格納
        mouseInput = new Vector2(Input.GetAxisRaw("Mouse X") * mouseSensitivity,
            Input.GetAxisRaw("Mouse Y") * mouseSensitivity);

        //横回転を反映(transform.eulerAnglesはオイラー角としての角度が返される)
        transform.rotation = Quaternion.Euler
            (transform.eulerAngles.x,
            transform.eulerAngles.y + mouseInput.x, //マウスのx軸の入力を足す
            transform.eulerAngles.z);



        //変数にy軸のマウス入力分の数値を足す
        verticalMouseInput += mouseInput.y;

        //変数の数値を丸める（上下の視点制御）
        verticalMouseInput = Mathf.Clamp(verticalMouseInput, -60f, 60f);

        //縦の視点回転を反映(-を付けないと上下反転してしまう)
        viewPoint.rotation = Quaternion.Euler
            (-verticalMouseInput,
            viewPoint.transform.rotation.eulerAngles.y,
            viewPoint.transform.rotation.eulerAngles.z);
    }

    //Update関数が呼ばれた後に実行される
    private void LateUpdate()
    {
        if (!photonView.IsMine)
        {
            //戻ってこれ以降の処理を行わない
            return;
        }
        //カメラをプレイヤーの子にするのではなく、スクリプトで位置を合わせる
        cam.transform.position = viewPoint.position;
        cam.transform.rotation = viewPoint.rotation;
    }

    public void PlayerMove()
    {
        //変数の水平と垂直の入力を格納する（wasdや矢印の入力）
        //moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 
        //0, Input.GetAxisRaw("Vertical"));

        //Debug.Log(moveDir);説明用

        //ゲームオブジェクトのｚ軸とx軸に入力された値をかけると進む方向が出せる
        //movement = ((transform.forward * moveDir.z) + (transform.right * moveDir.x)).normalized;

        //現在位置に進む方向＊移動スピード＊フレーム間秒数を足す
        //transform.position += movement * activeMoveSpeed * Time.deltaTime;

        // キーボードの入力を取得
        moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

        // ジョイスティックの入力があれば、それを使用する
        if (joystick.Horizontal() != 0 || joystick.Vertical() != 0)
        {
            moveDir = new Vector3(joystick.Horizontal(), 0, joystick.Vertical());
        }

        // 移動方向の計算
        movement = ((transform.forward * moveDir.z) + (transform.right * moveDir.x)).normalized;

        // プレイヤーの位置を更新
        transform.position += movement * activeMoveSpeed * Time.deltaTime;
    }

    //スマホ用の移動
    public void MoveForward()
    {
        transform.position += transform.forward * activeMoveSpeed * Time.deltaTime;
    }

    public void MoveBack()
    {
        transform.position -= transform.forward * activeMoveSpeed * Time.deltaTime;
    }

    public void MoveLeft()
    {
        transform.position -= transform.right * activeMoveSpeed * Time.deltaTime;
    }

    public void MoveRight()
    {
        transform.position += transform.right * activeMoveSpeed * Time.deltaTime;
    }

    private void AnimatorSet()
    {
        if (moveDir != Vector3.zero)
        {
            animator.SetBool("Walk", true);

        }
        else
        {
            animator.SetBool("Walk", false);
        }
    }

}