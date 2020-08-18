# MsgRelay
*提示: 适用于酷Q平台的 [MsgRelay v1](https://github.com/TravinDreek/MsgRelay/tree/master) 已不再维护. 此分支是 MsgRelay v2, 适用于 [mirai](https://github.com/mamoe/mirai) 平台.*

MsgRelay 是一个运行于 MCC 和 QQ 的消息转发器, 允许 QQ 机器人将来自 Minecraft 服务器的消息转发至指定群, 以及将来自群内的消息转发至 Minecraft 服务器. 借助 MsgRelay, 你可以直接通过 QQ 与你最喜欢的服务器中的玩家聊天.

![图片](https://i.loli.net/2020/03/20/9JD5381ZSutHofb.png)

## 需求
- [MCC (Minecraft Console Client)](https://github.com/ORelio/Minecraft-Console-Client)
- 安装了 [mirai-api-http](https://github.com/project-mirai/mirai-api-http) 插件的 [mirai-console](https://github.com/mamoe/mirai-console)

## 开始使用
1. 将 MCC 放在任意目录下, 然后将将本仓库的 `MsgRelay.cs` 放在其同级目录下.
2. 进行[配置](#配置).
3. 启动 mirai-console, 登录一个 QQ 帐号.
4. 启动 MCC, 登录一个 Minecraft 帐号 (如果要使用离线版, 请将密码填写为 `-`), 并输入 `/script MsgRelay.cs`

## 配置
请先在 mirai-api-http 插件的 settings.yml 中启用 websocket 服务, 以允许 MC 机器人监听 QQ 消息事件:
```yml
enableWebsocket: true
```
然后, 请在 `MsgRelay.cs` 中找到以下部分并将其配置:
```cs
private string host = "127.0.0.1:8080"; // HTTP API 地址
private string authKey = "1234567890"; //  HTTP API AuthKey
private string botQQ = "1234567890"; // 机器人 QQ 帐号
private string group = "1234567890"; // 用于接收和发送消息的 QQ 群号
private string filterRegex = @"^.+$"; // 用于匹配 MC 聊天消息的正则表达式, 符合条件的消息将被转发至 QQ 群
private string msgFormat = "{1}: {2}"; // MC 机器人发送消息的格式, {0} = QQ号, {1} = 群名片, {2} = 消息内容
```


## 问题追踪
如果你遇到了任何问题, 请将你的问题发在 Issues.