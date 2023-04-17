using System;
using DotAgenda.MethodClass;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Globalization.DateTimeFormatting;
using Newtonsoft.Json;
using System.IO.Pipes;
using System.IO;
using System.Windows;

namespace DotAgenda.Models
{
    public class Alerte
    {
        private EventDay _Evenement;
        public EventDay Evenement
        {
            get { return _Evenement; }
            set 
            {
                if (_Evenement != value)
                    _Evenement = value;
            }
        }

        private DateTime _HeureEnvoi;
        public DateTime HeureEnvoi
        {
            get { return _HeureEnvoi; }
            set
            {
                if (_HeureEnvoi != value)
                    _HeureEnvoi = value;
            }
        }

        private TypeMoyenEnvoie _MoyenEnvoi;
        public TypeMoyenEnvoie MoyenEnvoi
        {
            get { return _MoyenEnvoi; }
            set
            {
                if (_MoyenEnvoi != value)
                    _MoyenEnvoi = value;
            }
        }

        private DelaiType _TypeDelai;
        public DelaiType TypeDelai
        {
            get { return _TypeDelai; }
            set
            {
                if (_TypeDelai != value)
                    _TypeDelai = value;
            }
        }

        private int _Delai;
        public int Delai
        {
            get { return _Delai; }
            set
            {
                if (_Delai != value)
                    _Delai = value;
            }
        }

        public enum TypeMoyenEnvoie
        {
            Mail,
            Notification,
            Both
        }

        public enum DelaiType
        {
            Minute,
            Hour,
            Day,
            Week
        }

        private string _ID;

        public string ID
        {
            get { return _ID; }
        }

        //Type + number

        public Alerte(EventDay e, int delai, TypeMoyenEnvoie moyenEnvoie, DelaiType delaiType)
        {
            this._ID = Primitives._prim.GenerateID();
            this.Evenement = e;
            this.Delai = delai;
            this.MoyenEnvoi = moyenEnvoie;
            this.TypeDelai = delaiType;

            switch(delaiType)
            {
                case DelaiType.Minute:
                    this.HeureEnvoi = e.DateDebut.AddMinutes(-Delai);
                    break;
                case DelaiType.Hour:
                    this.HeureEnvoi = e.DateDebut.AddHours(-Delai);
                    break;
                case DelaiType.Day:
                    this.HeureEnvoi = e.DateDebut.AddDays(-Delai);
                    break;
                case DelaiType.Week:
                    this.HeureEnvoi = e.DateDebut.AddDays(-(Delai * 7));
                    break;
            }

            SendAlertPipe("Add");
        }

        public void SendAlertPipe(string Type)
        {
            AlertProtocolJSON aJSON = new AlertProtocolJSON(this, Type);

            string alertsJson = JsonConvert.SerializeObject(aJSON);

            using (NamedPipeClientStream pipeClient = new NamedPipeClientStream(".", "AlertsPipe", PipeDirection.Out))
            {
                try
                {
                    pipeClient.Connect(5000);

                    using (StreamWriter writer = new StreamWriter(pipeClient))
                    {
                        writer.WriteLine(alertsJson);
                        writer.Flush();
                    }
                }

                catch (TimeoutException e)
                {
                    Console.WriteLine("Impossible de se connecter au Pipe AlertsPipe");
                }

            }
        }
    }
}
