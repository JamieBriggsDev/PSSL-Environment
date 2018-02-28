using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSSL_Environment.ObjTypes
{
    public class Normal : Type
    {
        public const int MinimumDataLength = 4;
        public const string Prefix = "vn";

        public double NX { get; set; }

        public double NY { get; set; }

        public double NZ { get; set; }

        public int Index { get; set; }

        public void LoadFromStringArray(string[] data)
        {
            if (data.Length < MinimumDataLength)
                throw new ArgumentException("Input array must be of minimum length " + MinimumDataLength, "data");

            if (!data[0].ToLower().Equals(Prefix))
                throw new ArgumentException("Data prefix must be '" + Prefix + "'", "data");

            bool success;

            double nx, ny, nz;

            success = double.TryParse(data[1], NumberStyles.Any, CultureInfo.InvariantCulture, out nx);
            if (!success) throw new ArgumentException("Could not parse X parameter as double");

            success = double.TryParse(data[2], NumberStyles.Any, CultureInfo.InvariantCulture, out ny);
            if (!success) throw new ArgumentException("Could not parse Y parameter as double");

            success = double.TryParse(data[3], NumberStyles.Any, CultureInfo.InvariantCulture, out nz);
            if (!success) throw new ArgumentException("Could not parse Z parameter as double");

            NX = nx;
            NY = ny;
            NZ = nz;
        }

        public override string ToString()
        {
            return string.Format("v {0} {1} {2}", NX, NY, NZ);
        }

    }
}
