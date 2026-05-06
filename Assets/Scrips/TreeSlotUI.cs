using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TreeSlotUI : MonoBehaviour
{
    [Header("Referencias")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI priceText;
    public Button actionButton;

    private TreeData _tree;
    private ShopController _shop;

    public void Setup(TreeData tree, ShopController shop)
    {
        _tree = tree;
        _shop = shop;

        nameText.text = tree.treeName;

        if (tree.isUnlocked)
        {
            priceText.text = "Desbloqueado";
            actionButton.GetComponentInChildren<TextMeshProUGUI>().text = "Seleccionar";
        }
        else
        {
            priceText.text = $"Costo: {tree.unlockCost} oro";
            actionButton.GetComponentInChildren<TextMeshProUGUI>().text = "Comprar";
        }

        actionButton.onClick.AddListener(OnButtonPressed);
    }

    void OnButtonPressed() => _shop.TryUnlock(_tree);

    void OnDestroy() => actionButton.onClick.RemoveAllListeners();
}
