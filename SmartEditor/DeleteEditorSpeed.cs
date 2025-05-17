using JALib.Core;
using JALib.Core.Patch;
using UnityEngine;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using SFB;
using SmartEditor.FixLoad.CustomSaveState;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SmartEditor;

public class DeleteEditorSpeed() : Feature(Main.Instance, nameof(DeleteEditorSpeed), patchClass: typeof(DeleteEditorSpeed)) {
    [JAPatch(typeof(scnEditor), "Update", PatchType.Replace, false)]
    public static void scnEditor_Update(scnEditor __instance, bool ___refreshBgSprites, bool ___refreshDecSprites, ref float ___backupTimer) {
        if(!scnGame.instance || ADOBase.controller.pauseMenu.gameObject.activeSelf) return;
        if(ADOBase.controller.paused && AsyncInputManager.isActive) ADOBase.controller.UpdateInput();
        __instance.thumbnailMaker.gameObject.SetActive(true);
        if(StandaloneFileBrowser.lastFrameCount == Time.frameCount) return;
        FixPrivateMethod.UpdateSteamCallbacks();
        if(RDC.runningOnSteamDeck && !__instance.steamDeckWarningPassed && RDInput.cancelPress) FixPrivateMethod.QuitToMenu();
        FixPrivateMethod.UpdateSelectedFloor();
        FixPrivateMethod.OttoUpdate();
        if(___refreshBgSprites) __instance.UpdateBackgroundSprites();
        if(___refreshDecSprites) __instance.UpdateDecorationObjects();
        if(Input.GetKeyDown(KeyCode.Escape) && !scrController.instance.paused) __instance.SwitchToEditMode();
        else {
            if(Time.unscaledTime > ___backupTimer + (double) __instance.backupInterval) {
                ___backupTimer = Time.unscaledTime;
                FixPrivateMethod.SaveBackup();
            }
            if(__instance.eventSystem.currentInputModule is CustomStandaloneInputModule currentInputModule) {
                PointerEventData pointerData = currentInputModule.GetPointerData();
                bool flag = false;
                if(pointerData != null && pointerData.pointerCurrentRaycast.module) {
                    GameObject gameObject = pointerData.pointerCurrentRaycast.gameObject;
                    if(gameObject) {
                        Transform transform = gameObject.transform;
                        while(!transform.TryGetComponent(out ScrollRect _)) {
                            transform = transform.parent;
                            if(!transform) goto label_23;
                        }
                        flag = true;
                    }
                }
                label_23:
                if(__instance.prefsContainer.gameObject.activeInHierarchy || __instance.particleEditorContainer.gameObject.activeInHierarchy)
                    flag = true;
                if(!flag) {
                    Vector2 mouseScrollDelta = RDInput.mouseScrollDelta;
                    if(Mathf.Abs(mouseScrollDelta.y) > 0.05000000074505806) __instance.ZoomCamera(mouseScrollDelta.y, !Persistence.editorUseLegacyZoom);
                }
            }
            __instance.selectingFloorIDText.gameObject.SetActive(scnEditor.selectingFloorID);
            if(__instance.userIsEditingAnInputField) scnEditor.selectingFloorID = false;
            if(!__instance.selectingFloorIDTextMoving) {
                __instance.selectingFloorIDRectTransform.DOAnchorPosY(112.12f, 0.3f).SetEase<TweenerCore<Vector2, Vector2, VectorOptions>>(Ease.OutExpo).SetUpdate<TweenerCore<Vector2, Vector2, VectorOptions>>(true);
                __instance.selectingFloorIDTextMoving = true;
            }
            if(!scnEditor.selectingFloorID) __instance.selectingFloorIDRectTransform.PositionY(0.0f);
            if(__instance.playMode) return;
            FixPrivateMethod.HandleKeyboardActions();
            FixPrivateMethod.HandleMouseActions();
        }

    }


    [JAPatch(typeof(EditorSpeedIndicator), "UpdatePercentText", PatchType.Replace, false)]
    public static void EditorSpeedIndicator_UpdatePercentText() { }

    [JAPatch(typeof(EditorSpeedIndicator), "ShiftSpeed", PatchType.Replace, false)]
    public static void EditorSpeedIndicator_ShiftSpeed() { }
}