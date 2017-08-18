using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Networking;
using System.Threading;

namespace RPG_GameServer {

    public static class RPGConsole {

        public static void WriteLine( string str ) { Write( str + "\n" ); }
        public static void WriteLine( string str, ConsoleColor color ) { Write( str + "\n", color ); }

        public static void Write( string str ) { Console.Write( str ); }

        public static void Write( string str, ConsoleColor color ) {
            ConsoleColor oldColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.Write( str );
            Console.ForegroundColor = oldColor;
        }

    }

    public partial class Program {

        public static TcpServer Server;

        private static Thread gameThread;
        private static bool _gameActive;
        public static bool GameActive {
            get => _gameActive;
            set {
                _gameActive = value;

                if ( value )
                    gameThread = ThreadHandler.Create( () => {
                        while ( GameActive ) {
                            Broadcast( ServerData.Players );
                            Thread.Sleep( 10 );
                        }
                    } );
                else
                    ThreadHandler.Remove( gameThread );
            }
        }

        public static void Main( string[] args ) {
            Server = new TcpServer( 23000, InitClient );

            RPGConsole.WriteLine( $"Server started on {Server.LocalEndPoint}, waiting for incoming connections...", ConsoleColor.DarkGreen );

            bool active = true;
            while ( active ) {
                string message = Console.ReadLine();
                string[] split = message?.Split( ' ' );

                switch ( split?[ 0 ] ) {
                    default:
                        if ( message?.IndexOf( '/' ) != 0 )
                            Broadcast( message );
                        else
                            RPGConsole.WriteLine( $"{split[ 0 ]} was not recognized as an RPG-GameServer command", ConsoleColor.Red );
                        break;
                    case "/help": {
                            RPGConsole.WriteLine( "All available commands:\n", ConsoleColor.DarkCyan );
                            RPGConsole.WriteLine( "By writing anything not listed below, you are able to broadcast a packet to all connected clients.\nE.G. \"Hello World\" will be broadcasted but \"/help\" will not." );
                            RPGConsole.WriteLine( "/help { Shows all available commands in a list format }" );
                            RPGConsole.WriteLine( "/newclient, /newc, /nclient, /nc { Opens a new socket window, if available. (DEBUGGING ONLY) }" );
                            RPGConsole.WriteLine( "/show { Shows the server's bound ip, port or both. }" );
                            RPGConsole.WriteLine( "/quit, /exit, /stop, /close { Stops the server and closes the application. }" );
                        }
                        break;
                    case "/show":
                        if ( split.Length < 2 ) {
                            RPGConsole.WriteLine( "\"/show\" requires (at least) one parameter.", ConsoleColor.Red );
                            break;
                        }

                        switch ( split[ 1 ].ToLower() ) {
                            default:
                                RPGConsole.WriteLine( "\"" + split[ 1 ] + "\" was not recognized as a ScreenBuddiesServer command.", ConsoleColor.Red );
                                break;
                            case "help":
                                Console.ForegroundColor = ConsoleColor.White;
                                RPGConsole.WriteLine( "Available \"/show [cmd]\" commands:\n" );
                                Console.ForegroundColor = ConsoleColor.Gray;
                                RPGConsole.WriteLine( "info" );
                                RPGConsole.WriteLine( "port" );
                                RPGConsole.WriteLine( "ip" );
                                break;
                            case "info":
                                RPGConsole.WriteLine( $"Bound on: {Server.LocalEndPoint}", ConsoleColor.Yellow );
                                break;
                            case "port":
                                RPGConsole.WriteLine( $"Bound on port: {Server.LocalEndPoint.ToString().Split( ':' )[ 1 ]}", ConsoleColor.Yellow );
                                break;
                            case "ip":
                                RPGConsole.WriteLine( $"Bound on ip: {Server.LocalEndPoint.ToString().Split( ':' )[ 0 ]}", ConsoleColor.Yellow );
                                break;
                        }
                        break;
                    case "/users":
                    case "/online":
                    case "/list": {
                            RPGConsole.WriteLine( "Currently online users:\n", ConsoleColor.DarkCyan );
                            foreach ( Player player in ServerData.Players )
                                RPGConsole.WriteLine( $"{player.Username} ({player.Socket.LocalEndPoint})" );
                        }
                        break;
                    case "/kick": {
                            if ( split.Length <= 1 ) {
                                RPGConsole.WriteLine( "No parameters given, please specify the player's username like so (without brackets):\r\n/kick [username]", ConsoleColor.Red );
                                break;
                            }
                            if ( !ServerData.Players.Exists( split[ 1 ] ) ) {
                                RPGConsole.WriteLine( $"Could not find player \"{split[ 1 ]}\"", ConsoleColor.Red );
                                break;
                            }

                            Player player = ServerData.Players[ split[ 1 ] ];
                            Broadcast( $"{player.Username} was kicked from the server." );
                            ClientDisconnected( player.Socket, null );
                        }
                        break;
                    case "/game": {
                            if ( split.Length <= 1 ) {
                                RPGConsole.WriteLine( $"No parameters given. Usage:\r\n{split[ 0 ]} [active, deactive]", ConsoleColor.Red );
                                break;
                            }
                            if ( split[ 1 ] == "active" )
                                GameActive = true;
                            if ( split[ 1 ] == "deactive" )
                                GameActive = false;
                        }
                        break;
                    case "/quit":
                    case "/exit":
                    case "/stop":
                    case "/close":
                        active = false;
                        break;
                }
            }

            RPGConsole.WriteLine( "Server is closing down...", ConsoleColor.Red );
            RPGConsole.WriteLine( "Press any key to close this window...", ConsoleColor.DarkCyan );
            Console.ReadKey( true );
        }

        public static void Broadcast( object data ) { Broadcast( new Packet( data ) ); }

        public static void Broadcast( Packet packet ) {
            foreach ( Player player in ServerData.Players )
                player.Socket.Send( packet );
        }

    }
}