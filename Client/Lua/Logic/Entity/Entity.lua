--- Generated by EmmyLua(https://github.com/EmmyLua)
--- Created by shenyi.
--- DateTime: 2020/8/13
-----------------------------------------------------------

---@class Entity:Updatable
Entity = BaseClass("Entity", Updatable)
-----------------------------------------------------------
---@param aoiData AOIData
function Entity:ctor(aoiData)
    local sceneId = RoleModel.RoleData.sceneId
    ---@type AOIData
    self.aoiData = aoiData
    ---@type Game.EntityBehavior
    self.behavior = EntityBehaviorManager.CreateEntity(
            sceneId,
            aoiData.aoiId,
            aoiData.attrib.modelId,
            { x = aoiData.trans.pos_x, y = aoiData.trans.pos_y, z = aoiData.trans.pos_z },
            aoiData.trans.forward,
            aoiData.attrib.type,
            function(components)
                coroutine.start(function()
                    self.animComp = components[0]
                    self.rotateComp = components[1]
                    self:OnBodyCreate(components)
                end)
            end)

    ---@type UnityEngine.GameObject
    self.gameObject = self.behavior.gameObject
    ---@type UnityEngine.Transform
    self.transform = self.behavior.transform
end

---@ 实体元素创建的时候回调
function Entity:OnBodyCreate(components) end

function Entity:dtor()
    self.behavior:RemoveAllListeners()
    EntityBehaviorManager.DestroyEntity(self.aoiData.aoiId)
end