using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAccess.Models
{
    public sealed class OnDisconnectEventArgs : EventArgs
    {
        /// <summary>
        /// The time when the disconnection occurred.
        /// </summary>
        public DateTime When { get; set; }
    }
}
