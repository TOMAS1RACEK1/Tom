using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvidencePojisteni
{
    internal class Databaze
    { 
        private List<Pojistenec> pojistenciList; 

        public Databaze() 
        {
            pojistenciList = new List<Pojistenec>();  
        }
        public void PridejPojisteneho(string jmeno, string prijmeni, string telefon, int vek)
        {
            pojistenciList.Add(new Pojistenec( jmeno, prijmeni, telefon, vek));
        } 
        public List<Pojistenec> VratVsechnyPojistene()
        {
            return pojistenciList; 
        } 
        public Pojistenec  VyhledejPojisteneho(string jmeno, string prijmeni)
        {
            foreach (Pojistenec p in pojistenciList)
            {
                if (p.Jmeno == jmeno && p.Prijmeni == prijmeni)
                {
                    return p;
                }
            }
           // Console.WriteLine("Uživatel nenalezen");
            return null;          
        }
        
            
        



    }
}
