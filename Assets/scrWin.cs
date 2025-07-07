using UnityEngine;
/// <summary>
/// This is just to control what shows on the win scren and which side pops up as the winner
/// </summary>

public class scrWin : MonoBehaviour
{
    public GameObject panelP1Winner;
    public GameObject panelP2Winner;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        panelP1Winner.SetActive(false);
        panelP2Winner.SetActive(false);

        if (GameManager.Instance.CurrentWinner == GameManager.CurrentWinnerType.P1)
            panelP1Winner.SetActive(true);
        else 
            if (GameManager.Instance.CurrentWinner == GameManager.CurrentWinnerType.P2)
            panelP2Winner.SetActive(true);  
    }
}
