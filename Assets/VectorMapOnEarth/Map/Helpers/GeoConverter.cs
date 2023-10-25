using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace Helpers
{
    public static class GeoConvert
    {
        private const double MeterScale = 1000;//1;
        private const double InvMeterScale = 0.001;//1;

        public static float TileSize = 500;
        private const int EarthRadius = 6378137;
        private const int EarthRadiusPolar = 6356863;
        public static double InitialResolution = 2 * Math.PI * EarthRadius / TileSize;
        private const double OriginShift = 2 * Math.PI * EarthRadius / 2;

        public static Vector3d LatLonToMetersForEarth(double lat, double lon)
        {
            var p = new Vector3d();

            p.x = EarthRadius * Mathf.Sin((float)lon * Mathf.Deg2Rad) * Mathf.Cos((float)lat * Mathf.Deg2Rad);
            p.y = EarthRadius * Mathf.Cos((float)lon * Mathf.Deg2Rad) * Mathf.Cos((float)lat * Mathf.Deg2Rad);
            p.z = EarthRadius * Mathf.Sin((float)lat * Mathf.Deg2Rad); // * Mathf.Sin((float)lon * Mathf.Deg2Rad);

            return new Vector3d(p.y, p.z, p.x) * InvMeterScale / 10;
        }
    }
}
