using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace SalesCommission.Models
{

    public class DataPoint
    {
        public DataPoint(string label, double y)
        {
            this.Label = label;
            this.Y = y;
        }

        //Explicitly setting the name to be used while serializing to JSON.
        public string Label = "";

        //Explicitly setting the name to be used while serializing to JSON.
        public Nullable<double> Y = null;
    }
}