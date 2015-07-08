## Neebs Gaming Video Notifier for Telegram

C# library using Telegrams Bot API (https://core.telegram.org/bots/api)
Monitors Neebs Gaming (https://www.youtube.com/user/NeebsGaming) for new videos, and notifies subscribers about new uploads

## Directions

* Create a bot with Telegram using BotFather: https://core.telegram.org/bots#botfather
* Register for a google api token @ https://console.developers.google.com

Edit HiddenSetting.config, and add your telegram bot API token, and google developer api information.
```XML
<?xml version="1.0" encoding="utf-8"?>
<appSettings>
    <!-- Create using BotFather: https://core.telegram.org/bots#botfather -->
    <add key="TelegramApiToken" value="*** Your Bot Api Key ***" />
    
    <!-- Create your own token @ https://console.developers.google.com -->
    <add key="YoutubeApiAppName" value="*** Your Youtube App Name ***" />
    <add key="YoutubeApiToken" value="*** Your Youtube Api Key ***" />
    
    <!-- Pointed at NeebsGaming channel, but really, it could point anywhere. -->
    <add key="YoutubeChannelId" value="UCiufyZv8iRPTafTw0D4CvnQ" /> 
</appSettings>
```

* Compile, and run NeebsBot.exe

## Commands

The bot understands two commands: "subscribe", and "unsubscribe". These are only available as a direct message to the bot.

Any subscriber, or group the bot belongs to will get broadcast messages when new uploads are detected on the youtube channel.
