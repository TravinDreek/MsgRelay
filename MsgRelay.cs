//MCCScript 1.0
//using System.Net.WebSockets;
MCC.LoadBot(new MsgRelay());
//MCCScript Extensions

public class MsgRelay : ChatBot {
	private string sessionKey;
	private string host = "127.0.0.1:8080"; // HTTP API 地址
	private string authKey = "1234567890"; //  HTTP API AuthKey
	private string botQQ = "1234567890"; // 机器人 QQ 帐号
	private string group = "1234567890"; // 用于接收和发送消息的 QQ 群号
	private string filterRegex = @"^.+$"; // 用于匹配 MC 聊天消息的正则表达式, 符合条件的消息将被转发至 QQ 群
	private string msgFormat = "{1}: {2}"; // MC 机器人发送消息的格式, {0} = QQ号, {1} = 群名片, {2} = 消息内容

	public override void Initialize() {
		while(true) {
			LogToConsole("正在从 HTTP API 获取 Session...");
			string authResult = Post("http://" + host + "/auth", "{\"authKey\":\"" + authKey + "\"}");

			if(GetSubStr(authResult, "\"code\":", ",") == "0") {
				sessionKey = GetSubStr(authResult, "\"session\":\"", "\"}");
				LogToConsole("获取 Session 成功. 正在校验并激活 Session...");
				string verifyResult = Post("http://" + host + "/verify", "{\"sessionKey\":\"" + sessionKey + "\", \"qq\":" + botQQ + "}");

				if(GetSubStr(verifyResult, "\"code\":", ",") == "0") {
					LogToConsole("成功激活 Session.");
					StartQQMsgListener("ws://" + host + "/message?sessionKey=" + sessionKey);
					break;
				}
				else {
					LogToConsole("激活 Session 失败(" + GetSubStr(verifyResult, "\"code\":", ",") + "). 将在 10 秒后获取新的 Session...");
					Thread.Sleep(10000);
				}
			}
			else {
				LogToConsole("获取 Session 失败 (" + GetSubStr(authResult, "\"code\":", ",") + "). 将在 10 秒后尝试重新获取...");
				Thread.Sleep(10000);
			}
		}
	}

	/*
		MCC 的聊天消息事件
	*/
	public override void GetText(string msg) {
		msg = GetVerbatim(msg);
		Match match = Regex.Match(msg, filterRegex);
		if(match.Success) {
			Post("http://" + host + "/sendGroupMessage",
			"{\"sessionKey\":\"" + sessionKey + "\",\"target\":" + group + ",\"messageChain\":[{\"type\":\"Plain\", \"text\":\"" + msg + "\"}]}");
		}
	}

	/*
		开始监听 QQ 消息
	*/
	private async void StartQQMsgListener(string uri) {
		while (true) {
			try {
				LogToConsole("正在连接到 Websocket...");
				ClientWebSocket socket = new ClientWebSocket();
				socket.ConnectAsync(new Uri(uri), CancellationToken.None).Wait();
				LogToConsole("连接 Websocket 成功. 开始监听消息...");
				while (true) {
					try {
						byte[] array = new byte[4096];
						WebSocketReceiveResult result = await socket.ReceiveAsync(new ArraySegment<byte>(array), CancellationToken.None);
						if (result.MessageType == WebSocketMessageType.Text) {
							string received = Encoding.UTF8.GetString(array, 0, result.Count);
							if(GetSubStr(received, "\"type\":\"", "\",") == "GroupMessage" && GetSubStr(received, "\"group\":{\"id\":", ",") == group) {
								SendText(string.Format(
									msgFormat,
									GetSubStr(received, "\"sender\":{\"id\":", ","),
									GetSubStr(received, "\"memberName\":\"", "\","),
									GetSubStr(received, "\"text\":\"", "\"}]")
								));
							}
						}
					}
					catch {
						socket.CloseAsync(WebSocketCloseStatus.Empty, string.Empty, CancellationToken.None);
						LogToConsole("与 Websocket 的连接断开. 将在 5 秒后尝试重新连接...");
						Thread.Sleep(5000);
						break;
					}
				}
			}
			catch {
				LogToConsole("连接 Websocket 失败. 将在 10 秒后尝试重新连接...");
				Thread.Sleep(10000);
			}
		}
	}

	/*
		POST 请求发送
	*/
	public static string Post(string url, string content) {
		string result = "";
		HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
		req.Method = "POST";
		req.ContentType = "application/json";
		byte[] data = Encoding.UTF8.GetBytes(content);
		req.ContentLength = data.Length;
		using (Stream reqStream = req.GetRequestStream()) {
			reqStream.Write(data, 0, data.Length);
			reqStream.Close();
		}
		HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
		Stream stream = resp.GetResponseStream();
		using (StreamReader reader = new StreamReader(stream, Encoding.UTF8)) {
			result = reader.ReadToEnd();
		}
		return result;
	}

	/*
		硬核解析 JSON
	*/
	private string GetSubStr(string src, string home_str, string end_str) {
		int home_pos = src.IndexOf(home_str);
		if(home_pos != -1) {
			home_pos = home_pos + home_str.Length;
			int end_pos = src.IndexOf(end_str, home_pos);
			if(end_pos != -1) {
				return src.Substring(home_pos, end_pos - home_pos);
			}
		}
		return "";
	}
}