---@class AOIController:Controller
local AOIController = BaseClass("AOIController", Controller)
------------------------------------------------------------------------------
local EntityType = EntityType
local Player = require("Logic/Entity/Behaviour/Player")
local Hero = require("Logic/Entity/Behaviour/Hero")
local Monster = require("Logic/Entity/Behaviour/Monster")
------------------------------------------------------------------------------
local world_conig = require "Logic/Config/World"
------------------------------------------------------------------------------
function AOIController:ctor()
    ---@type table<string, Entity> @private
    self.entites_map = {}
    self:AddMessageListener(NetMsgId.s2c_create_entities, self.CreateEntitieEx, self)
    self:AddMessageListener(NetMsgId.s2c_delete_entities, self.RemoveEntities, self)
end

function AOIController:CreateEntitieEx(args)
    self:CreateEntities(args.data)
end

---@param args table<number, AOIData>
function AOIController:CreateEntities(args)
    local sceneId = RoleModel.RoleAttrib.sceneId
    local sceneName = world_conig[sceneId].Resource
    local scene_config = require("Logic/Config/" .. sceneName)

    local roleId  = RoleModel.roleId
    for aoidId, aoiData in pairs(args) do
        if self.entites_map[aoidId] then
            logError("已经创建:" .. aoiData.aoiId)
        else
            local entity
            if aoiData.attrib.roleId == roleId then
                aoiData.attrib.type = EntityType.hero
                entity = Hero.New(aoiData)
                MainCamera:SetTarget(entity.behavior.transform)
            elseif aoiData.attrib.type == EntityType.player then
                entity = Player.New(aoiData)
            else
                local element = scene_config[aoiData.attrib.elementId]
                if aoiData.attrib.type == EntityType.monster then
                    entity = Monster.New(aoiData, element)
                end
            end
            self.entites_map[aoidId] = entity
        end

    end
end

---@param args table<number, Entity>
function AOIController:RemoveEntities(args)
    for _, aoiId in ipairs(args.ids) do
        local entity = self.entites_map[aoiId]
        if entity then
            entity:Delete()
            self.entites_map[aoiId] = nil
        end
    end
end

---@return Entity
function AOIController:GetEntityByAoiId(uid)
    return self.entites_map[uid]
end

function AOIController:DestroyEntityByAoiId(uid)
    local entity = self.entites_map[uid]
    if entity then
        entity:Delete()
        self.entites_map[uid] = nil
    end
end

--自动寻路到场景中elementID的元素
function AOIController:SimplePosToTarget(elementID, callBack)

end

return AOIController