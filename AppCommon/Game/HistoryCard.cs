using AppCommon.GameModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace AppCommon.Game
{
    [DataContract]
    public class HistoryCard
    {
        [DataMember]
        public required Card Card { get; set; }
        [DataMember]
        public required int TargetPlayer { get; set; }
    }
}
