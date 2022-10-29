using UnityEngine;
using TMPro;

public class UISingleton : MonoBehaviour
{
    public static UISingleton singleton;

    public TextMeshProUGUI unitCount;
    public TextMeshProUGUI attractionFroce;
    public TextMeshProUGUI repultionForce;
    public TextMeshProUGUI foodSearchRadius;
    public TextMeshProUGUI poisonSearchRadius;
    public TextMeshProUGUI maxFroce;
    public TextMeshProUGUI maxSpeed;

    private void Awake()
    {
        singleton = this;
    }

    public void SetText(ref AverageDNAStats averageDNAStats,int seekersCount)
    {
        unitCount.text = seekersCount.ToString();

        attractionFroce.text = averageDNAStats.attractionFroce.ToString();
        repultionForce.text = averageDNAStats.repultionForce.ToString();

        foodSearchRadius.text = averageDNAStats.foodSearchRadius.ToString();
        poisonSearchRadius.text = averageDNAStats.poisonSearchRadius.ToString();

        maxFroce.text = averageDNAStats.maxFroce.ToString();
        maxSpeed.text = averageDNAStats.maxSpeed.ToString();
    }
}