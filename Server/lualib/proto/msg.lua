local msg = [[

.mainAttrib {
	roleId 0 : string
	name 1 : string
	level 2 : integer
	exp 3 : integer
	vip 4 : integer
	totalFight 5 : integer
	progress 6 : integer
	sceneId 7 : integer
	headIconId 8 : integer
	headFrameId 9 : integer

	crystal 10 : integer
	gold 11 : integer
	silver 12 : integer
	energy 13 : integer
	achive 14 : integer
	guide 15 : integer
	guildEnable 16 : integer
	vipExp 17 : integer
	vipGift 18 : integer
	mouthCard 19 : integer
	modelId 20 : integer
	guildId 21 : integer
	daySign 22 : integer
}

.hero {
	id 0 : string
	configId 1 : integer
	level 2 : integer
	star 3 : integer
	totalFight 4 : integer
}

.equipExtraAttrib {
	key 0 : integer
	value 1 : integer
	des 2 : string
}

.item {
	id 0 : string
	configId 1 : integer
	mainType 2 : integer
	count 3 : integer
	extraAttrib 4 : *equipExtraAttrib
}

.roleInfo {
    account 0 : string
    attrib 1 : mainAttrib
	heroPackage 2 : *hero(id)
	itemPackage 3 : *item(id)
}

.req_login {
	uid 0 : string
}

.ack_login {
	roleInfo 0 : roleInfo
}

.req_register {
	nickname 0 : string
	channel 1 : integer
}

.ack_register {
	error 0 : integer
	roleInfo 1 : roleInfo
}

.sync_attrib {
    paramId 0 : string
    type 1 : integer
    name 2 : string
    modelId 3 : integer
}

.sync_trans {
    pos_x 0 :double
    pos_y 1 :double
    pos_z 2 :double
    forward 3 :double
}

.aoiAttrib {
    aoiId 0 : integer
    attrib 1 : sync_attrib
}

.aoiTrans {
    aoiId 0 : integer
    trans 1 : sync_trans
}

.aoiData {
    aoiId 0 : integer
    attrib 1 : sync_attrib
    trans 5 : sync_trans
}

.ack_enter_game {
    ok 0 : boolean
    aoi_map 1 : *aoiData(aoiId)
}

.c2s_sync_trans {
    trans 0 : sync_trans
}


.s2c_create_entities{
    data 0 : *aoiData(aoiId)
}

.s2c_delete_entities{
    ids 0 : *integer
}

.s2c_aoi_trans {
    list 0 : *aoiTrans(aoiId)
}

]]

return msg