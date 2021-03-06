// ========================================================
// des：
// author: shenyi
// time：2020-12-10 19:16:30
// version：1.0
// ========================================================

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using XLua;

namespace Game
{
	public class EntityBehaviorManager
	{
		/// <summary>
		/// 实体对象池
		/// </summary>
		/// <typeparam name="EntityBehavior"></typeparam>
		/// <returns></returns>
		private static MonoObjectPool<EntityBehavior> entitiesPool = new MonoObjectPool<EntityBehavior>(AllocEntity);

		/// <summary>
		/// 按照uid存取
		/// </summary>
		/// <typeparam name="string"></typeparam>
		/// <typeparam name="EntityBehavior"></typeparam>
		/// <returns></returns>
		private static Dictionary<long, EntityBehavior> entityBehaviors = new Dictionary<long, EntityBehavior>();
		/// <summary>
		/// 用于按照顺序遍历
		/// </summary>
		/// <typeparam name="EntityBehavior"></typeparam>
		/// <returns></returns>
		private static LinkedList<EntityBehavior> entityBehaviorsQueue = new LinkedList<EntityBehavior>();

		private static GameObject m_EntityContainer;
		private static GameObject entityContainer
		{
			get
			{
				if (m_EntityContainer == null) m_EntityContainer = new GameObject("Models");
				m_EntityContainer.transform.localScale = Vector3.one;
				return m_EntityContainer;
			}
		}

		private static EntityBehavior AllocEntity()
		{
			GameObject go = new GameObject("Entity");
			EntityBehavior behavior = go.AddComponent<EntityBehavior>();
			return behavior;
		}

		/// <summary>
		/// 创建实体
		/// </summary>
		/// <param name="sceneid"></param>
		/// <param name="uid"></param>
		/// <param name="proxyId"></param>
		/// <param name="res">预设体AddressableKey</param>
		/// <param name="bornPos">出生位置</param>
		/// <param name="orientation">方向</param>
		/// <param name="scale">缩放</param>
		/// <param name="entityType">实体类型</param>
		/// <param name="onBodyCreated">实体的组件</param>
		/// <returns></returns>
		public static EntityBehavior CreateEntity(int sceneid, int aoiId, int resId, Vector3 bornPos, float orientation, int entityType, Action<LuaTable> onBodyCreated = null)
		{
			if (entityBehaviors.ContainsKey(aoiId))
			{
				Debug.LogError("CreateEntity error exist uid=" + aoiId);
				return null;
			}

			EntityBehavior entity = entitiesPool.Alloc();
			if (entity == null)
				Debug.LogError("entity is nil");

			// 设置父物体
			GameObject parent = entity.gameObject;
			if (!parent.activeSelf)
				parent.SetActive(true);
			parent.transform.localScale = Vector3.one;
			parent.transform.SetParent(entityContainer.transform, false);
			parent.transform.position = bornPos;
			parent.transform.rotation = Quaternion.Euler(0, orientation, 0);

			entity.Init(sceneid, aoiId, resId, entityType, onBodyCreated);
			// 把实体放到容器中
			entityBehaviors.Add(aoiId, entity);
			entityBehaviorsQueue.AddFirst(entity);

			return entity;
		}



		private static async void CreateBody(EntityBehavior entity)
		{
			entity.bodyLoading = true;
			int uid = entity.AoiId;
			ModelConfig config = ModelConfigAsset.Get(entity.ResId);
			if (config == null)
				return;
			var obj = await ResourceManager.LoadPrefabFromePool(config.Resource);
			if (!obj) return;
			if (entity == null || uid != entity.AoiId || entity.bodyLoading == false)
			{
				ResourceManager.RecyclePrefab(obj);
				return;
			}

			EntityBehavior p = GetEntity(entity.AoiId);
			if (p != null && p != entity)
			{
				Debug.LogError("这个错误可以无视 保留查看而已 entity create error");
				ResourceManager.RecyclePrefab(obj);
				return;
			}

			GameObject go = obj as GameObject;
			if (p != null)
			{
				go.SetActive(true);
				go.name = "@Body_Entity@";
				p.Body = go;
				go.transform.SetParent(p.transform, false);
				go.transform.localPosition = Vector3.zero;
				go.transform.localRotation = Quaternion.identity;
				LayerMask defaultLayer = go.layer;
			
				p.InitComp(true);
				entity.bodyLoading = false;
			}
			else
			{
				ResourceManager.RecyclePrefab(go); // 只回收Body
				entity.bodyLoading = false;
			}
		}


		public static void Update()
		{
			var Enumerator = entityBehaviorsQueue.First;
			// 遍历队列中的所有实体
			while (Enumerator != null)
			{
				EntityBehavior entity = Enumerator.Value;
				if (entity.Body == null)
				{
					if (!entity.bodyLoading)
						CreateBody(entity);
				}
				else
				{
					if (entity.OnBodyCreate != null)
					{
						entity.OnBodyCreate(entity.CompTable);
						entity.OnBodyCreate = null;
					}
					else
					{
						entity.Update();
					}
				}
					
				Enumerator = Enumerator.Next;
			}
		}

		public static void FixedUpdate()
		{
			var Enumerator = entityBehaviorsQueue.First; ///Enumertor实体
			// 遍历队列中的所有实体
			while (Enumerator != null)
			{
				EntityBehavior entity = Enumerator.Value;
				if (entity.Body != null && entity.OnBodyCreate == null)
					entity.FixedUpdate();

				Enumerator = Enumerator.Next;
			}
		}

		/// <summary>
		/// 删除指定实体
		/// </summary>
		/// <param name="entity"></param>
		private static void DestroyEntity(EntityBehavior entity)
		{
			// 从队列移除(TODO:优化)
			entityBehaviorsQueue.Remove(entity);
			if (entity && entity.gameObject)
			{
				GameObject body = entity.Body;
				if (body != null)
					body.name = entity.gameObject.name;
				// 从对象池移除
				if (entity.bodyLoading) //强制删除免得回调重复赋值
					entitiesPool.Recycle(entity); // TODO 对象池提供一个的方法立即回收的方法
				else
					entitiesPool.Recycle(entity);
			}

		}

		/// <summary>
		/// 通过uid删除指定的实体
		/// </summary>
		/// <param name="aoiId"></param>
		public static void DestroyEntity(long aoiId)
		{
			if (entityBehaviors.ContainsKey(aoiId))
			{
				EntityBehavior entity = entityBehaviors[aoiId];
				entityBehaviors.Remove(aoiId);
				DestroyEntity(entity);
			}
			else
			{
				Debug.Log("DestroyEntity error not found uid=" + aoiId);
			}
		}

		/// <summary>
		/// 销毁所有Entity TODO
		/// </summary>
		/// <param name="uid"></param>
		public static void DestroyAllEntity()
		{
			List<long> ids = new List<long>();
			foreach (var item in entityBehaviors)
				ids.Add(item.Key);
			ids.ForEach(p => DestroyEntity(p));
		}

		public static EntityBehavior GetEntity(long uid)
		{
			EntityBehavior entity = null;
			entityBehaviors.TryGetValue(uid, out entity);
			return entity;
		}

		public static void Cleanup()
		{
			entitiesPool.Cleanup();
		}
	}
}