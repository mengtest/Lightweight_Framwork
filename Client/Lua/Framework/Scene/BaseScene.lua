---
--- Generated by EmmyLua(https://github.com/EmmyLua)
--- Created by shenyi.
--- DateTime: 2020/3/20 15:43

---@class BaseScene
local BaseScene = BaseClass("BaseScene", Updatable)
local base = Updatable

function BaseScene:OnCreate(scene_config) end
function BaseScene:ctor(scene_config)
	self:OnCreate(scene_config)
end

---预加载资源，预加载结束之后再关闭loadingui
function BaseScene:OnPrepare(...) end
function BaseScene:Prepare(...)
	return self:OnPrepare(...)
end

function BaseScene:OnEnter() end
function BaseScene:Enter()
	self:OnEnter()
end

function BaseScene:OnLeave() end
function BaseScene:Leave()
	self:OnLeave()
end

function BaseScene:OnDestroy() end
function BaseScene:dtor()
	self:OnDestroy()
end

return BaseScene