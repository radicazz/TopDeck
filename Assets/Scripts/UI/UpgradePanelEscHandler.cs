using UnityEngine;
public class UpgradePanelEscHandler : MonoBehaviour{
  void Update(){ if(Input.GetKeyDown(KeyCode.Escape)){ var ui=FindFirstObjectByType<UpgradePanelUIDocument>(); if(ui!=null) ui.HidePanel(); } }
}
