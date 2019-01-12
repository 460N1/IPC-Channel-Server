using System;
using System.Runtime.Remoting.Channels.Ipc;
using System.Security.Permissions;

public class Server
{
    [SecurityPermission(SecurityAction.Demand)]
    public static void Main(string[] args)
    {
        // Krijo server channel
        IpcChannel serverChannel =
            new IpcChannel("localhost:9090");

        // Regjistro channelin e serverit dhe percakto sigurine
        System.Runtime.Remoting.Channels.ChannelServices.RegisterChannel(serverChannel, false);

        // Trego emrin e channel
        Console.WriteLine("Emri i channel eshte {0}.",
            serverChannel.ChannelName);

        // Trego prioritetin e channel
        Console.WriteLine("Prioriteti i channel eshte {0}.",
            serverChannel.ChannelPriority);

        // Trego URIte e asociuara me kete channel
        System.Runtime.Remoting.Channels.ChannelDataStore channelData =
            (System.Runtime.Remoting.Channels.ChannelDataStore)
            serverChannel.ChannelData;
        foreach (string uri in channelData.ChannelUris){
            Console.WriteLine("URI e channel eshte {0}.", uri);
        }

        // Lejo qasjen ne nje object per remote call
        System.Runtime.Remoting.RemotingConfiguration.
            RegisterWellKnownServiceType(
                typeof(RemoteObject), "RemoteObject.rem",
                System.Runtime.Remoting.WellKnownObjectMode.Singleton);

        // Parso URLne e channel
        string[] urls = serverChannel.GetUrlsForUri("RemoteObject.rem");
        if (urls.Length > 0){
            string objectUrl = urls[0], objectUri,
                channelUri = serverChannel.Parse(objectUrl, out objectUri);
            Console.WriteLine("URI e objektit eshte {0}.", objectUri);
            Console.WriteLine("URI e channel eshte {0}.", channelUri);
            Console.WriteLine("URL e objektit eshte {0}.", objectUrl);
        }
        Console.WriteLine("Per t'e ndalur serverin, shtyp 0 dhe ENTER.");
        // Prit per ndalim
        // Serveri ndalet momentin kur perdoruesi shtyp ndonje send
        String uInput="";
        while (!uInput.Equals("exit")) {
            try{uInput = Console.ReadLine();}
            catch{uInput = "";}
        }
        if (uInput.Equals("exit")){
            serverChannel.StopListening(null);
            Console.WriteLine("Serveri eshte ndalur.");
            Console.ReadLine();
        }
    }
}
public class RemoteObject : MarshalByRefObject
{
    private int callCount = 0;

    public int GetCount()
    {
        callCount++;
        Console.WriteLine("Metoda GetCount eshte thirrur " + callCount + " here.");
        return (callCount);
    }
}