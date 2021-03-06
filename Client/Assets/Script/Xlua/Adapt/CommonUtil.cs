﻿using UnityEngine;
using UnityEngine.EventSystems;

namespace Game
{
    public static class CommonUtil
    {
        public static void SetPos(GameObject obj, float x, float y, float z)
        {
            obj.transform.position = new Vector3(x, y, z);
        }

        public static void SetPos(Transform trans, float x, float y, float z)
        {
            trans.position = new Vector3(x, y, z);
        }

        public static void SetLocalPos(GameObject obj, float x, float y, float z)
        {
            obj.transform.localPosition = new Vector3(x, y, z);
        }

        public static void SetLocalPos(Transform trans, float x, float y, float z)
        {
            trans.localPosition = new Vector3(x, y, z);
        }

        public static void SetRot(GameObject obj, float x, float y, float z)
        {
            obj.transform.eulerAngles = new Vector3(x, y, z);
        }

        public static void SetRot(Transform trans, float x, float y, float z)
        {
            trans.eulerAngles = new Vector3(x, y, z);
        }

        public static void SetLocalRot(GameObject obj, float x, float y, float z)
        {
            obj.transform.localRotation = Quaternion.Euler(x, y, z);
        }

        public static void SetLocalRot(Transform trans, float x, float y, float z)
        {
            trans.localRotation = Quaternion.Euler(x, y, z);
        }

        public static void SetLocalScale(GameObject obj, float x, float y, float z)
        {
            obj.transform.localScale = new Vector3(x, y, z);
        }

        //public static void SetParent(GameObject parent, GameObject child)
        //{
        //    child.transform.SetParent(parent.transform);
        //    child.transform.localPosition = Vector3.zero;
        //    child.transform.localRotation = Quaternion.identity;
        //    child.transform.localScale = Vector3.one;
        //}

		public static void SetAsLastSibling(GameObject go)
		{
			go.transform.SetAsLastSibling();
		}

		public static void SetAsLastSibling(Transform trans)
		{
			trans.SetAsLastSibling();
		}

		public static void SetAsFirstSibling(GameObject go)
		{
			go.transform.SetAsFirstSibling();
		}

		public static void SetAsFirstSibling(Transform trans)
		{
			trans.SetAsFirstSibling();
		}

		/// <summary>
		/// 设置层级
		/// </summary>
		/// <param name="go"></param>
		/// <param name="layer"></param>
		public static void SetLayerRecursively(GameObject go, int layer)
        {
            go.layer = layer;
            Transform t = go.transform;
            for (int i = 0; i < t.childCount; i++)
                SetLayerRecursively(t.GetChild(i).gameObject, layer);
        }

        public static void SetLayerRecursively(Transform trans, int layer)
        {
            SetLayerRecursively(trans.gameObject, layer);
        }

        /// <summary>
        /// 设置tag
        /// </summary>
        /// <param name="go"></param>
        /// <param name="tag"></param>
        public static void SetTagRecursively(GameObject go, string tag)
        {
            go.tag = tag;
            Transform t = go.transform;
            for (int i = 0; i < t.childCount; i++)
                SetTagRecursively(t.GetChild(i).gameObject, tag);
        }

        public static void SetTagRecursively(Transform trans, string tag)
        {
            SetTagRecursively(trans.gameObject, tag);
        }

        public static GameObject FindGo(string path)
        {
            return GameObject.Find(path);
        }

		public static Transform FindTrans(string path)
		{
			return GameObject.Find(path).transform;
		}

		public static GameObject FindGo(GameObject go, string path)
		{
			return go.transform.Find(path).gameObject;
		}

		public static Transform FindTrans(GameObject go, string path)
        {
            return go.transform.Find(path);
        }

        public static GameObject FindGo(Transform trans, string path)
        {
            return trans.Find(path).gameObject;
        }

		public static Transform FindTrans(Transform trans, string path)
		{
			return trans.Find(path);
		}


		public static Animation FindAnimation(GameObject go, string path = null)
        {
            return go.transform.FindComponent<Animation>(path);
        }

        public static Animation FindAnimation(Transform trans, string path = null)
        {
            return trans.FindComponent<Animation>(path);
        }

		/// <summary>
		/// 游戏点击事件 0 无点击 1 点击UI上 2 点击在地面上 3 点击不在地面 不在场景上
		/// 使用射线只有在寻路网格上点击才有效
		/// </summary>
		public static int IsPointerOnUI()
		{
#if !UNITY_EDITOR && (UNITY_IPHONE || UNITY_IOS || UNITY_ANDROID)
        if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began)
#else
			if (Input.GetMouseButton(0))
#endif
			{
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				RaycastHit hit;
				if (Physics.Raycast(ray, out hit))
				{

#if !UNITY_EDITOR && (UNITY_IPHONE || UNITY_IOS || UNITY_ANDROID)
                    if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
#else
					if (EventSystem.current.IsPointerOverGameObject())
#endif
					{
						return 1;
					}
					else
					{
						if (hit.collider.CompareTag("TerrainGeometry"))
						{
							return 2;
						}
						else
						{
							return 3;
						}
					}
				}
				return 1;

			}
			return 0;
		}
	}

}