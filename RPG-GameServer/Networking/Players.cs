using System;
using System.Collections.Generic;
using System.Linq;

namespace Networking {

    /// <summary>
    /// An indexed collection class to keep track of all the <see cref="Players"/>.
    /// </summary>
    [Serializable]
    public class Players : List<Player> {

        /// <summary>
        /// This variable will keep track of the last used ID and increments it, so there will never be a duplicate ID.
        /// </summary>
        private static int _idAutoIncrement;

        /// <summary>
        /// A boolean that controls whether new <see cref="Player"/>-instances are to be added to the <see cref="Players"/>-list or not.
        /// </summary>
        public bool AddUsersAutomatically = true;

        /// <summary>
        /// Gets or sets a <see cref="Player"/> by finding their ID.
        /// </summary>
        /// <param name="userID">The ID to search for</param>
        /// <returns>The <see cref="Player"/> if found, null if not</returns>
        public new Player this[ int userID ] => this.FirstOrDefault( user => user.ID == userID );

        /// <summary>
        /// Gets or sets a <see cref="Player"/> by finding their username.
        /// </summary>
        /// <param name="username">The username to search for</param>
        /// <returns>The <see cref="Player"/> if found, null if not</returns>
        public Player this[ string username ] => this.FirstOrDefault( user => user.Username.ToLower() == username.ToLower() );

        /// <summary>
        /// Gets or sets a <see cref="Player"/> by finding their username.
        /// </summary>
        /// <param name="socket">The <see cref="TcpSocket"/> to search for</param>
        /// <returns>The <see cref="Player"/> if found, null if not</returns>
        public Player this[ TcpSocket socket ] => this.FirstOrDefault( user => user.Socket == socket );

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
        /// <param name="userID">The <see cref="Player"/>'s ID to check</param>
        /// <returns>True if the <see cref="Player"/> was found, false if not</returns>
        public bool Exists( int userID ) => this[ userID ] != null;

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
            if ( player.ID < 0 )
                player.ID = _idAutoIncrement++;

            if ( this[ player.Username ] != null ) {
                if ( !overwrite )
                    return Contains( player );
                Remove( player );
                base.Add( player );
            } else {
                base.Add( player );
            }

            return Contains( player );
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
        /// <param name="userID">The ID of the <see cref="Player"/> to remove from the list</param>
        /// <returns>True if the <see cref="Player"/> does not exist in the <see cref="Players"/>-list (anymore), false if the <see cref="Player"/> still exists</returns>
        public bool Remove( int userID ) => Remove( this[ userID ] );

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

        public void ClearDisconnectedUsers() {
            // Checks which players are either not connected or have null as a socket value and removes those
            foreach ( Player user in this.Where( user => user.Socket == null || !user.Socket.Connected ) ) {
                Remove( user );
            }
        }

        /// <summary>
        /// Replaces the <see cref="Players"/>-list with the given <see cref="List{Player}"/>
        /// </summary>
        /// <param name="players">The <see cref="Players"/>-list to replace</param>
        /// <param name="userList">The <see cref="List{Player}"/> to replace the <see cref="Players"/>-list with</param>
        /// <returns>The replaced <see cref="Players"/>-list instance</returns>
        public static Players operator -( Players players, IEnumerable<Player> userList ) {
            return new Players( userList );
        }

        /// <summary>
        /// Merges a <see cref="List{Player}"/> and a <see cref="Players"/>-list into a <see cref="Players"/>-list
        /// </summary>
        /// <param name="players">The <see cref="Players"/>-list to merge the <see cref="List{Player}"/> into</param>
        /// <param name="userList">The <see cref="List{Player}"/> to merge into the <see cref="Players"/>-list</param>
        /// <returns>The merged <see cref="Players"/>-list instance</returns>
        public static Players operator +( Players players, IEnumerable<Player> userList ) {
            foreach ( Player user in userList )
                players.Add( user );

            return players;
        }
    }

    /// <summary>
    /// A data container class used to save the player's info like their username and socket information.
    /// </summary>
    [Serializable]
    public class Player {
        public int ID = -1;
        public string Username;
        [NonSerialized] public TcpSocket Socket;

        public Player() { }

        public Player( string username ) {
            Username = username;

            if ( !Data.Players.AddUsersAutomatically )
                Data.Players.Add( this );
        }

        /// <summary>
        /// Creates a new player
        /// </summary>
        /// <param name="username"></param>
        /// <param name="socket"></param>
        public Player( string username, TcpSocket socket ) {
            Username = username;
            Socket = socket;

            if ( !Data.Players.AddUsersAutomatically )
                Data.Players.Add( this );
        }
    }

}