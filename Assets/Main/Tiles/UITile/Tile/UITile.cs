using UnityEngine;

public class UITile : MonoBehaviour
{
    [SerializeField] private GameObject iconCellBorder;
    [SerializeField] private GameObject iconDisableSelection;
    [SerializeField] private GameObject iconEnablelection;
    [SerializeField] private GameObject iconExhausted;
    [SerializeField] private GameObject highlightActiveCountry;
    [SerializeField] private GameObject highlightPlayerCountry;


    private void Awake()
    {
        iconCellBorder.SetActive(false);
        iconDisableSelection.SetActive(false);
        iconEnablelection.SetActive(false);
        iconExhausted.SetActive(false);
        highlightActiveCountry.SetActive(false);
        highlightPlayerCountry.SetActive(false);
    }

    public void SetCellBorder(bool value)
    {
        iconCellBorder.SetActive(value);
    }

    public void SetDisableSelection(bool? disable)
    {
        if (disable == null)
        {
            iconDisableSelection.SetActive(false);
            iconEnablelection.SetActive(false);
        }
        else
        {
            iconEnablelection.SetActive(!disable.Value);
            iconDisableSelection.SetActive(disable.Value);
        }
    }

    public void SetExhausted(bool value)
    {
        iconExhausted.SetActive(value);
    }

    public void SetActiveCountry(bool value)
    {
        highlightActiveCountry.SetActive(value);
    }

    public void SetPlayerCountry(bool value)
    {
        highlightPlayerCountry.SetActive(value);
    }
}
