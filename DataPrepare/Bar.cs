using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataPrepare
{
    class Bar
    {
        private DateTime dt;
        private int onlyTime; // время бара в виде целого числа
        private double open;
        private double high;
        private double low;
        private double close;
        private int volume;
        // private int adjustKoeff;

        public DateTime Dt { get => dt; }
        public double Open { get => open; }
        public double High { get => high; }
        public double Low { get => low; }
        public double Close { get => close; }
        public int Volume { get => volume; }
        public int OnlyTime { get => onlyTime; }
        // public int AdjustKoeff { get => adjustKoeff; set => adjustKoeff = value; }

        public Bar ( string bar )
        {
            string[] words = bar.Split ( ',' );
            string pattern = "yyyyMMddHHmmss";

            DateTime.TryParseExact ( words[0] + words[1], pattern, null, System.Globalization.DateTimeStyles.None, out dt );
            onlyTime = int.Parse ( words[1] );
            open = double.Parse ( words[2].Replace ( '.', ',' ) );
            high = double.Parse ( words[3].Replace ( '.', ',' ) );
            low = double.Parse ( words[4].Replace ( '.', ',' ) );
            close = double.Parse ( words[5].Replace ( '.', ',' ) );
            volume = int.Parse ( words[6] );

        }

        public override string ToString ()
        {
            return dt.ToString ( "yyyyMMdd" ) + "," +
                dt.ToString ( "HHmmss" ) + "," +
                open.ToString ().Replace ( ',', '.' ) + "," +
                high.ToString ().Replace ( ',', '.' ) + "," +
                low.ToString ().Replace ( ',', '.' ) + "," +
                close.ToString ().Replace ( ',', '.' ) + "," +
                volume.ToString ();
        }

        public string AjustedString ( double koeff )
        {
            return dt.ToString ( "yyyyMMdd" ) + "," +
                 dt.ToString ( "HHmmss" ) + "," +
                 ( open + koeff ).ToString ().Replace ( ',', '.' ) + "," +
                 ( high + koeff ).ToString ().Replace ( ',', '.' ) + "," +
                 ( low + koeff ).ToString ().Replace ( ',', '.' ) + "," +
                 ( close + koeff ).ToString ().Replace ( ',', '.' ) + "," +
                 volume.ToString ();
        }
    }
}
