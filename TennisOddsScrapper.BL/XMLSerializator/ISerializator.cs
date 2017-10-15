using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TennisOddsScrapper.BL.Models;

namespace TennisOddsScrapper.BL.XMLSerializator
{
    public interface ISerializator
    {
        string Path { get; set; }

        OddSerializationList TransformData(List<OddValue> oddsValues);
        void Serialize(OddSerializationList oddSerialization);
    }
}
