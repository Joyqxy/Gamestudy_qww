using UnityEngine;
using UnityEngine.SceneManagement;

public class InteractableObject : MonoBehaviour
{
    [Header("交互提示")]
    public string promptOnApproach = "按 E 查看"; // 靠近时显示的初始提示
    public string taskDescriptionText = "这里是任务的详细描述..."; // 第一次交互显示的文本
    public string promptForAction = "按 E 开始任务"; // 显示描述后，等待执行动作的提示

    [Header("交互行为")]
    public bool isTwoStepInteraction = true; // 是否为两步交互
    public bool triggerSceneChangeOnAction = false; // 第二步（或单步）是否切换场景
    public string targetSceneName = ""; // 目标场景名称
    private bool descriptionShown = false; // 标记描述是否已显示

    // 获取当前应该显示的交互提示文本 (给PlayerInteraction调用)
    public string GetCurrentPrompt()
    {
        if (isTwoStepInteraction && descriptionShown)
        {
            return promptForAction;
        }
        return promptOnApproach;
    }

    // 玩家交互时调用此方法
    public void OnInteract(PlayerInteraction player) // 接收PlayerInteraction的引用
    {
        if (player == null) return;

        if (isTwoStepInteraction && !descriptionShown)
        {
            // 第一步：显示任务描述
            player.ShowDetailedText(taskDescriptionText); // 通知玩家脚本显示详细文本
            descriptionShown = true;
            player.UpdateInteractionPromptUI(this); // 通知玩家脚本更新交互提示 (变为 "按 E 开始任务")
        }
        else
        {
            // 第二步：执行主要动作 (或者这是个单步交互)
            player.HideDetailedText(); // 如果详细文本开着，关掉它

            if (triggerSceneChangeOnAction && !string.IsNullOrEmpty(targetSceneName))
            {
                Debug.Log($"准备跳转到场景: {targetSceneName}");
                LoadTargetScene();
            }
            else
            {
                // 执行其他非场景切换的动作
                Debug.Log($"执行了 {gameObject.name} 的主要动作。");
            }
            // 重置状态，以便下次交互时从第一步开始
            descriptionShown = false; 
            // 交互完成后，PlayerInteraction会在OnTriggerExit时隐藏提示，
            // 或者如果玩家仍在范围内，会根据GetCurrentPrompt()显示初始提示
        }
    }

    protected void LoadTargetScene()
    {
        SceneManager.LoadScene(targetSceneName);
    }

    // 当玩家进入触发区域
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerCharacter_Boss"))
        {
            PlayerInteraction playerInteraction = other.GetComponent<PlayerInteraction>();
            if (playerInteraction != null)
            {
                // 玩家进入时，重置此对象的交互状态，确保每次都是从第一步开始
                ResetInteractionState();
                playerInteraction.RegisterInteractable(this);
            }
        }
    }

    // 当玩家离开触发区域
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("PlayerCharacter_Boss"))
        {
            PlayerInteraction playerInteraction = other.GetComponent<PlayerInteraction>();
            if (playerInteraction != null)
            {
                playerInteraction.UnregisterInteractable(this);
            }
            // 玩家离开时重置状态是一个好习惯
            ResetInteractionState();
        }
    }

    // 重置交互状态 (例如，当玩家离开触发区或完成一次完整交互后)
    public void ResetInteractionState()
    {
        descriptionShown = false;
    }
}
