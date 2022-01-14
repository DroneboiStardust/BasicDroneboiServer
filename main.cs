using System;
using Error;
using UnityEngine;
using PacketClass;

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
    }
}