# MsgRelay
MsgRelay 是一个运行于 MCC 和 CQ 的消息转发器, 允许 QQ 机器人将来自 Minecraft 服务器的消息转发至指定群, 以及将来自群内的消息转发至 Minecraft 服务器. 借助 MsgRelay, 你可以直接通过 QQ 与你最喜欢的服务器中的玩家聊天.

![图片](https://i.loli.net/2020/03/20/9JD5381ZSutHofb.png)

## 安装方式
1. 下载 [MCC](https://github.com/ORelio/Minecraft-Console-Client), [CQ](https://cqp.cc/t/23253), 以及本仓库中的 `.cpk` 文件和 `.cs` 文件.
2. 将 `peaksol.msgrelay.cq.cpk` 文件放入 CQ 目录下的 `app` 文件夹
3. 将 `MsgRelay.cs` 直接放在 MCC 所在的目录下, 并[配置](##配置)好此文件.
4. 打开CQ, 登录一个 QQ 账号, 从菜单中进入 `应用 - 应用管理`, 选中 `MsgRelay` 并点击 `启用`.
5. 打开 MCC, 登录一个 Minecraft 账号(如果要使用盗版, 请将密码填写为 `-`), 并输入 `/script MsgRelay.cs`


## 配置
`MsgRelay.cs` 中有两项配置, 你需要正确配置好这两项才能使 MsgRelay 正常运行. 请在代码中找到以下部分并将其配置:
```cs
private string group = "XXXXXXXXX"; // 此处填写用于接收和发送消息的群号
private string filterRegex = @"^<(?<name>[A-Za-z0-9_]{1,16})> (?<chatmsg>.+)"; // 此处填写用于匹配聊天消息的正则表达式
```
请注意, 你所写的正则表达式中必须要包含 `name` 和 `chatmsg` 分组(如上所示), 这样我们才能正确地将消息. 该示例中的正则表达式用于匹配形如 `<玩家名> 消息内容` 的消息格式.

## 原理
MCC 是一个第三方 Minecraft 客户端, 此客户端允许通过脚本来自动化一系列操作. CQ 是一个机器人核心, 允许通过编写插件来自制拥有特定功能的机器人. MsgRelay 通过在这两个平台间建立通讯, 做到了在 Minecraft 和 QQ 中跨平台聊天.

## 问题追踪
如果你遇到了任何问题, 请将你的问题发在 Issues.