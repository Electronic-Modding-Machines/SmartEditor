using System.Reflection;
using ADOFAI;
using ADOFAI.LevelEditor.Controls;
using JALib.Tools;
using PropertyInfo = System.Reflection.PropertyInfo;

namespace SmartEditor.FixLoad.CustomSaveState;

public static class FixPrivateMethod {
    public static MethodInfo DeleteFloorMethod = typeof(scnEditor).Method("DeleteFloor");
    public static MethodInfo MoveCameraToFloorMethod = typeof(scnEditor).Method("MoveCameraToFloor");
    public static MethodInfo OffsetFloorIDsInEventsMethod = typeof(scnEditor).Method("OffsetFloorIDsInEvents");
    public static MethodInfo CopyEventMethod = typeof(scnEditor).Method("CopyEvent");
    public static FieldInfo copiedHitsoundField = typeof(scnEditor).Field("copiedHitsound");
    public static FieldInfo copiedTrackColorField = typeof(scnEditor).Field("copiedHitsound");
    public static MethodInfo convertEmptyToNaNMethod = typeof(PropertyControl_Vector2).Method("ConvertEmptyToNaN");
    public static MethodInfo convertNaNToEmptyMethod = typeof(PropertyControl_Vector2).Method("ConvertNaNToEmpty");
    public static MethodInfo updateSteamCallbacksMethod = typeof(scnEditor).Method("UpdateSteamCallbacks"); 
    public static MethodInfo quitToMenuMethod = typeof(scnEditor).Method("QuitToMenu");
    public static MethodInfo updateSelectedFloorMethod = typeof(scnEditor).Method("UpdateSelectedFloor");
    public static MethodInfo ottoUpdateMethod = typeof(scnEditor).Method("OttoUpdate");
    public static MethodInfo saveBackupMethod = typeof(scnEditor).Method("SaveBackup");
    public static MethodInfo handleKeyboardActionsMethod = typeof(scnEditor).Method("HandleKeyboardActions");
    public static MethodInfo handleMouseActionsMethod = typeof(scnEditor).Method("HandleMouseActions");
    public static PropertyInfo pausedProperty = typeof(scnEditor).Property("paused");
    
    public static LevelEvent copiedHitsound => (LevelEvent) copiedHitsoundField.GetValue(scnEditor.instance);
    public static LevelEvent copiedTrackColor => (LevelEvent) copiedTrackColorField.GetValue(scnEditor.instance);

    public static void DeleteFloor(int index, bool remakePath = true) {
        DeleteFloorMethod.Invoke(scnEditor.instance, [index, remakePath]);
    }

    public static void MoveCameraToFloor(scrFloor floor) {
        MoveCameraToFloorMethod.Invoke(scnEditor.instance, [floor]);
    }

    public static void OffsetFloorIDsInEvents(int index, int offset) {
        OffsetFloorIDsInEventsMethod.Invoke(scnEditor.instance, [index, offset]);
    }
    public static LevelEvent CopyEvent(LevelEvent @event, int seqId) {
        return (LevelEvent) CopyEventMethod.Invoke(scnEditor.instance, [@event, seqId]);
    }
    public static string ConvertEmptyToNaN(string s) {
        var instance = UnityEngine.Object.FindObjectOfType<PropertyControl_Vector2>();
        return (string)convertEmptyToNaNMethod.Invoke(instance, new object[] { s });
    }


    public static string ConvertNaNToEmpty(string s) {
        var instance = UnityEngine.Object.FindObjectOfType<PropertyControl_Vector2>();
        return (string)convertNaNToEmptyMethod.Invoke(instance, new object[] { s });
    }

    public static void UpdateSteamCallbacks() {
        updateSteamCallbacksMethod.Invoke(scnEditor.instance, null);
    }
    public static void QuitToMenu() {
        quitToMenuMethod.Invoke(scnEditor.instance, null);
    }
    public static void UpdateSelectedFloor() {
        updateSelectedFloorMethod.Invoke(scnEditor.instance, null);
    }
    public static void OttoUpdate() {
        ottoUpdateMethod.Invoke(scnEditor.instance, null);
    }
    public static void SaveBackup() {
        saveBackupMethod.Invoke(scnEditor.instance, null);
    }
    public static void HandleKeyboardActions() {
        handleKeyboardActionsMethod.Invoke(scnEditor.instance, null);
    }
    public static void HandleMouseActions() {
        handleMouseActionsMethod.Invoke(scnEditor.instance, null);
    }
    public static bool paused() {
        return (bool) pausedProperty.GetValue(scnEditor.instance);
    }
}