## Battlelog Status Notifier for Telegram

C# library using Telegram's Bot API (https://core.telegram.org/bots/api)
Monitors Battlelog (https://battlelog.battlefield.com) for user status changes

## Directions

* Install Telegram (https://telegram.org/), and create a bot by messaging @BotFather: https://core.telegram.org/bots#botfather

Edit HiddenSetting.config, and add your telegram bot API token, and google developer api information.
```XML
<?xml version="1.0" encoding="utf-8"?>
<appSettings>
    <!-- Create using BotFather: https://core.telegram.org/bots#botfather -->
    <add key="TelegramApiToken" value="*** Your Bot Api Key ***" />
    
    <!-- Your credentials @ https://battlelog.battlefield.com -->
    <add key="BattlelogUserName" value="*** Your Battlelog UserName ***" />
    <add key="BattlelogPassword" value="*** Your Battlelog Password ***" />

    <!-- 
    Should the authenticated user be included in calculations? 
    
    This causes an additional request to the user's profile page and is useful if 
    the purpose of these notifications are to broadcast user activity to a group.
    -->
    <add key="MonitorCurrentUser" value="true" />
    
    <!-- List of ussername to watch for activity, comma seperated. Blank says monitor all friends, and current user. -->
    <add key="MonitoredUserNames" value="" /> 
</appSettings>
```

* Compile, and run BattlefieldBot.exe

## Commands

The bot understands two commands: "subscribe", and "unsubscribe". These are only available as a direct message to the bot.

Any subscriber, or group the bot belongs to will get broadcast messages when new uploads are detected on the youtube channel.
