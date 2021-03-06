// ========================================================
// des：
// author: 
// time：2020-12-10 09:58:19
// version：1.0
// ========================================================

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace TMPro.EditorUtilities
{
	public class TMPro_CreateObjectMenuEx
	{

		/// <summary>
		/// Create a TextMeshPro object that works with the Mesh Renderer
		/// </summary>
		/// <param name="command"></param>
		[MenuItem("GameObject/3D Object/Text - TextMeshPro", false, 30)]
		static void CreateTextMeshProObjectPerform(MenuCommand command)
		{
			GameObject go = new GameObject("Text (TMP)");

			// Add support for new prefab mode
			StageUtility.PlaceGameObjectInCurrentStage(go);

			TextMeshPro textMeshPro = go.AddComponent<TextMeshPro>();
			textMeshPro.text = "Sample text";
			textMeshPro.alignment = TextAlignmentOptions.TopLeft;

			Undo.RegisterCreatedObjectUndo((Object)go, "Create " + go.name);

			GameObject contextObject = command.context as GameObject;
			if (contextObject != null)
			{
				GameObjectUtility.SetParentAndAlign(go, contextObject);
				Undo.SetTransformParent(go.transform, contextObject.transform, "Parent " + go.name);
			}

			Selection.activeGameObject = go;
		}


		/// <summary>
		/// Create a TextMeshPro object that works with the CanvasRenderer
		/// </summary>
		/// <param name="command"></param>
		[MenuItem("GameObject/UI/New/UIText", false, 100)]
		static void CreateTextMeshProGuiObjectPerform(MenuCommand menuCommand)
		{
			GameObject go = TMP_DefaultControlsEx.CreateText(GetStandardResources());

			// Override text color and font size
			TMP_Text textComponent = go.GetComponent<TMP_Text>();
			textComponent.color = Color.white;
			//if (textComponent.m_isWaitingOnResourceLoad == false)
			//	textComponent.fontSize = TMP_Settings.defaultFontSize;
			PlaceUIElementRoot(go, menuCommand);
		}

		[MenuItem("GameObject/UI/New/UIImage", false, 101)]
		static public void CreateImage(MenuCommand menuCommand)
		{
			GameObject go = TMP_DefaultControlsEx.CreateImage(GetStandardResources());
			PlaceUIElementRoot(go, menuCommand);
		}

		[MenuItem("GameObject/UI/New/UIButton", false, 102)]
		static public void AddButton(MenuCommand menuCommand)
		{
			GameObject go = TMP_DefaultControlsEx.CreateButton(GetStandardResources());

			// Override font size
			TMP_Text textComponent = go.GetComponentInChildren<TMP_Text>();
			textComponent.fontSize = 24;

			PlaceUIElementRoot(go, menuCommand);
		}


		[MenuItem("GameObject/UI/New/UIToggle", false, 103)]
		static public void AddToggle(MenuCommand menuCommand)
		{
			GameObject go = TMP_DefaultControlsEx.CreateToggle(GetStandardResources());

			// Override font size
			TMP_Text textComponent = go.GetComponentInChildren<TMP_Text>();
			textComponent.fontSize = 24;

			PlaceUIElementRoot(go, menuCommand);
		}

		[MenuItem("GameObject/UI/New/UISlider", false, 104)]
		static public void AddSlider(MenuCommand menuCommand)
		{
			GameObject go = TMP_DefaultControlsEx.CreateSlider(GetStandardResources());

			PlaceUIElementRoot(go, menuCommand);
		}


		[MenuItem("GameObject/UI/New/UIInput", false, 105)]
		static void AddTextMeshProInputField(MenuCommand menuCommand)
		{
			GameObject go = TMP_DefaultControlsEx.CreateInputField(GetStandardResources());
			PlaceUIElementRoot(go, menuCommand);
		}


		[MenuItem("GameObject/UI/New/UIDropdown", false, 111)]
		static public void AddDropdown(MenuCommand menuCommand)
		{
			GameObject go = TMP_DefaultControlsEx.CreateDropdown(GetStandardResources());
			PlaceUIElementRoot(go, menuCommand);
		}

		[MenuItem("GameObject/UI/New/UIPanel", false, 106)]
		static public void AddPanel(MenuCommand menuCommand)
		{
			GameObject go = TMP_DefaultControlsEx.CreatePanel(GetStandardResources());
			PlaceUIElementRoot(go, menuCommand);
			RectTransform rect = go.GetComponent<RectTransform>();
			rect.anchoredPosition = Vector2.zero;
			rect.sizeDelta = Vector2.zero;
		}


		private const string kUILayerName = "UI";

		private const string kStandardSpritePath = "UI/Skin/UISprite.psd";
		private const string kBackgroundSpritePath = "UI/Skin/Background.psd";
		private const string kInputFieldBackgroundPath = "UI/Skin/InputFieldBackground.psd";
		private const string kKnobPath = "UI/Skin/Knob.psd";
		private const string kCheckmarkPath = "UI/Skin/Checkmark.psd";
		private const string kDropdownArrowPath = "UI/Skin/DropdownArrow.psd";
		private const string kMaskPath = "UI/Skin/UIMask.psd";

		static private TMP_DefaultControlsEx.Resources s_StandardResources;


		static private TMP_DefaultControlsEx.Resources GetStandardResources()
		{
			if (s_StandardResources.standard == null)
			{
				s_StandardResources.standard = AssetDatabase.GetBuiltinExtraResource<Sprite>(kStandardSpritePath);
				s_StandardResources.background = AssetDatabase.GetBuiltinExtraResource<Sprite>(kBackgroundSpritePath);
				s_StandardResources.inputField = AssetDatabase.GetBuiltinExtraResource<Sprite>(kInputFieldBackgroundPath);
				s_StandardResources.knob = AssetDatabase.GetBuiltinExtraResource<Sprite>(kKnobPath);
				s_StandardResources.checkmark = AssetDatabase.GetBuiltinExtraResource<Sprite>(kCheckmarkPath);
				s_StandardResources.dropdown = AssetDatabase.GetBuiltinExtraResource<Sprite>(kDropdownArrowPath);
				s_StandardResources.mask = AssetDatabase.GetBuiltinExtraResource<Sprite>(kMaskPath);
			}
			return s_StandardResources;
		}


		private static void SetPositionVisibleinSceneView(RectTransform canvasRTransform, RectTransform itemTransform)
		{
			// Find the best scene view
			SceneView sceneView = SceneView.lastActiveSceneView;
			if (sceneView == null && SceneView.sceneViews.Count > 0)
				sceneView = SceneView.sceneViews[0] as SceneView;

			// Couldn't find a SceneView. Don't set position.
			if (sceneView == null || sceneView.camera == null)
				return;

			// Create world space Plane from canvas position.
			Camera camera = sceneView.camera;
			Vector3 position = Vector3.zero;
			Vector2 localPlanePosition;

			if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRTransform, new Vector2(camera.pixelWidth / 2, camera.pixelHeight / 2), camera, out localPlanePosition))
			{
				// Adjust for canvas pivot
				localPlanePosition.x = localPlanePosition.x + canvasRTransform.sizeDelta.x * canvasRTransform.pivot.x;
				localPlanePosition.y = localPlanePosition.y + canvasRTransform.sizeDelta.y * canvasRTransform.pivot.y;

				localPlanePosition.x = Mathf.Clamp(localPlanePosition.x, 0, canvasRTransform.sizeDelta.x);
				localPlanePosition.y = Mathf.Clamp(localPlanePosition.y, 0, canvasRTransform.sizeDelta.y);

				// Adjust for anchoring
				position.x = localPlanePosition.x - canvasRTransform.sizeDelta.x * itemTransform.anchorMin.x;
				position.y = localPlanePosition.y - canvasRTransform.sizeDelta.y * itemTransform.anchorMin.y;

				Vector3 minLocalPosition;
				minLocalPosition.x = canvasRTransform.sizeDelta.x * (0 - canvasRTransform.pivot.x) + itemTransform.sizeDelta.x * itemTransform.pivot.x;
				minLocalPosition.y = canvasRTransform.sizeDelta.y * (0 - canvasRTransform.pivot.y) + itemTransform.sizeDelta.y * itemTransform.pivot.y;

				Vector3 maxLocalPosition;
				maxLocalPosition.x = canvasRTransform.sizeDelta.x * (1 - canvasRTransform.pivot.x) - itemTransform.sizeDelta.x * itemTransform.pivot.x;
				maxLocalPosition.y = canvasRTransform.sizeDelta.y * (1 - canvasRTransform.pivot.y) - itemTransform.sizeDelta.y * itemTransform.pivot.y;

				position.x = Mathf.Clamp(position.x, minLocalPosition.x, maxLocalPosition.x);
				position.y = Mathf.Clamp(position.y, minLocalPosition.y, maxLocalPosition.y);
			}

			itemTransform.anchoredPosition = position;
			itemTransform.localRotation = Quaternion.identity;
			itemTransform.localScale = Vector3.one;
		}


		private static void PlaceUIElementRoot(GameObject element, MenuCommand menuCommand)
		{
			GameObject parent = menuCommand.context as GameObject;
			bool explicitParentChoice = true;
			if (parent == null)
			{
				parent = GetOrCreateCanvasGameObject();
				explicitParentChoice = false;

				// If in Prefab Mode, Canvas has to be part of Prefab contents,
				// otherwise use Prefab root instead.
				PrefabStage prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
				if (prefabStage != null && !prefabStage.IsPartOfPrefabContents(parent))
					parent = prefabStage.prefabContentsRoot;
			}
			if (parent.GetComponentInParent<Canvas>() == null)
			{
				// Create canvas under context GameObject,
				// and make that be the parent which UI element is added under.
				GameObject canvas = CreateNewUI();
				canvas.transform.SetParent(parent.transform, false);
				parent = canvas;
			}

			// Setting the element to be a child of an element already in the scene should
			// be sufficient to also move the element to that scene.
			// However, it seems the element needs to be already in its destination scene when the
			// RegisterCreatedObjectUndo is performed; otherwise the scene it was created in is dirtied.
			SceneManager.MoveGameObjectToScene(element, parent.scene);

			if (element.transform.parent == null)
			{
				Undo.SetTransformParent(element.transform, parent.transform, "Parent " + element.name);
			}

			GameObjectUtility.EnsureUniqueNameForSibling(element);

			// We have to fix up the undo name since the name of the object was only known after reparenting it.
			Undo.SetCurrentGroupName("Create " + element.name);

			GameObjectUtility.SetParentAndAlign(element, parent);
			if (!explicitParentChoice) // not a context click, so center in sceneview
				SetPositionVisibleinSceneView(parent.GetComponent<RectTransform>(), element.GetComponent<RectTransform>());

			Undo.RegisterCreatedObjectUndo(element, "Create " + element.name);

			Selection.activeGameObject = element;
		}


		static public GameObject CreateNewUI()
		{
			// Root for the UI
			var root = new GameObject("Canvas");
			root.layer = LayerMask.NameToLayer(kUILayerName);
			Canvas canvas = root.AddComponent<Canvas>();
			canvas.renderMode = RenderMode.ScreenSpaceOverlay;
			root.AddComponent<CanvasScaler>();
			root.AddComponent<GraphicRaycaster>();

			// Works for all stages.
			StageUtility.PlaceGameObjectInCurrentStage(root);
			bool customScene = false;
			PrefabStage prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
			if (prefabStage != null)
			{
				root.transform.SetParent(prefabStage.prefabContentsRoot.transform, false);
				customScene = true;
			}

			Undo.RegisterCreatedObjectUndo(root, "Create " + root.name);

			// If there is no event system add one...
			// No need to place event system in custom scene as these are temporary anyway.
			// It can be argued for or against placing it in the user scenes,
			// but let's not modify scene user is not currently looking at.
			if (!customScene)
				CreateEventSystem(false);
			return root;
		}


		private static void CreateEventSystem(bool select)
		{
			CreateEventSystem(select, null);
		}


		private static void CreateEventSystem(bool select, GameObject parent)
		{
			var esys = Object.FindObjectOfType<EventSystem>();
			if (esys == null)
			{
				var eventSystem = new GameObject("EventSystem");
				GameObjectUtility.SetParentAndAlign(eventSystem, parent);
				esys = eventSystem.AddComponent<EventSystem>();
				eventSystem.AddComponent<StandaloneInputModule>();

				Undo.RegisterCreatedObjectUndo(eventSystem, "Create " + eventSystem.name);
			}

			if (select && esys != null)
			{
				Selection.activeGameObject = esys.gameObject;
			}
		}


		// Helper function that returns a Canvas GameObject; preferably a parent of the selection, or other existing Canvas.
		static public GameObject GetOrCreateCanvasGameObject()
		{
			GameObject selectedGo = Selection.activeGameObject;

			// Try to find a gameobject that is the selected GO or one if its parents.
			Canvas canvas = (selectedGo != null) ? selectedGo.GetComponentInParent<Canvas>() : null;
			if (IsValidCanvas(canvas))
				return canvas.gameObject;

			// No canvas in selection or its parents? Then use any valid canvas.
			// We have to find all loaded Canvases, not just the ones in main scenes.
			Canvas[] canvasArray = StageUtility.GetCurrentStageHandle().FindComponentsOfType<Canvas>();
			for (int i = 0; i < canvasArray.Length; i++)
				if (IsValidCanvas(canvasArray[i]))
					return canvasArray[i].gameObject;

			// No canvas in the scene at all? Then create a new one.
			return CreateNewUI();
		}

		static bool IsValidCanvas(Canvas canvas)
		{
			if (canvas == null || !canvas.gameObject.activeInHierarchy)
				return false;

			// It's important that the non-editable canvas from a prefab scene won't be rejected,
			// but canvases not visible in the Hierarchy at all do. Don't check for HideAndDontSave.
			if (EditorUtility.IsPersistent(canvas) || (canvas.hideFlags & HideFlags.HideInHierarchy) != 0)
				return false;

			if (StageUtility.GetStageHandle(canvas.gameObject) != StageUtility.GetCurrentStageHandle())
				return false;

			return true;
		}
	}
}
