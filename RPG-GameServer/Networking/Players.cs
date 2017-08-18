using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Networking {

    /// <summary>
    /// An indexed collection class to keep track of all the <see cref="Players"/>.
    /// </summary>
    [Serializable]
    public class Players : List<Player> {

        /// <summary>
        /// A boolean that controls whether new <see cref="Player"/>-instances are to be added to the <see cref="Players"/>-list or not.
        /// </summary>
        public bool AddPlayersAutomatically = true;

        /// <summary>
        /// Gets or sets a <see cref="Player"/> by finding their username.
        /// </summary>
        /// <param name="username">The username to search for</param>
        /// <returns>The <see cref="Player"/> if found, null if not</returns>
        public Player this[ string username ] => this.FirstOrDefault( player => player.Username.ToLower() == username.ToLower() );

        /// <summary>
        /// Gets or sets a <see cref="Player"/> by finding their username.
        /// </summary>
        /// <param name="socket">The <see cref="TcpSocket"/> to search for</param>
        /// <returns>The <see cref="Player"/> if found, null if not</returns>
        public Player this[ TcpSocket socket ] => this.FirstOrDefault( player => player.Socket == socket );

        /// <summary>
        /// Creates a new instance of the <see cref="Players"/> class
        /// </summary>
        public Players() { }

        private Players( IEnumerable<Player> userList ) : base( userList ) { }

        /// <summary>
        /// Checks if the given <see cref="Player"/> exists.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> to check</param>
        /// <returns>True if the <see cref="Player"/> was found, false if not</returns>
        public bool Exists( Player player ) => Contains( player );

        /// <summary>
        /// Checks if the given <see cref="Player"/> exists.
        /// </summary>
        /// <param name="username">The <see cref="Player"/>'s username to check</param>
        /// <returns>True if the <see cref="Player"/> was found, false if not</returns>
        public bool Exists( string username ) => this[ username ] != null;

        /// <summary>
        /// Checks if the given <see cref="Player"/> exists.
        /// </summary>
        /// <param name="socket">The <see cref="Player"/>'s <see cref="TcpSocket"/> to check</param>
        /// <returns>True if the <see cref="Player"/> was found, false if not</returns>
        public bool Exists( TcpSocket socket ) => this[ socket ] != null;

        /// <summary>
        /// Creates and adds a new <see cref="Player"/> to the <see cref="Players"/>-list.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> to add</param>
        /// <returns>True if the player was successfully added, false if not</returns>
        public new bool Add( Player player ) => Add( player, false );

        /// <summary>
        /// Adds a range of <see cref="Players"/> to the <see cref="Players"/>-list.
        /// </summary>
        /// <param name="userEnumerable">The <see cref="IEnumerable{Player}"/>-range to add</param>
        /// <returns>True if all users were successfully added, false if not</returns>
        public new bool AddRange( IEnumerable<Player> userEnumerable ) => !( userEnumerable.Where( player => !Add( player, false ) ).ToArray().Length > 0 );

        /// <summary>
        /// Creates and adds a new <see cref="Player"/> to the <see cref="Players"/>-list.
        /// </summary>
        /// <param name="username">The username of the <see cref="Player"/> to add</param>
        /// <returns>True if the player was successfully added, false if not</returns>
        public bool Add( string username ) => Add( new Player( username ), false );

        /// <summary>
        /// Adds a <see cref="Player"/> to the <see cref="Players"/>-list.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> to add</param>
        /// <param name="overwrite">Whether to overwrite if the player already exists or not (default: false)</param>
        /// <returns>True if the player was successfully added, false if not</returns>
        public bool Add( Player player, bool overwrite ) {
            if ( Exists( player ) ) {
                if ( !overwrite )
                    return false;
                Remove( player );
            }
            base.Add( player );

            return Exists( player );
        }

        /// <summary>
        /// Removes a <see cref="Player"/> from the <see cref="Players"/>-list.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> to remove from the list</param>
        /// <returns>True if the <see cref="Player"/> does not exist in the <see cref="Players"/>-list (anymore), false if the <see cref="Player"/> still exists</returns>
        public new bool Remove( Player player ) {
            if ( !Exists( player ) )
                return true;

            base.Remove( player );
            return !Exists( player );
        }

        /// <summary>
        /// Removes a <see cref="Player"/> from the <see cref="Players"/>-list.
        /// </summary>
        /// <param name="username">The username of the <see cref="Player"/> to remove from the list</param>
        /// <returns>True if the <see cref="Player"/> does not exist in the <see cref="Players"/>-list (anymore), false if the <see cref="Player"/> still exists</returns>
        public bool Remove( string username ) => Remove( this[ username ] );

        /// <summary>
        /// Removes a <see cref="Player"/> from the <see cref="Players"/>-list.
        /// </summary>
        /// <param name="socket">The socket of the <see cref="Player"/> to remove from the list</param>
        /// <returns>True if the <see cref="Player"/> does not exist in the <see cref="Players"/>-list (anymore), false if the <see cref="Player"/> still exists</returns>
        public bool Remove( TcpSocket socket ) => Remove( this[ socket ] );

        /// <summary>
        /// Clears the <see cref="Players"/>-list of all <see cref="Players"/> and <see cref="TcpSocket"/>s./>
        /// </summary>
        /// <returns>True if successfully cleared, false if not</returns>
        public new bool Clear() {
            base.Clear();
            return Count == 0;
        }

        public void ClearDisconnectedPlayers() {
            // Checks which users are either not connected or have null as a socket value and removes those
            foreach ( Player player in this.Where( player => player.Socket == null || !player.Socket.Connected ).ToArray() ) {
                Remove( player );
            }
        }
    }

    /// <summary>
    /// A data container class used to save the player's info like their username and socket information.
    /// </summary>
    [Serializable]
    public class Player : EntityTransform {
        public string Username;
        [NonSerialized] public TcpSocket Socket;

        private void Init( string username, bool active = false, Vector3 position = new Vector3(), Vector3 rotation = new Vector3(), Vector3 scale = new Vector3() ) {
            Username = username;
            Active = active;
            Position = position;
            Rotation = rotation;
            Scale = scale;

            if ( !ServerData.Players.AddPlayersAutomatically )
                ServerData.Players.Add( this );
        }

        public Player() { }

        public Player( string username, bool active, Vector3 position, Vector3 rotation, Vector3 scale ) => Init( username, active, position, rotation, scale );
        public Player( string username, bool active, Vector3 position, Vector3 rotation ) => Init( username, active, position, rotation );
        public Player( string username, bool active, Vector3 position ) => Init( username, active, position );
        public Player( string username, bool active ) => Init( username, active );
        public Player( string username ) => Init( username );

        /// <summary>
        /// Creates a new player
        /// </summary>
        /// <param name="username"></param>
        /// <param name="socket"></param>
        public Player( string username, TcpSocket socket ) {
            Username = username;
            Socket = socket;

            if ( !ServerData.Players.AddPlayersAutomatically )
                ServerData.Players.Add( this );
        }
    }

}