using UnityEngine;
using UnityEngine.UI; // 如果使用旧版 UI Text
// using TMPro; // 如果使用 TextMeshPro

public class PlayerInteraction : MonoBehaviour
{
    public KeyCode interactionKey = KeyCode.E;
    public Text interactionPromptUI; // 用于显示短交互提示 (如 "按 E 查看")
    public Text detailedTextUI;      // 新增：用于显示详细描述文本的UI Text

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
            detailedTextUI.gameObject.SetActive(false); // 初始化时也隐藏详细文本UI
        }
    }

    void Update()
    {
        if (canInteract && currentInteractable != null)
        {
            if (Input.GetKeyDown(interactionKey))
            {
                currentInteractable.OnInteract(this); // 将自身传递给InteractableObject
            }
        }
    }

    // 由InteractableObject在OnTriggerEnter时调用
    public void RegisterInteractable(InteractableObject interactable)
    {
        currentInteractable = interactable;
        canInteract = true;
        UpdateInteractionPromptUI(interactable); // 显示初始提示
        HideDetailedText(); // 确保详细文本框是隐藏的
    }

    // 由InteractableObject在OnTriggerExit时调用
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
            HideDetailedText(); // 玩家离开时也隐藏详细文本
        }
    }

    // 由InteractableObject调用，用于更新短交互提示UI
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

    // 由InteractableObject调用，用于显示详细文本
    public void ShowDetailedText(string textToShow)
    {
        if (detailedTextUI != null)
        {
            detailedTextUI.text = textToShow;
            detailedTextUI.gameObject.SetActive(true);
            // 当显示详细文本时，可以暂时隐藏短交互提示，避免重叠
            if (interactionPromptUI != null)
            {
                interactionPromptUI.gameObject.SetActive(false);
            }
        }
    }

    // 由InteractableObject或其他逻辑调用，用于隐藏详细文本
    public void HideDetailedText()
    {
        if (detailedTextUI != null)
        {
            detailedTextUI.gameObject.SetActive(false);
        }
    }
}
