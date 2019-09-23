using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace DataPrepare
{
    class Contract
    {
        // private List<Bar> noNightsData = new List<Bar> ();
        private List<Bar> iniData = new List<Bar> ();
        // private List<Bar> adjustedData = new List<Bar> ();
        // private List<Bar> adjustedDataNoNights = new List<Bar> ();
        DateTime expirDate; // last bar date
        private DateTime prevExpirDate; // prev contract last bar date
        double adjustSize;
        double summAdjustSize;
        double expirClose;
        double prevExpirClose;
        private bool firstContractInDataSet = false;

        public DateTime ExpirDate { get => expirDate; }

        public double AdjustSize
        {
            get => adjustSize;
            set
            {
                adjustSize = value;
                if (value == 0) summAdjustSize = 0;
            }
        }
        public double SummAdjustSize { get => summAdjustSize; set => summAdjustSize = value; }
        public double ExpirClose { get => expirClose; }
        public double PrevExpirClose { get => prevExpirClose; }


        // конструктор для первого контракта (самого раннего)
        public Contract ( string fileName, bool isContractFirst )
        {
            if (isContractFirst) firstContractInDataSet = true;
            ConstructHelper ( fileName );

        }

        // конструктор для всех контрактов кроме первого
        public Contract ( string fileName, DateTime prevExpir )
        {
            prevExpirDate = prevExpir;
            ConstructHelper ( fileName );
        }

        // общая часть для обоих конструкторов
        void ConstructHelper ( string fName )
        {
            if (File.Exists ( fName ))
            {
                using (StreamReader sr = File.OpenText ( fName ))
                {
                    string s;
                    while (( s = sr.ReadLine () ) != null)
                    {
                        if (!ParseHeader1 ( s )) // не первая строка (шапка с названием полей)
                        {
                            Bar b = new Bar ( s );
                            iniData.Add ( b ); // добавили бары в контракт
                        }
                    }
                }
            }
            expirDate = new DateTime (
                iniData[iniData.Count - 1].Dt.Year,
                iniData[iniData.Count - 1].Dt.Month,
                iniData[iniData.Count - 1].Dt.Day,
                Prog.expirHour, Prog.expirMin, Prog.expirSec );

            if (firstContractInDataSet) prevExpirClose = iniData[0].Close;
            foreach (var item in iniData)
            {
                if (item.Dt == expirDate) expirClose = item.Close;
                if (!firstContractInDataSet)
                {
                    if (item.Dt == prevExpirDate) prevExpirClose = item.Close;
                }
            }

        }

        // парсим строку из csv файла и проверяем является ли она первой строкой формата
        // <DATE>,<TIME>,<OPEN>,<HIGH>,<LOW>,<CLOSE>,<VOL>
        static bool ParseHeader1 ( string str )
        {
            string[] words = str.Split ( ',' );
            if (words[0] == "<DATE>")
                return true;
            else
                return false;
        }

        public override string ToString ()
        {

            for (int i = 0; i < iniData.Count; i++)
            {
                Console.WriteLine ( iniData[i] );
            }
            return null;
        }

        // отдает коллекцию строк строго от прошлого экспира до текущего (12 часов посл дня)
        // все OHLC баров увеличено на adjustSize БЕЗ вечерок
        public List<string> AdjustNoNights ()
        {
            List<string> ls = new List<string> ();
            foreach (var bar in iniData)
            {
                if (bar.Dt > prevExpirDate & bar.OnlyTime <= Prog.vecherkaTime & bar.Dt <= ExpirDate)
                {
                    ls.Add ( bar.AjustedString ( summAdjustSize ) );
                }
            }
            return ls;
        }

        // отдает коллекцию строк от прошлого экспира до текущего бары увеличены на adjustSize
        // с вечерками
        public List<string> Ajust ()
        {
            List<string> ls = new List<string> ();
            foreach (var bar in iniData)
            {
                if (bar.Dt > prevExpirDate & bar.Dt <= ExpirDate)
                {
                    ls.Add ( bar.AjustedString ( summAdjustSize ) );
                }
            }
            return ls;
        }

        // отдает коллекцию строк БЕЗ вечерки
        public List<string> NoNights ()
        {
            List<string> ls = new List<string> ();
            foreach (var bar in iniData)
            {
                if (bar.OnlyTime <= Prog.vecherkaTime & bar.Dt <= ExpirDate)
                {
                    ls.Add ( bar.ToString () );
                }
            }
            return ls;
        }
    }
}

