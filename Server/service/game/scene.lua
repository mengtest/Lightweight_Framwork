local skynet = require "skynet"
local snax = require "skynet.snax"
local mc = require "skynet.multicast"

local event_names = event_names
---@type entityMgr
local entityMgr = require "entity.entityMgr"

---@type gameconfig
local config
local channel
local sceneConfig
---@class Scene_Req
local response = response
---@class Scene_Post
local accept = accept
---@class SceneParam
local sceneInfo = {}
---@type table <string, SceneRoleInfo>
local role_map = {}


local function update()
    local deltaTime = 1 / config.fps
    local tick = 100 * deltaTime
    while true do
        local alive_map = entityMgr:get_alive_map()
        for _, entity in pairs(alive_map) do
            entity:update(deltaTime)
        end

        local entity_map = entityMgr:get_sync_info()
        if table.size(entity_map) > 0 then
            --TODO: 序列化完之后再发？
            channel:publish(event_names.scene.s2c_aoi_trans, entity_map)
        end

        local create_map = entityMgr:get_create_map()
        if table.size(create_map) > 0 then
            channel:publish(event_names.scene.create_entities, create_map)
        end

        local delete_map = entityMgr:get_delete_map()
        if table.size(delete_map) > 0 then
            channel:publish(event_names.scene.delete_entities, delete_map)
        end

        skynet.sleep(tick)
    end
end

function init( ... )
    local start_arge = {...}
    local sceneId = start_arge[1]
    local sceneName = start_arge[2]
    config = require(skynet.getenv("config"))
    sceneConfig = require("config/" .. sceneName)
    ---创建场景组播频道
    channel = mc.new()
    ---创建场景信息表
    sceneInfo.sceneId = sceneId
    sceneInfo.sceneName = sceneName
    sceneInfo.serviceName = SERVICE_NAME
    sceneInfo.handle = skynet.self()
    sceneInfo.channel = channel.channel
    ---创建场景元素
    entityMgr:create_elements(sceneConfig)
    ---开启场景主循环
    skynet.fork(update)
end

function response.get_scene_param()
    return sceneInfo
end

---@param roleInfo RoleInfo
function response.role_enter_scene(agent, roleInfo)
    local roleId = roleInfo.roleId
    
    if role_map[roleId] then skynet.error("玩家已经在场景中") return false end
    local role = entityMgr:create_player(roleInfo)
    ---@class SceneRoleInfo
    local roleInfo =
    {
        agent = agent,
        role = role
    }
    role_map[roleId] = roleInfo

    local aoi_map = entityMgr:get_all_aoiData()
    return true, role.aoiData.aoiId, aoi_map
end

function response.role_leave_scene(roleId)
    local roleInfo = role_map[roleId]
    if not roleInfo then skynet.error("玩家不在场景中" .. roleId) return false end
    local aoiId = roleInfo.role.aoiData.aoiId
    entityMgr:remove_entity(aoiId)
    role_map[roleId] = nil

    return true
end

---@param args Sync_Trans
function accept.c2s_sync_trans(roleId, args)
    local roleInfo = role_map[roleId]
    if not roleInfo then skynet.error("玩家不在场景中" .. roleId) return false end
    entityMgr:c2s_sync_trans(roleInfo.role.aoiData.aoiId, args)
end


function exit( ... )

    --TODO 处理channel
end
