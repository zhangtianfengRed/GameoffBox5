using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.script
{
    public class MapData
    {
        public int width { get; set; }
        public int high { get; set; }
        public GameObject Ground;
        public GameObject Groundgroup;

        public MapData(GameObject gameObject)
        {
            Ground = gameObject;
            Groundgroup = new GameObject("GroundGroup");
        }

        public void Startmap()
        {
            for(int x = 0; x < width; x++)
            {
                for(int y = 0; y < high; y++)
                {
                    GameObject T= UnityEngine.Object.Instantiate(Ground, new Vector3(x, y, 0), Quaternion.identity);
                    T.transform.parent = Groundgroup.transform;
                }
            }
        }
    }
}
