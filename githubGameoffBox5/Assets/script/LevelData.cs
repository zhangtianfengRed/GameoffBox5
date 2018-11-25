using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.script
{
    public abstract class LevelData
    {
        public GameObject Box { get; set; }
        public GameObject Target { get; set; }
        public GameObject play { get; set; }

        public LevelData()
        {
            Box = null;
            Target = null;
            play = null;
        }
    }


}
