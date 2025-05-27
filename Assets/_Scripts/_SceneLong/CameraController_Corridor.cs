using UnityEngine;

public class CameraController_Corridor : MonoBehaviour
{
    public Transform playerTarget; // ��ҵ�Transform��� (��Inspector��ָ��)
    public float smoothSpeed = 0.125f; // ����������ƽ���ٶ�
    public Vector3 offset; // ������������ҵ�ƫ����

    [Header("������߽� (��������)")]
    public bool enableBounds = true;
    public float minX_Cam = -5f;
    public float maxX_Cam = 5f;
    public float minY_Cam = -3f;
    public float maxY_Cam = 3f;
    // ��Щ�߽�ֵ��Ҫ������ĳ��ȴ�С��ϣ��������ƶ��ķ�Χ������

    void Start()
    {
        if (playerTarget == null)
        {
            // �����Զ�������� (���������ض���ǩ)
            GameObject playerObject = GameObject.FindGameObjectWithTag("PlayerCharacter_Boss");
            if (playerObject != null)
            {
                playerTarget = playerObject.transform;
            }
            else
            {
                Debug.LogError("�����δ���ҵ����Ŀ�� (playerTarget)!");
                enabled = false; // ���ýű�
            }
        }
    }

    void LateUpdate() // ʹ��LateUpdateȷ������ƶ���ɺ��ٸ��������λ��
    {
        if (playerTarget == null) return;

        Vector3 desiredPosition = playerTarget.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime); // Time.deltaTime for frame rate independence in Lerp speed

        if (enableBounds)
        {
            // ��ȡ������ĳߴ� (orthographicSize �Ǹ߶ȵ�һ��)
            // Camera cam = GetComponent<Camera>();
            // float camHeight = cam.orthographicSize * 2;
            // float camWidth = cam.aspect * camHeight;

            // �������λ�������ڱ߽���
            smoothedPosition.x = Mathf.Clamp(smoothedPosition.x, minX_Cam, maxX_Cam);
            smoothedPosition.y = Mathf.Clamp(smoothedPosition.y, minY_Cam, maxY_Cam);
        }

        transform.position = new Vector3(smoothedPosition.x, smoothedPosition.y, transform.position.z); //����Z�᲻��
    }
}
