﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Dynamic;
using System.Reflection;

namespace ExerciseDB
{
    class Program
    {
        static void Main(string[] args)
        {
            string connStr = "Server=localhost;Database=orders;Trusted_Connection=True;";
            SqlConnection con = new SqlConnection(connStr);


            using (con)
            {
                Console.WriteLine($"connessione: {con}");
                con.Open();
                Console.WriteLine("connessione aperta");
                check(con);
                while (true)
                {
                    Console.WriteLine("Benvenuto nella pagina login");
                    Console.WriteLine("-----------------------------------------");
                    Console.WriteLine("selezionare un numero per eseguire operazioni");
                    Console.WriteLine("-----------------------------------------");
                    Console.WriteLine("selezionare 1 per creare un utente");
                    Console.WriteLine("-----------------------------------------");
                    Console.WriteLine("selezionare 2 per loggarsi");
                    Console.WriteLine("-----------------------------------------");
                    Console.WriteLine("selezionare 3 per visualizzare la lista degli ordini");
                    Console.WriteLine("-----------------------------------------");
                    Console.WriteLine("selezionare 4 per visualizzare il dettaglio dell'ordine");
                    Console.WriteLine("-----------------------------------------");
                    Console.WriteLine("selezionare 5 per creare un nuovo ordine");
                    Console.WriteLine("-----------------------------------------");
                    Console.WriteLine("selezionare 6 per uscire");
                    Console.WriteLine("-----------------------------------------");
                    int options = (int)Console.Read();
                    switch (options)
                    {
                        case 1:
                            createUser(con);
                            break;
                        case 2:
                            Login(con);
                            break;
                        case 3:
                            listOrder(con);
                            break;
                        case 4:
                            detailOrder(con);
                            break;
                        case 5:
                            createOrder(con);
                            break;
                        case 6:
                            //exit();
                            break;
                        default:
                            Console.WriteLine("selezionare un numero valido");
                            Console.WriteLine("-----------------------------------------");
                            return;
                    }
                    //createTable(con);
                }
                con.Close();
                Console.WriteLine("connessione chiusa");
                Console.ReadLine();

            }
        }

        //public static void createTable(SqlConnection con)
        //{
        //    string q = $"use orders \n" +
        //        $"create table Utenti( \n" +
        //        $"id int primary key, \n" +
        //        $"login varchar(20) not null, \n" +
        //        $"password varchar(20) not null \n" +
        //        $")";
        //    var cmd = new SqlCommand(q, con);
        //    insertUser(con);
        //}

        public class User
        {
            public static string username { get; set; }
            public static string password { get; set; }
            public User(string u, string p)
            {
                username = u; password = p;
            }
        }

        public static void check(SqlConnection con)
        {
            var cmd = new SqlCommand("select * from Utenti", con);
            using (var utenti = cmd.ExecuteReader())
            {
                Console.WriteLine("tabella non vuota procedere con il login");
                Console.WriteLine("-----------------------------------------");
            }
            insertUser(con);
        }

        public static void insertUser(SqlConnection con)
        {
            string username = "admin";
            string password = "admin";
            
            
            string q = $"insert into Utenti values ('{username}', '{password}')";
             
            var cmd = new SqlCommand(q, con);
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("utente già registrato");
                Console.WriteLine("-----------------------------------------");
            }  
                //using (var utenti = cmd.ExecuteReader())
                //{
                //    readUsers(con);
                //}
            //readUsers(con);
                        
            //SqlParameter par1 = new SqlParameter("@username", SqlDbType.VarChar, 50);
            //cmd.Parameters.Add(par1);
            //par1.Value = username;
            //SqlParameter par2 = new SqlParameter("@password", SqlDbType.VarChar, 50);
            //cmd.Parameters.Add(par2);
            //par2.Value = password;
            //cmd.Parameters.Add(new SqlParameter("@username", 1));
            //cmd.Parameters.Add(new SqlParameter("@password", 1));
            //Console.WriteLine($"ho modificato {cmd.ExecuteNonQuery()} righe");
            
        }
        public static void Login(SqlConnection con)
        {
            Console.WriteLine("Inserire il nome utente");
            string username = Console.ReadLine();
            Console.WriteLine("-----------------------------------------");
            Console.WriteLine("Inserire la password");
            string password = Console.ReadLine();
            Console.WriteLine("-----------------------------------------");
            string q = $"select * from Utenti where login = '{username}' and password = '{password}'";
            
            var cmd = new SqlCommand(q, con);
            using (var utenti = cmd.ExecuteReader())
            {
                if (utenti.Read())
                { 
                    Console.WriteLine($"benvenuto {username}");
                    Console.WriteLine("--> {0} {1}", utenti["login"], utenti["password"]);
                    Console.WriteLine("-----------------------------------------");
                    new User(username, password);
                }
            }
        }

        public static void readUsers(SqlConnection con)
        {
            var cmd = new SqlCommand("select * from Utenti", con);
            using (var utenti = cmd.ExecuteReader())
            {
                while (utenti.Read())
                {
                    Console.WriteLine("--> {0} {1}", utenti["login"], utenti["password"]);
                    Console.WriteLine("-----------------------------------------");
                }
            }
            
        }

        public static void createUser(SqlConnection con)
        {
            Console.WriteLine("Inserire il nome utente");           
            string username = Console.ReadLine();
            Console.WriteLine("-----------------------------------------");
            Console.WriteLine("Inserire la password");            
            string password = Console.ReadLine();
            Console.WriteLine("-----------------------------------------");


            string q = $"insert into Utenti values ('{username}', '{password}')";

            var cmd = new SqlCommand(q, con);
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("utente già registrato");
                Console.WriteLine("-----------------------------------------");
                //menu();
            }
        }

        public static void listOrder(SqlConnection con)
        {
            string q = "select o.orderid, customer, orderdate, sum(price*qty) as totale \n" +
                " from orders as o \n" +
                "inner join orderitems as i \n " +
                "on o.orderid = i.orderid \n" +
                "group by o.orderid, customer, orderdate;";
            var cmd = new SqlCommand(q, con);
            using (var orders = cmd.ExecuteReader())
            {
                while (orders.Read())
                {
                    Console.WriteLine("{0} {1} {2} {3}", orders["orderid"], orders["customer"], orders["orderdate"], orders["totale"]);
                }
            }
        }

        public static void detailOrder(SqlConnection con)
        {
            Console.WriteLine("inseire id dell'ordine");
            int ord = (int)Console.Read();
            Console.WriteLine("-----------------------------------------");
            string q = $"select orderid, customer, orderdate, item, qty, price" +
                "from orders" +
                "where orderid = @ord";
            var cmd = new SqlCommand(q, con);
            cmd.Parameters.Add(new SqlParameter("id", ord));
            
            using (var orders = cmd.ExecuteReader())
            {
                if (orders.Read())
                {
                    Console.WriteLine("{0} {1} {2}", orders["orderid"], orders["customer"], 
                        orders["orderdate"]);
                }
                else
                {
                    Console.WriteLine("ordine non trovato");
                    Console.WriteLine("-----------------------------------------");
                }
            }

            string r = "select orderid, item, qty, price" +
                "from orderitems" +
                "where orderid = @ord";
            var cmd1 = new SqlCommand(r, con);
            cmd1.Parameters.Add(new SqlParameter("id", ord));

            using (var orders = cmd1.ExecuteReader())
            {
                if (orders.Read())
                {
                    Console.WriteLine("{0} {1} {2} {3}", orders["orderid"],
                        orders["item"], orders["qty"], orders["price"]);
                }
                else
                {
                    Console.WriteLine("ordine non trovato");
                    Console.WriteLine("-----------------------------------------");
                }
            }
        }

        public static void createOrder(SqlConnection con)
        {
            DateTime date = DateTime.Now;
            Console.WriteLine("Aggiunta nuovo prodotto ... ");
            Console.WriteLine("-----------------------------------------");
            Console.WriteLine("Inserire il prodotto");
            string prodotto = Console.ReadLine();
            Console.WriteLine("-----------------------------------------");
            Console.WriteLine("Inserire la quantità");
            int quantità = (int)Console.Read();
            Console.WriteLine("-----------------------------------------");
            Console.WriteLine("Inserire il prezzo");
            double prezzo = (double)Console.Read();
            Console.WriteLine("-----------------------------------------");

            string r = $"insert into orders values ('', '{User.username}', '{date}')";
            var cmd = new SqlCommand(r, con);
            cmd.ExecuteNonQuery();

            //string s = $"select orderid from orders where customer = {User.username}";
            //var cmd1 = new SqlCommand(s, con);
            //var id = cmd1.ExecuteReader();

            //string q = $"insert into orderitems values ('{id}', '{prodotto}', '{quantità}', '{prezzo}')";
            //var cmd2 = new SqlCommand(q, con);
            //cmd2.ExecuteNonQuery();
        }
    }
}

