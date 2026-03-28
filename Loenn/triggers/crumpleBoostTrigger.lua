local triggers = require('triggers')
local lang = require('language_registry')

local crumpleBoostTrigger = {}

crumpleBoostTrigger.name = 'SlipperiestNook/CrumpleBoostTrigger'
crumpleBoostTrigger.placements = {
    {
        name = 'enable',
        data = {
            enable = true
        }
    },
    {
        name = 'disable',
        data = {
            enable = false
        }
    }
}

local printed = false

crumpleBoostTrigger.triggerText = function(room, trigger)
    local mode = trigger.enable and 'enable' or 'disable'

    local defaultText = triggers.getDrawableDisplayText(trigger)
    local modeText = tostring(lang.getLanguage().triggers[trigger._name].mode[mode])

    return string.format('%s (%s)', defaultText, modeText)
end

return crumpleBoostTrigger
