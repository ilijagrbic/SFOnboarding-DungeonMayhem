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
    public class PlayCardResult
    {
        [DataMember]
        public required GameState GameState{ get; set; }
        [DataMember]
        public bool playAnother {  get; set; }
    }
}
