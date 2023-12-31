using MySql.Data.MySqlClient; //!!
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KonyvtarAsztali
{
    public class Statisztika
    {

        private MySqlConnection connection;
        private List<Konyv> konyvek = new List<Konyv>();

        public Statisztika()
        {
            MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder();
            builder.Server = "localhost";
            builder.Port = 3306;
            builder.UserID = "root";
            builder.Password = "";
            builder.Database = "library";
            connection = new MySqlConnection(builder.ToString());
        }

        public List<Konyv> Konyvek { get => konyvek; set => konyvek = value; }

        //kapcsolat megnyitása
        private void ConnectionOpener()
        {
            try
            {
                if (connection.State == System.Data.ConnectionState.Closed)
                {
                    connection.Open();
                }
            }
            catch (MySqlException e)
            {
                throw new Exception("Nem sikerült a kapcsolatot megnyitni: " + e.Message);
            }
        }

        //kapcsolat bezárása
        private void ConnectionCloser()
        {
            try
            {
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    connection.Close();
                }
            }
            catch (MySqlException e)
            {
                throw new Exception("Nem sikerült a kapcsolatot bezárni: " + e.Message);
            }
        }

        public void Feltoltes()
        {
            konyvek = new List<Konyv>();
            ConnectionOpener();
            string sql = "SELECT * FROM books";
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = sql;
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    konyvek.Add(new Konyv(reader.GetInt32("id"), reader.GetString("title"), reader.GetString("author"), reader.GetInt32("publish_year"), reader.GetInt32("page_count")));
                }
            }
            ConnectionCloser();
        }

        public void OtszaznalHosszabb()
        {
            int db = 0;
            foreach (Konyv k in konyvek)
            {
                if (k.Page_count > 500)
                {
                    db++;
                }
            }
            Console.WriteLine("500 oldalnál hosszabb könyvek száma:  " + db);
        }
        public void EzerkilencszazOtvennelRegebbi()
        {
            int db = 0;
            foreach (Konyv k in konyvek)
            {
                if (k.Publish_year < 1950)
                {
                    db++;
                }
            }
            if (db >= 1)
            {
                Console.WriteLine("Van 1950-nél régebbi könyv");
            }
            else
            {
                Console.WriteLine("Nincs 1950-nél régebbi könyv");
            }
        }
        public void Leghosszabb()
        {
            int max = 0;
            foreach (Konyv k in konyvek)
            {
                if (k.Page_count > max)
                {
                    max = k.Page_count;
                }
            }
            foreach (Konyv k in konyvek)
            {
                if (k.Page_count == max)
                {
                    Console.WriteLine("A leghosszabb könyv:");
                    Console.WriteLine("Szerző:" + k.Author);
                    Console.WriteLine("Cím:" + k.Title);
                    Console.WriteLine("Kiadás éve:" + k.Publish_year);
                    Console.WriteLine("Oldalszám:" + k.Page_count);
                }
            }
        }
        public void LegtobbKonyvvelRendelkezoSzerzo()
        {
            Dictionary<string, int> szerzok = new Dictionary<string, int>();
            foreach (Konyv k in konyvek)
            {
                if (szerzok.ContainsKey(k.Author))
                {
                    szerzok[k.Author]++;
                }
                else
                {
                    szerzok.Add(k.Author, 1);
                }
            }
            int max = 0;
            string szerzo = "Hiba történt!";
            foreach (KeyValuePair<string, int> item in szerzok)
            {
                if (item.Value > max)
                {
                    max = item.Value;
                    szerzo = item.Key;
                }
            }
            Console.WriteLine("A legtöbb könyvvel rendelkező szerző: " + szerzo);
        }

        //?????
        /*
        public void KolcsonzesSzam()
        {
            Console.WriteLine("Adjon meg egy könyv címet: ");
            ConnectionOpener();
            string sql = "SELECT COUNT(*) FROM borrows";
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = sql;
            int db = Convert.ToInt32(command.ExecuteScalar());
            Console.WriteLine($"A(z) megadott könyv {db}x lett kikölcsönözve: ");
            ConnectionCloser();
        }
        */

        //sorok törlése
        public bool Torles(int id)
        {
            ConnectionOpener();
            string sql = "DELETE FROM books WHERE id = @id";
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = sql;
            command.Parameters.AddWithValue("@id", id);
            int sorok = command.ExecuteNonQuery();
            ConnectionCloser();
            return sorok == 1;
        }
    }
}
