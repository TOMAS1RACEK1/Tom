using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;

namespace EvidencePojisteni
{
    internal class Evidence
    {
        private Databaze databaze;

        public Evidence()
        {
            databaze = new Databaze();
        }
        public void PridejPojisteneho()
        {
            string jmeno = ziskejUdaj("Zadejte jméno");
            string prijmeni = ziskejUdaj("Zadejte příjmení");

            Console.WriteLine("Zadejte telefonní číslo:");
            string telefon;
            while (string.IsNullOrEmpty(telefon = Console.ReadLine()) || !Regex.IsMatch(telefon, @"^\d{9}$"))
            {
                Console.WriteLine("Chybné zadání, zadejte číslo znovu");
            } 

            Console.WriteLine("Zadejte věk");
            int vek;
            
            while(!int.TryParse(Console.ReadLine(), out  vek) || vek < 0 || vek > 100)
            {
                Console.WriteLine("Chybné zadání, zadejte věk znovu");
            }
            databaze.PridejPojisteneho(jmeno, prijmeni, telefon, vek);
            Console.WriteLine();
            Console.WriteLine("Data byla uložena, pokračujte libovolnou klávesou..");
        }

        public void VypisVsechnyPojistene()
        {
            List<Pojistenec> pojistenci = databaze.VratVsechnyPojistene();
            foreach (Pojistenec p in pojistenci)
                Console.WriteLine(p);
            Console.WriteLine();
            Console.WriteLine("Pokračujte libovolnou klávesou");
        }

        public void VyhledejPojisteneho()
        {
            string jmeno = ziskejUdaj("Zadejte jméno");
            string prijmeni = ziskejUdaj("Zadejte příjmení");
            Pojistenec p = databaze.VyhledejPojisteneho(jmeno, prijmeni); 

            if (p != null)
            {
                Console.WriteLine(p);
            }
            else
            {
                Console.WriteLine("Pojistný nenalezen.");
            }
            Console.WriteLine();
            Console.WriteLine("Pokračujte libovolnou klávesou");
        }

        public void VypisUvodniObrazovku()
        {
            Console.Clear();
            Console.WriteLine("--------------------\n" +
                "Evidence pojištěných\n" +
                "--------------------");
            Console.WriteLine(); 
        } 
        private string ziskejUdaj(string prompt)  // metoda pro získání jména
        {
            Console.WriteLine(prompt);
            string udaj;
            while (string.IsNullOrWhiteSpace(udaj = Console.ReadLine()) || !Regex.IsMatch(udaj, @"^[^\d\s]+$"))  // ošetření vstupu od uživatele
            {
                Console.WriteLine("Zadej text znovu:");
            }
            return udaj;
        }
        //private string ziskejPrijmeni() // metoda pro získání příjmení
        //{
        //    Console.WriteLine("Zadejte příjmení");
        //    string prijmeni;
        //    while (string.IsNullOrWhiteSpace(prijmeni = Console.ReadLine()))  // ošetření vstupu od uživatele
        //    {
        //        Console.WriteLine("Zadej text znovu:");
        //    }
        //    return prijmeni;
        //}
    }
}
