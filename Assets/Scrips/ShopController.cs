using UnityEngine;
using TMPro;

public class ShopController : MonoBehaviour
{
    [Header("Referencias")]
    public Transform treeContainer;      // Panel donde aparecen los árboles
    public GameObject treeSlotPrefab;    // Prefab: botón con nombre + precio
    public TextMeshProUGUI goldText;

    void OnEnable() => Refresh();

    public void Refresh()
    {
        foreach (Transform child in treeContainer)
            Destroy(child.gameObject);

        foreach (TreeData tree in GameManager.Instance.allTrees)
        {
            GameObject slot = Instantiate(treeSlotPrefab, treeContainer);
            TreeSlotUI slotUI = slot.GetComponent<TreeSlotUI>();
            if (slotUI != null)
                slotUI.Setup(tree, this);
        }

        if (goldText != null)
            goldText.text = $"Oro: {GameManager.Instance.playerData.gold}";
    }

    public void TryUnlock(TreeData tree)
    {
        if (tree.isUnlocked)
        {
            SelectTree(tree);
            return;
        }

        bool success = GameManager.Instance.UnlockTree(tree);
        if (!success) return; // sin oro suficiente

        Refresh();
    }

    void SelectTree(TreeData tree)
    {
        GameManager.Instance.SelectTree(tree);
        // Aquí luego navegamos al panel del árbol
    }
}