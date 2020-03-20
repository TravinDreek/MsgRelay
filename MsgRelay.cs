//MCCScript 1.0
//using System.Net.Sockets;
MCC.LoadBot(new MsgRelay());
//MCCScript Extensions

public class MsgRelay : ChatBot{
	private Socket socket;
	private static byte[] result = new byte[4096];
	private string group = "XXXXXXXXX"; // 用于接收和发送消息的群号
	private string filterRegex = @"^<(?<name>[A-Za-z0-9_]{1,16})> (?<chatmsg>.+)"; // 使用正则表达式来匹配聊天消息


	public override void Initialize() {
        Thread thread = new Thread(new ThreadStart(Listener));
		thread.Start();
	}

	public override void GetText(string msg) {
		if(msg.Contains("unload")) {
			LogToConsole("Goodbye.");
			UnloadBot();
		}
		else {
			msg = GetVerbatim(msg);
			if(SocketConnected(socket)){
				Match match = Regex.Match(msg, filterRegex);
				if(match.Success) {
					string name = match.Groups["name"].Value;
					if(name != GetUsername()){
						string chatmsg = match.Groups["chatmsg"].Value;
						socket.Send(Encoding.UTF8.GetBytes("<success>true</success>" + "<group>" + group + "</group>" + "<msg>" + name + ": " + chatmsg + "</msg>"));
					}
				}
			}
		}
	}

	private void Listener() {
		while (true) {
			try {
				LogToConsole("正在连接到 CQ 服务器...");
				socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
				socket.Connect("127.0.0.1", 23333);
				LogToConsole("连接 CQ 服务器成功, 开始监听消息...");

				while (true) {
					try {
						int receiveLength = socket.Receive(result);
						string text = Encoding.UTF8.GetString(result, 0, receiveLength);
						if(text.IndexOf("<success>true</success>") != -1) {
							if(GetSubStr(text, "<group>", "</group>") == group) {
								SendText(GetSubStr(text, "<msg>", "</msg>"));
							}
						};
					}
					catch {
						socket.Close();
						LogToConsole("与 CQ 服务器的连接断开, 将在 10 秒后尝试重新连接...");
						Thread.Sleep(10000);
						break;
					}
				}
			}
			catch {
				LogToConsole("连接 CQ 服务器失败, 将在 10 秒后尝试重新连接...");
				Thread.Sleep(10000);
			}
		}
	}

	private bool SocketConnected(Socket s) {
		bool part1 = s.Poll(1000, SelectMode.SelectRead);
		bool part2 = (s.Available == 0);
		return !(part1 && part2);
	}

	private string GetSubStr(string src, string home_str, string end_str) {
		int home_pos = src.IndexOf(home_str);
		if(home_pos != -1) {
			home_pos = home_pos + home_str.Length;
			int end_pos = src.IndexOf(end_str, home_pos);
			if(end_pos != -1){
				return src.Substring(home_pos, end_pos - home_pos);
			}
		}
		return "";
	}
}