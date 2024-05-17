using UnityEngine;

public class UITile : MonoBehaviour
{
    [SerializeField] private GameObject iconCellBorder;
    [SerializeField] private GameObject iconDisableSelection;
    [SerializeField] private GameObject iconExhausted;


    private void Awake()
    {
        iconCellBorder.SetActive(false);
        iconDisableSelection.SetActive(false);
        iconExhausted.SetActive(false);
    }

    public void SetCellBorder(bool value)
    {
        iconCellBorder.SetActive(value);
    }

    public void SetDisableSelection(bool value)
    {
        iconDisableSelection.SetActive(value);
    }

    public void SetExhausted(bool value)
    {
        iconExhausted.SetActive(value);
    }
}
