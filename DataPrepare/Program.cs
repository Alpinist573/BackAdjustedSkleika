﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
/// <summary>
/// из файлов csv с данными формата:
/// <DATE>,<TIME>,<OPEN>,<HIGH>,<LOW>,<CLOSE>,<VOL>
/// 20101101,100500,31084.0000000,31084.0000000,31040.0000000,31040.0000000,3
/// находящихся в папке folderPath
/// вырезается вечерка (все что позже vecherkaTime)
/// создается поддирректория NoNightData и туда под теми же именами
/// файлы уже без вечерки перезаписываются
/// ЗАМОРОЧКА с ТС ЛАБОМ, csv файл надо скопировать (содержимое) в чистый файл в нотепад++ и сохр как тхт,
/// просто переименовать недостаточно
/// =============================================
/// 
/// </summary>

namespace DataPrepare
{
    class Prog
    {
        static string firstString = "<DATE>,<TIME>,<OPEN>,<HIGH>,<LOW>,<CLOSE>,<VOL>";

        static string resultPath = @"C:\Users\Sergei Levit\Documents\База\Биржа\Данные\";
        
        public static int vecherkaTime = 184500;

        public static int expirTime = 120000;
        public static int expirHour = 12;
        public static int expirMin = 00;
        public static int expirSec = 00;

        static DateTime errorData = DateTime.Parse ( "01.01.0001 0:00:00" );
        static List<string> allDataFromFile = new List<string> (); // список со всеми данными из файла

        static void Main ( string[] args )
        {


          

            List<string> Paths = new List<string>();
            Paths.Add(@"C:\Users\Sergei Levit\Documents\База\Биржа\Данные\Gold 15 min\");
            Paths.Add(@"C:\Users\Sergei Levit\Documents\База\Биржа\Данные\Br 15 min\");
            Paths.Add(@"C:\Users\Sergei Levit\Documents\База\Биржа\Данные\Eu 15 min\");
            Paths.Add(@"C:\Users\Sergei Levit\Documents\База\Биржа\Данные\Sber 15 min\");
            Paths.Add(@"C:\Users\Sergei Levit\Documents\База\Биржа\Данные\Ri 15 min\");
            Paths.Add(@"C:\Users\Sergei Levit\Documents\База\Биржа\Данные\Si 15 min\");
            Paths.Add(@"C:\Users\Sergei Levit\Documents\База\Биржа\Данные\Sber 5 min\");
            Paths.Add(@"C:\Users\Sergei Levit\Documents\База\Биржа\Данные\Ri 5 min\");
            Paths.Add(@"C:\Users\Sergei Levit\Documents\База\Биржа\Данные\Eu 5 min\");
            Paths.Add(@"C:\Users\Sergei Levit\Documents\База\Биржа\Данные\Si 5 min\");

            foreach (var folderPath in Paths)
            {
                string[] tmpstr = folderPath.Split('\\');
                string newFileName = tmpstr[tmpstr.Count() - 2];
               

                Console.WriteLine("=========================");
                Console.WriteLine(newFileName);

                string[] files = Directory.GetFiles(folderPath);
                Contract[] contracts = new Contract[files.Length];

                for (int i = 0; i < files.Length; i++)
                {
                    if (i == 0) contracts[i] = new Contract(files[i], true);
                    else contracts[i] = new Contract(files[i], contracts[i - 1].ExpirDate);

                }

                // пихает в отдельную папку контракты без вечерок (отрезаны по экспир)
                for (int i = 0; i < files.Length; i++)
                {
                    // listToFileWriter(FileNameExtractor(files[i]), @"nonightdata\", contracts[i].NoNights(), folderPath);
                }

                // высчитывает adjustSize и суммированный adjustSize

                for (int i = contracts.Length - 1; i >= 0; i--)
                {
                    if (i == contracts.Length - 1) contracts[i].AdjustSize = 0;
                    else
                    {
                        contracts[i].AdjustSize = contracts[i + 1].PrevExpirClose - contracts[i].ExpirClose;
                        contracts[i].SummAdjustSize = contracts[i + 1].SummAdjustSize + contracts[i].AdjustSize;
                    }
                }
                for (int i = 0; i < contracts.Length; i++)
                {
                    Console.WriteLine("adjSize = {0}, summAdj = {1}", contracts[i].AdjustSize, contracts[i].SummAdjustSize);
                }

                // пихает в отдельную папку adjusted с вечеркой - один файл склейка
                List<string> adj = new List<string>();
                // adj.Add ( firstString );
                for (int i = 0; i < contracts.Length; i++)
                {
                    List<string> ls = contracts[i].Ajust();
                    foreach (var item in ls)
                    {
                        adj.Add(item);
                    }
                }
                listToFileOverWriter(newFileName +".txt", @"adjustWithNights\", adj, resultPath);

                // пихает в отдельную папку adjusted без вечерки - один файл склейка
                adj.Clear();
                // adj.Add ( firstString );
                for (int i = 0; i < contracts.Length; i++)
                {
                    List<string> ls = contracts[i].AdjustNoNights();
                    foreach (var item in ls)
                    {
                        adj.Add(item);
                    }
                }
                // listToFileWriter("adjNoNights.csv", @"adjustNoNights\", adj, folderPath);

                Console.WriteLine("");
                
            }
            Console.ReadKey ();



        }

        // извлекает имя файла из полного пути
        static string FileNameExtractor ( string fullPathwithName )
        {
            return new FileInfo ( fullPathwithName ).Name;
        }

        // записывает List в созданный файл, если такого файла еще нет
        static void listToFileWriter ( string fileName, string newFolder, List<string> ls, string folderPath1 )
        {

            Directory.CreateDirectory ( folderPath1 + newFolder );
            string path = folderPath1 + newFolder + fileName;

            if (!File.Exists ( path ))
            {
                using (StreamWriter sw = File.CreateText ( path ))
                {
                    sw.WriteLine ( firstString );
                    for (int i = 0; i < ls.Count; i++)
                    {
                        sw.WriteLine ( ls[i] );
                    }
                }
            }
            else
            {
                throw new Exception ( string.Format ( "Такой файл - {0} уже есть !!", fileName ) );
            }
        }

        // записывает List, если файл такой есть - перезаписывает
        static void listToFileOverWriter(string fileName, string newFolder, List<string> ls, string folderPath1)
        {

            Directory.CreateDirectory(folderPath1 + newFolder);
            string path = folderPath1 + newFolder + fileName;

            
                using (StreamWriter sw = File.CreateText(path))
                {
                    sw.WriteLine(firstString);
                    for (int i = 0; i < ls.Count; i++)
                    {
                        sw.WriteLine(ls[i]);
                    }
                }
           
        }

        // парсим строку из csv файла и выстаскиваем оттуда дату и время свечи, 
        // если строка кривая - отдаст 01.01.0001 0:00:00
        static DateTime parseDateTimeFromString ( string str )
        {
            string[] words = str.Split ( ',' );
            string pattern = "yyyyMMddHHmmss";

            DateTime dt;
            DateTime.TryParseExact ( words[0] + words[1], pattern, null, System.Globalization.DateTimeStyles.None, out dt );
            // Console.WriteLine ( dt );
            // Console.ReadKey ();
            return dt;
        }


        // парсим строку из csv файла и проверяем является ли она первой строкой формата
        // <DATE>,<TIME>,<OPEN>,<HIGH>,<LOW>,<CLOSE>,<VOL>
        static bool parseHeader ( string str )
        {
            string[] words = str.Split ( ',' );
            if (words[0] == "<DATE>")
                return true;
            else
                return false;
        }

        // читает из файла построчно и отфильтровав вечерку 
        // (все что позже vecherkatime) пихает в List
        static List<string> ReadFileToList ( string fileName )
        {
            if (File.Exists ( fileName ))
            {
                using (StreamReader sr = File.OpenText ( fileName ))
                {
                    string s;
                    while (( s = sr.ReadLine () ) != null)
                    {
                        DateTime dt = parseDateTimeFromString ( s );
                        if (dt == errorData)
                        {
                            bool x = parseHeader ( s );
                            if (x) allDataFromFile.Add ( s );
                            // Console.WriteLine ( s );
                        }
                        else
                        {
                            string min;
                            if (dt.Minute < 10) { min = "0" + dt.Minute.ToString (); }
                            else { min = dt.Minute.ToString (); }

                            int barTime = Convert.ToInt32 ( ( dt.Hour.ToString () + min + "00" ) );
                            if (barTime <= vecherkaTime)
                            {
                                allDataFromFile.Add ( s );
                                // Console.WriteLine ( s );
                            }

                        }
                    }
                }
                // Console.ReadKey ();
                return allDataFromFile;
            }
            else throw new Exception ( "Нот сач филе !! ))" );
        }
    }

}
