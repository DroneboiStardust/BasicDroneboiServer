using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Error;
using UnityEngine;
using PacketClass;

using TcpChatServer;

class Program
{
    public static void Main()
    {
        Packet packet = new Packet(1000);

        packet.Write(9923);
        packet.Write(int.MaxValue);
        packet.Write("ur mom is sussy");
        packet.Write(12833);
        packet.Write(new Vector3(999f,21931f,391f));

        packet.SetId(6969);

        packet.InitExternal();

        Console.WriteLine(packet.Read<int>(true)[1]);
        Console.WriteLine(packet.Read<int>(true)[1]);
        Console.WriteLine(packet.Read<string>(true)[1]);
        Console.WriteLine(packet.Read<int>(true)[1]);

        Vector3 vector3 = packet.Read<Vector3>(true)[1];
        Console.WriteLine(vector3.x+","+vector3.y+","+vector3.z);

        Console.WriteLine("id:"+packet.GetId());

            
            
            // TCP server port
            int port = 3258;

            Console.WriteLine($"TCP server port: {port}");

            Console.WriteLine();

            // Create a new TCP chat server
            var server = new ChatServer(IPAddress.Any, port);

            // Start the server
            Console.Write("Server starting...");
            server.Start();
            Console.WriteLine("Done!");

            Console.WriteLine("Press Enter to stop the server or '!' to restart the server...");

            // Perform text input
            for (;;)
            {
                string line = Console.ReadLine();
                if (string.IsNullOrEmpty(line))
                    break;

                // Restart the server
                if (line == "!")
                {
                    Console.Write("Server restarting...");
                    server.Restart();
                    Console.WriteLine("Done!");
                    continue;
                }

                // Multicast admin message to all sessions
                line = "(admin) " + line;
                server.Multicast(line);
            }

            // Stop the server
            Console.Write("Server stopping...");
            server.Stop();
            Console.WriteLine("Done!");
    }
}