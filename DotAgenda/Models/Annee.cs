using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotAgenda.Models
{
    public class Annee
    {
        private bool _Bisextile;
        public bool Bisextile
        {
            get { return _Bisextile; }
        }

        private int _Num;
        public int Num
        {
            get { return _Num; }
            set 
            { 
                _Num = value;
                _Bisextile = DateTime.IsLeapYear(_Num);
            }
        }


        private Mois[] _M;
        public Mois[] M
        {
            get { return _M; }
            set { _M = value; }
        }

        public Annee()
        {
            M = new Mois[12];
        }
    }
}
