using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Networking {

    [Serializable]
    public struct Login {
        public string Username;
        public string SessionPassword;
    }

    public enum CommandType {
        UsernameTaken,

    }

    [Serializable]
    public struct Command {
        public CommandType Type;
        public User User;
    }

    [ Serializable ]
    public struct PlayerEvent {
        public User User;
        public string Status;
    }

}
