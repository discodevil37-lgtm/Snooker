using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private int playerScore;
    public int PlayerScore
    {
        get { return playerScore; }
        set
        {
            playerScore = value;
            if (guiScore != null) guiScore.text = $"SCORE : {playerScore}";
        }
    }

    [SerializeField] private GameObject ballLine;
    [SerializeField] private GameObject[] ballPositions;
    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private GameObject cueBall;
    [SerializeField] private float xInput = 0f;
    [SerializeField] private GameObject cam;

    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Text guiScore;
    [SerializeField] private TMP_Text TEXT;

    [SerializeField] private GameObject JustBigPanal;
    [SerializeField] private GameObject MenuOpener;
    [SerializeField] private GameObject MenuPanel;
    [SerializeField] private GameObject ResetBotton;
    [SerializeField] private GameObject ExitBotton;
    [SerializeField] private GameObject ExitMenuBotton;

    private bool ISrestart = false;
    private bool isBallInMotion = false;
    private Rigidbody cueBallRb;

    // ตัวแปรเก็บมุมการเล็งแยกต่างหาก ไม่ไม่อิงกับ Rotation ของลูกบอล
    private float aimAngle = 0f;
    private Vector3 cameraOffset = new Vector3(0f, 4f, -6f);

    public static GameManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (cueBall != null)
        {
            cueBallRb = cueBall.GetComponent<Rigidbody>();
            aimAngle = cueBall.transform.eulerAngles.y;
        }

        SetBall(BallColor.Red, 1);
        SetBall(BallColor.Yellow, 2);
        SetBall(BallColor.Green, 3);
        SetBall(BallColor.Brown, 4);
        SetBall(BallColor.Blue, 5);
        SetBall(BallColor.Pink, 6);
        SetBall(BallColor.Black, 7);

        if (cam != null) cam.transform.parent = null;

        CameraBehindCueBall();

        if (!ISrestart && Setting.fromSave)
        {
            LoadGame();
        }

        ShowString("  ", false);
        menuClose();
    }

    void Update()
    {
        if (!isBallInMotion)
        {
            RotateAim();
            CameraBehindCueBall();
        }
        else
        {
            FollowCueBallWhenMoving();
            CheckIfBallsStopped();
        }

        if (Keyboard.current.backspaceKey.wasPressedThisFrame)
        {
            StopBall();
        }

        if (Keyboard.current.spaceKey.wasPressedThisFrame && !isBallInMotion)
        {
            SHootBall();
        }

        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
        {
            xInput = -1f;
        }
        else if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
        {
            xInput = 1f;
        }
        else
        {
            xInput = 0f;
        }

        if (Keyboard.current.leftShiftKey.isPressed && Keyboard.current.sKey.wasPressedThisFrame)
        {
            SaveGame();
        }

        if (playerScore >= 28)
        {
            ShowString(" R U HACKER ", true);
            Time.timeScale = 0f;
        }
    }

    public void SetBall(BallColor col, int i)
    {
        if (ballPositions != null && i < ballPositions.Length && ballPositions[i] != null)
        {
            GameObject obj = Instantiate(ballPrefab, ballPositions[i].transform.position, Quaternion.identity);
            Ball b = obj.GetComponent<Ball>();
            if (b != null)
            {
                b.SetColorAndPoint(col);
            }
        }
    }

    // หมุนมุมเล็งแบบแยกอิสระ
    private void RotateAim()
    {
        aimAngle += xInput * 100f * Time.deltaTime;

        // ปรับทิศทางของเส้นเล็ง (ballLine) ให้หมุนตามมุมเล็ง
        if (ballLine != null)
        {
            ballLine.transform.rotation = Quaternion.Euler(0f, aimAngle, 0f);
        }
    }

    private void SHootBall()
    {
        if (cueBallRb == null) return;

        isBallInMotion = true;

        if (ballLine != null)
        {
            ballLine.SetActive(false);
        }

        // คำนวณทิศทางยิงจากมุม aimAngle โดยตรง (ไม่พึ่งพา Rotation ของ CueBall)
        Vector3 shootDirection = Quaternion.Euler(0f, aimAngle, 0f) * Vector3.forward;
        cueBallRb.AddForce(shootDirection.normalized * 35f, ForceMode.Impulse);
    }

    // จัดตำแหน่งกล้องให้อยู่หลังจุดเล็ง
    public void CameraBehindCueBall()
    {
        if (cam != null && cueBall != null)
        {
            Quaternion aimRotation = Quaternion.Euler(0f, aimAngle, 0f);
            Vector3 targetCamPos = cueBall.transform.position + (aimRotation * cameraOffset);

            cam.transform.position = targetCamPos;
            cam.transform.LookAt(cueBall.transform.position + Vector3.up * 0.2f);
        }
    }

    // กล้องตามลูกบอลขณะเคลื่อนที่ (มองจากมุมสูง นิ่ง ไม่หมุนตามลูก)
    private void FollowCueBallWhenMoving()
    {
        if (cueBall != null && cam != null)
        {
            // ล็อกตำแหน่งกล้องไว้ที่มุมสูงด้านหลัง ลอยตามลูกไปแบบนุ่มนวล
            Vector3 followOffset = Quaternion.Euler(0f, aimAngle, 0f) * new Vector3(0f, 6f, -5f);
            Vector3 targetPosition = cueBall.transform.position + followOffset;

            cam.transform.position = Vector3.Lerp(cam.transform.position, targetPosition, Time.deltaTime * 4f);
            cam.transform.LookAt(cueBall.transform.position);
        }
    }

    private void CheckIfBallsStopped()
    {
        Ball[] allBalls = FindObjectsByType<Ball>(FindObjectsSortMode.None);
        bool allStopped = true;

        if (cueBallRb != null && cueBallRb.linearVelocity.magnitude > 0.05f)
        {
            allStopped = false;
        }

        if (allStopped)
        {
            foreach (Ball b in allBalls)
            {
                if (b != null)
                {
                    Rigidbody rb = b.GetComponent<Rigidbody>();
                    if (rb != null && rb.linearVelocity.magnitude > 0.05f)
                    {
                        allStopped = false;
                        break;
                    }
                }
            }
        }

        if (allStopped)
        {
            StopBall();
        }
    }

    private void StopBall()
    {
        isBallInMotion = false;

        if (cueBallRb != null)
        {
            cueBallRb.linearVelocity = Vector3.zero;
            cueBallRb.angularVelocity = Vector3.zero;
        }

        if (ballLine != null)
        {
            ballLine.SetActive(true);
        }

        CameraBehindCueBall();
    }

    public void ShowScoreText(int input)
    {
        PlayerScore += input;
    }

    public void ShowString(string S, bool flag)
    {
        if (JustBigPanal != null) JustBigPanal.SetActive(flag);
        if (TEXT != null) TEXT.text = S;
    }

    public void menuOpen()
    {
        if (MenuPanel != null) MenuPanel.SetActive(true);
    }

    public void menuClose()
    {
        if (MenuPanel != null) MenuPanel.SetActive(false);
    }

    public void restart()
    {
        SceneManager.LoadSceneAsync("Loading");
        Setting.fromSave = false;
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void SaveGame()
    {
        StopBall();

        if (cueBall != null)
        {
            PlayerPrefs.SetFloat("cueBallPosX", cueBall.transform.position.x);
            PlayerPrefs.SetFloat("cueBallPosY", cueBall.transform.position.y);
            PlayerPrefs.SetFloat("cueBallPosZ", cueBall.transform.position.z);
            Debug.Log("Saved");
        }
    }

    public void LoadGame()
    {
        if (cueBall != null)
        {
            float x = PlayerPrefs.GetFloat("cueBallPosX");
            float y = PlayerPrefs.GetFloat("cueBallPosY");
            float z = PlayerPrefs.GetFloat("cueBallPosZ");

            cueBall.transform.position = new Vector3(x, y, z);
            Debug.Log("Load");
        }
    }
}