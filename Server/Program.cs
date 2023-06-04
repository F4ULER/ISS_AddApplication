using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            // ip и port поключение соединения сервера и клиенту
            // реализуется на 1 машине (localhost), поэтому берем стандартные для этого ip и port
            const string ip = "127.0.0.1";
            const int port = 8080;

            //конечная точка (куда можно подключаться) или точка подключения
            //Parse помогает преобразовать строку в текстовом формате в стандартный формат подключения
            var tcpEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);

            //Объявление сокета (дверь, через которую устанавливается соединение)
            //набор параметров для tcp протокола
            var tcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            //Связываение. Перевод сокета в режим ожидания. Что нужно слушать сокету конкретный порт
            tcpSocket.Bind(tcpEndPoint);

            //запуск сокета на прослушивание (количество клиентов, которые могут подключиться на один порт. По очереди)
            tcpSocket.Listen(5);
        
            //бесконечный процесс прослушивания
            while (true)
            {
                // обработчик на прием сообщения. создается новый сокет для подключения клиента
                //создается сокет для каждого отдельного клиента, а после выполнения - удаляется
                var listener = tcpSocket.Accept();

                // куда будем принимать сообщение
                var buffer = new byte[256];

                //количество реально полученных байт
                var size = 0;


                var data = new StringBuilder(); //собирает полученные данные

                do
                {
                    // получение байт
                    size = listener.Receive(buffer);

                    //из большого сообщения потихоньку собираем сообщение, чтобы избежать потерю данных
                    data.Append(Encoding.UTF8.GetString(buffer, 0, size));
                }
                while (listener.Available > 0); //будет выполняться считывание пока в подключении есть данные

                Console.WriteLine(data);

                //обратный ответ. кодируем строку и отправляем
                listener.Send(Encoding.UTF8.GetBytes("Успех!"));

                //выключаем соединение (listener) и закрываем
                listener.Shutdown(SocketShutdown.Both);
                listener.Close();
            }
        }
    }
}
