using UnityEngine;
using UnityEngine.UI; // ���ʹ�þɰ� UI Text
// using TMPro; // ���ʹ�� TextMeshPro

public class PlayerInteraction : MonoBehaviour
{
    public KeyCode interactionKey = KeyCode.E;
    public Text interactionPromptUI; // ������ʾ�̽�����ʾ (�� "�� E �鿴")
    public Text detailedTextUI;      // ������������ʾ��ϸ�����ı���UI Text

    private InteractableObject currentInteractable = null;
    private bool canInteract = false;

    void Start()
    {
        if (interactionPromptUI != null)
        {
            interactionPromptUI.gameObject.SetActive(false);
        }
        if (detailedTextUI != null)
        {
            detailedTextUI.gameObject.SetActive(false); // ��ʼ��ʱҲ������ϸ�ı�UI
        }
    }

    void Update()
    {
        if (canInteract && currentInteractable != null)
        {
            if (Input.GetKeyDown(interactionKey))
            {
                currentInteractable.OnInteract(this); // �������ݸ�InteractableObject
            }
        }
    }

    // ��InteractableObject��OnTriggerEnterʱ����
    public void RegisterInteractable(InteractableObject interactable)
    {
        currentInteractable = interactable;
        canInteract = true;
        UpdateInteractionPromptUI(interactable); // ��ʾ��ʼ��ʾ
        HideDetailedText(); // ȷ����ϸ�ı��������ص�
    }

    // ��InteractableObject��OnTriggerExitʱ����
    public void UnregisterInteractable(InteractableObject interactable)
    {
        if (currentInteractable == interactable)
        {
            canInteract = false;
            currentInteractable = null;
            if (interactionPromptUI != null)
            {
                interactionPromptUI.gameObject.SetActive(false);
            }
            HideDetailedText(); // ����뿪ʱҲ������ϸ�ı�
        }
    }

    // ��InteractableObject���ã����ڸ��¶̽�����ʾUI
    public void UpdateInteractionPromptUI(InteractableObject interactable)
    {
        if (interactionPromptUI != null && interactable != null && canInteract)
        {
            interactionPromptUI.text = interactable.GetCurrentPrompt();
            interactionPromptUI.gameObject.SetActive(true);
        }
        else if (interactionPromptUI != null)
        {
             interactionPromptUI.gameObject.SetActive(false);
        }
    }

    // ��InteractableObject���ã�������ʾ��ϸ�ı�
    public void ShowDetailedText(string textToShow)
    {
        if (detailedTextUI != null)
        {
            detailedTextUI.text = textToShow;
            detailedTextUI.gameObject.SetActive(true);
            // ����ʾ��ϸ�ı�ʱ��������ʱ���ض̽�����ʾ�������ص�
            if (interactionPromptUI != null)
            {
                interactionPromptUI.gameObject.SetActive(false);
            }
        }
    }

    // ��InteractableObject�������߼����ã�����������ϸ�ı�
    public void HideDetailedText()
    {
        if (detailedTextUI != null)
        {
            detailedTextUI.gameObject.SetActive(false);
        }
    }
}
