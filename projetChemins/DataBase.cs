using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace algoDarwin
{
    class DataBase
    {
        private string _dbPath = "../db.db3";
        private static DataBase db;
        public static DataBase getDataBase()
        {
            if (db == null)
            {
                db = new DataBase();
            }
            return db;
        }
        private DataBase() { }

        public void createTableVille()
        {
            SQLite.SQLiteConnection connection = new SQLite.SQLiteConnection(_dbPath);
            connection.CreateTable<Ville>();
        }

        public void createTableParams()
        {
            SQLite.SQLiteConnection connection = new SQLite.SQLiteConnection(_dbPath);
            connection.CreateTable<Params>();
        }
        //Insertion Ville
        public void InsertVille(Ville v)
        {
            Console.WriteLine("begin Insert");
            SQLite.SQLiteConnection connection = new SQLite.SQLiteConnection(_dbPath);
            connection.CreateTable<Ville>();
            connection.Insert(v);

            Console.WriteLine("end Insert");

        }

        //Select Ville
        public List<Ville> Select(string query)
        {

            SQLite.SQLiteConnection connection = new SQLite.SQLiteConnection(_dbPath);
            List<Ville> villes = connection.Query<Ville>(query);
            foreach (Ville v in villes)
            {
                Console.WriteLine("ville => " + v.ID + " " + v.NomVille + " " + v.XVille + " " + v.YVille);
            }

            return villes;
        }

        //Insertion Params
        public void InsertParams(Params p)
        {
            Console.WriteLine("begin Insert Params");
            SQLite.SQLiteConnection connection = new SQLite.SQLiteConnection(_dbPath);
            connection.CreateTable<Params>();
            connection.InsertOrReplace(p);
            Console.WriteLine("end Insert Params");

        }
        //Select ¨Params
        public List<Params> Select_Params(string query)
        {

            SQLite.SQLiteConnection connection = new SQLite.SQLiteConnection(_dbPath);
            List<Params> listParams = connection.Query<Params>(query);
            foreach (Params v in listParams)
            {
                Console.WriteLine("Params => " + v.Key + " " + v.Value);
            }
            return listParams;
        }
        public void Delete(Ville v)
        {
            Console.WriteLine("begin Delete");
            SQLite.SQLiteConnection connection = new SQLite.SQLiteConnection(_dbPath);
            connection.Delete<Ville>(v.ID);
            Console.WriteLine("end Delete");

        }

        public void Delete(Params p)
        {
            Console.WriteLine("begin Delete param");
            SQLite.SQLiteConnection connection = new SQLite.SQLiteConnection(_dbPath);
            connection.Delete(p);
            Console.WriteLine("end Delete param");

        }


        public void Update_Params(Params p)
        {
            Console.WriteLine("begin Update param");
            SQLite.SQLiteConnection connection = new SQLite.SQLiteConnection(_dbPath);
            connection.InsertOrReplace(p);
            Console.WriteLine("end Update param");
        }
    }
}
