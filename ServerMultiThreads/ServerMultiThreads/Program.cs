using System.Net;
using System.Net.Sockets;
using System.Text;

using Socket tcpListener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

tcpListener.Bind(new IPEndPoint(IPAddress.Any, 8888));
tcpListener.Listen(); 
Console.WriteLine("Ожидаю подключение... ");

while (true)
{
    var tcpClient = await tcpListener.AcceptAsync();
    Task.Run(async () => await ProcessClientAsync(tcpClient));
}


async Task ProcessClientAsync(Socket tcpClient)
{
    var response = new List<byte>();
    var bytesRead = new byte[1];
    while (true)
    {
        while (true)
        {
            var count = await tcpClient.ReceiveAsync(bytesRead);
            if (count == 0 || bytesRead[0] == '\n') break;
            response.Add(bytesRead[0]);
        }
        var data = Encoding.UTF8.GetString(response.ToArray());
        if (data == "END") break;

        Console.WriteLine($"Полученный текст: {data}");
        response.Clear();

        string reply = "Спасибо за запрос в " + data.Length.ToString()
                + " символов";
    }
    tcpClient.Shutdown(SocketShutdown.Both);
    tcpClient.Close();
}