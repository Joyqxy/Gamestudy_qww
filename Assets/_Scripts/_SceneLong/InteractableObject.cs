using UnityEngine;
using UnityEngine.SceneManagement;

public class InteractableObject : MonoBehaviour
{
    [Header("������ʾ")]
    public string promptOnApproach = "�� E �鿴"; // ����ʱ��ʾ�ĳ�ʼ��ʾ
    public string taskDescriptionText = "�������������ϸ����..."; // ��һ�ν�����ʾ���ı�
    public string promptForAction = "�� E ��ʼ����"; // ��ʾ�����󣬵ȴ�ִ�ж�������ʾ

    [Header("������Ϊ")]
    public bool isTwoStepInteraction = true; // �Ƿ�Ϊ��������
    public bool triggerSceneChangeOnAction = false; // �ڶ������򵥲����Ƿ��л�����
    public string targetSceneName = ""; // Ŀ�곡������
    private bool descriptionShown = false; // ��������Ƿ�����ʾ

    // ��ȡ��ǰӦ����ʾ�Ľ�����ʾ�ı� (��PlayerInteraction����)
    public string GetCurrentPrompt()
    {
        if (isTwoStepInteraction && descriptionShown)
        {
            return promptForAction;
        }
        return promptOnApproach;
    }

    // ��ҽ���ʱ���ô˷���
    public void OnInteract(PlayerInteraction player) // ����PlayerInteraction������
    {
        if (player == null) return;

        if (isTwoStepInteraction && !descriptionShown)
        {
            // ��һ������ʾ��������
            player.ShowDetailedText(taskDescriptionText); // ֪ͨ��ҽű���ʾ��ϸ�ı�
            descriptionShown = true;
            player.UpdateInteractionPromptUI(this); // ֪ͨ��ҽű����½�����ʾ (��Ϊ "�� E ��ʼ����")
        }
        else
        {
            // �ڶ�����ִ����Ҫ���� (�������Ǹ���������)
            player.HideDetailedText(); // �����ϸ�ı����ţ��ص���

            if (triggerSceneChangeOnAction && !string.IsNullOrEmpty(targetSceneName))
            {
                Debug.Log($"׼����ת������: {targetSceneName}");
                LoadTargetScene();
            }
            else
            {
                // ִ�������ǳ����л��Ķ���
                Debug.Log($"ִ���� {gameObject.name} ����Ҫ������");
            }
            // ����״̬���Ա��´ν���ʱ�ӵ�һ����ʼ
            descriptionShown = false; 
            // ������ɺ�PlayerInteraction����OnTriggerExitʱ������ʾ��
            // �������������ڷ�Χ�ڣ������GetCurrentPrompt()��ʾ��ʼ��ʾ
        }
    }

    protected void LoadTargetScene()
    {
        SceneManager.LoadScene(targetSceneName);
    }

    // ����ҽ��봥������
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerCharacter_Boss"))
        {
            PlayerInteraction playerInteraction = other.GetComponent<PlayerInteraction>();
            if (playerInteraction != null)
            {
                // ��ҽ���ʱ�����ô˶���Ľ���״̬��ȷ��ÿ�ζ��Ǵӵ�һ����ʼ
                ResetInteractionState();
                playerInteraction.RegisterInteractable(this);
            }
        }
    }

    // ������뿪��������
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("PlayerCharacter_Boss"))
        {
            PlayerInteraction playerInteraction = other.GetComponent<PlayerInteraction>();
            if (playerInteraction != null)
            {
                playerInteraction.UnregisterInteractable(this);
            }
            // ����뿪ʱ����״̬��һ����ϰ��
            ResetInteractionState();
        }
    }

    // ���ý���״̬ (���磬������뿪�����������һ������������)
    public void ResetInteractionState()
    {
        descriptionShown = false;
    }
}
