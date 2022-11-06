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

    public TextMeshProUGUI energy;
    public TextMeshProUGUI foodPref;
    public TextMeshProUGUI target;

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

    public void SetUnitDebugInfo(ref UnitInfo unitInfo)
    {
        energy.text = unitInfo.energy.ToString();
        foodPref.text = unitInfo.foodPref.ToString();
        target.text = unitInfo.target.ToString();
    }
}