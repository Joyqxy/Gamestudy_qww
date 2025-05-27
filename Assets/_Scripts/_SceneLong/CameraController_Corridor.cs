using UnityEngine;

public class CameraController_Corridor : MonoBehaviour
{
    public Transform playerTarget; // 玩家的Transform组件 (在Inspector中指定)
    public float smoothSpeed = 0.125f; // 摄像机跟随的平滑速度
    public Vector3 offset; // 摄像机相对于玩家的偏移量

    [Header("摄像机边界 (世界坐标)")]
    public bool enableBounds = true;
    public float minX_Cam = -5f;
    public float maxX_Cam = 5f;
    public float minY_Cam = -3f;
    public float maxY_Cam = 3f;
    // 这些边界值需要根据你的长廊大小和希望摄像机移动的范围来设置

    void Start()
    {
        if (playerTarget == null)
        {
            // 尝试自动查找玩家 (如果玩家有特定标签)
            GameObject playerObject = GameObject.FindGameObjectWithTag("PlayerCharacter_Boss");
            if (playerObject != null)
            {
                playerTarget = playerObject.transform;
            }
            else
            {
                Debug.LogError("摄像机未能找到玩家目标 (playerTarget)!");
                enabled = false; // 禁用脚本
            }
        }
    }

    void LateUpdate() // 使用LateUpdate确保玩家移动完成后再更新摄像机位置
    {
        if (playerTarget == null) return;

        Vector3 desiredPosition = playerTarget.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime); // Time.deltaTime for frame rate independence in Lerp speed

        if (enableBounds)
        {
            // 获取摄像机的尺寸 (orthographicSize 是高度的一半)
            // Camera cam = GetComponent<Camera>();
            // float camHeight = cam.orthographicSize * 2;
            // float camWidth = cam.aspect * camHeight;

            // 将摄像机位置限制在边界内
            smoothedPosition.x = Mathf.Clamp(smoothedPosition.x, minX_Cam, maxX_Cam);
            smoothedPosition.y = Mathf.Clamp(smoothedPosition.y, minY_Cam, maxY_Cam);
        }

        transform.position = new Vector3(smoothedPosition.x, smoothedPosition.y, transform.position.z); //保持Z轴不变
    }
}
