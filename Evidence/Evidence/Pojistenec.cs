using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvidencePojisteni
{
    internal class Pojistenec
    {
        public string Jmeno { get; private set; } 
        public string Prijmeni { get; private set; }
        public string Telefon { get; private set; }
        public int Vek { get; private set; } 

        public Pojistenec(string jmeno, string prijmeni, string telefon, int vek)
        {
            this.Jmeno = jmeno;
            this.Prijmeni = prijmeni;
            this.Telefon = telefon;
            this.Vek = vek;
        }
        public override string ToString()
        {
            return Jmeno + "\t" + Prijmeni + "\t" + Telefon + "\t" + Vek;
        }
    }
}
