using System.Reflection;
using ADOFAI;
using ADOFAI.LevelEditor.Controls;
using JALib.Tools;

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
        return (string)convertEmptyToNaNMethod.Invoke(scnEditor.instance, [s]);
    }
    public static string ConvertNaNToEmpty(string s) {
        return (string)convertNaNToEmptyMethod.Invoke(scnEditor.instance, [s]);
    }
}