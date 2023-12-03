using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ProgressBarManager : MonoBehaviour
{
    public Image HpBar;
    public Image FollowUpBar;
    [SerializeField] private float HPpercentage;
    Player playerScript;

    // Start is called before the first frame update
    void Start()
    {
        playerScript = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        if (HPpercentage != 0)
        {
            HPpercentage = playerScript.HealthPercentage;
            HpBar.fillAmount = playerScript.HealthPercentage;
            FollowUpDamage();
        }
        else
        {
            Debug.Log("HPerror");
        }
        
    }

    public void LoseHPbar()
    {
        HpBar.fillAmount = playerScript.HealthPercentage;
        Invoke("FollowUpDamage",1);
    }
    private void FollowUpDamage()
    {
        FollowUpBar.fillAmount=playerScript.HealthPercentage;
    }
}
