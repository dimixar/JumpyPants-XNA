using System;
using Microsoft.Xna.Framework;

namespace JumpyPants
{
    class PlayerIndexEventArgs : EventArgs
    {
        private PlayerIndex playerIndex;

        public PlayerIndexEventArgs(PlayerIndex playerIndex)
        {
            this.playerIndex = playerIndex;
        }



        public PlayerIndex PlayerIndex {get{return playerIndex;}}
    }
}
