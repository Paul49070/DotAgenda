using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Axis = LiveChartsCore.SkiaSharpView.Axis;
using System.Collections.ObjectModel;
using DotAgenda.Models;
using LiveChartsCore.SkiaSharpView.Painting.Effects;
using DotAgenda.MethodClass;
using DotAgenda.MethodClass.DataBaseMethods;

namespace DotAgenda.View
{
    /// <summary>
    /// Logique d'interaction pour ThirdView.xaml
    /// </summary>
    public partial class ThirdView : UserControl
    {
        GestionnaireEvent _global;
        Primitives _prim;
        DataBase _db;
        GlobalDict _dict;


        public Dictionary<string, int> NbParClasse_Year = new Dictionary<string, int>();
        public Dictionary<string, int> NbParClasse_Day = new Dictionary<string, int>();


        public ObservableCollection<double> ValeurMois = new ObservableCollection<double>() { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        public ObservableCollection<double> ValeurMoisLine = new ObservableCollection<double>();

        public ObservableCollection<string> NomMois = new ObservableCollection<string>() { "jan.", "fev.", "mar.", "avr.", "mai.", "jui.", "jui.", "aou.", "sep.", "oct.", "nov.", "dec." };
        public ObservableCollection<string> Label = new ObservableCollection<string>() { "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0" };
        public ObservableCollection<string> NomJours = new ObservableCollection<string>();

        public ObservableCollection<ClasseEvent_item> BoxClasseList = new ObservableCollection<ClasseEvent_item>();

        public DateTime DateActuelle_Graph;
        int yearAct;
        int _currentClasseIndex;
        string actualGraph = "jour";

        public SolidColorPaint LegendTextPaint { get; set; } =
        new SolidColorPaint
        {
            Color = new SKColor(50, 50, 50),
            SKTypeface = SKTypeface.FromFamilyName("Courier New")
        };

        public ISeries[] SeriesPie { get; set; } =
        {
            new PieSeries<double> { },
            new PieSeries<double> { },
            new PieSeries<double> { },
            new PieSeries<double> { },
        };

        public ISeries[] SeriesDashed { get; set; } =
        {
            new LineSeries<double> { },
        };


        public ISeries[] SeriesMonth { get; set; } =
        {

            new ColumnSeries<double>
            {
                Name = "Mary",
                Rx = 6,
                Ry = 6,
                Fill = new SolidColorPaint(SKColors.Red),
                IgnoresBarPosition = true
            }
        };

        public Axis[] XAxes { get; set; } =
        {

            new Axis
            {
                SeparatorsPaint = new SolidColorPaint(new SKColor(0, 0, 0, 0)),
                LabelsRotation = 0,
                SeparatorsAtCenter = false,
                TicksPaint = new SolidColorPaint(new SKColor(35, 35, 35)),
                TicksAtCenter = true,

            }
        };

        public Axis[] XAxesLine { get; set; } =
        {

            new Axis
            {
                ForceStepToMin= true,
                MinStep=1,
                SeparatorsPaint = new SolidColorPaint(new SKColor(0, 0, 0, 0)),
                LabelsRotation = 0,
                SeparatorsAtCenter = true,
                TicksPaint = new SolidColorPaint(new SKColor(35, 35, 35)),
                TicksAtCenter = true,
            }
        };

        public Axis[] YAxes { get; set; } =
            {
                new Axis
                  {
                    SeparatorsPaint = new SolidColorPaint(SKColor.Parse("#30696969")),
                  }
            };
        public ThirdView()
        {
            InitializeComponent();

            DataContext = this;

            _global = GestionnaireEvent._global;

            _dict = GlobalDict._dict;
            _db = DataBase._db;
            _prim = Primitives._prim;


            InitNbClasseJour();
            TXTboxInterval.Text = "3 jours";

            DateActuelle_Graph = _global._currentDay.Date;
            yearAct = DateActuelle_Graph.Year - DateTime.Today.Year + 1;
            //On créer le tableau de val pour le chart colonne

            _currentClasseIndex = 0;

            LegendTextPaint = new SolidColorPaint
            {
                Color = SKColor.Parse("#FFFFFF"),
                SKTypeface = SKTypeface.FromFile("/Font/#Poppins")
            };

            for (int i = 0; i < 4; ++i)
            {
                BoxClasseList.Add(new ClasseEvent_item());
            }

            BoxClasse.ItemsSource = BoxClasseList;
            RemplirComboBox();
            RemplirNbClasseYear();
            SetClasseBox(false, DateActuelle_Graph, 3);

            CreateColumnMonth(0);
        }

        private void RemplirComboBox()
        {
            ClasseSelection.ItemsSource = _dict.DictClasse.Values.ToList();
            ClasseSelection.SelectedItem = ClasseSelection.Items[0];
        }

        private void ClasseSelection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _currentClasseIndex = ClasseSelection.SelectedIndex;

            if (actualGraph == "mois")
            {
                CreateLine(true, DateActuelle_Graph, 0);
            }

            else if (actualGraph == "semaine")
            {
                CreateLine(false, DateActuelle_Graph, 7);
            }

            else
            {
                CreateLine(false, DateActuelle_Graph, 3);
            }
        }

        private void RemplirNbClasseYear()
        {
            string NomClasse;
            NbParClasse_Year.Clear();

            for (int j = 0; j < _dict.DictClasse.Count; ++j)
            {
                NomClasse = _global.A[yearAct].M[0].NbParClasse.ElementAt(j).Key;
                NbParClasse_Year.Add(NomClasse, 0);
                for (int i = 0; i < 12; ++i)
                {
                    NbParClasse_Year[NomClasse] += _global.A[yearAct].M[i].NbParClasse.ElementAt(j).Value;
                }
            }

            TriDict(NbParClasse_Year);
        }

        private void TriDict(Dictionary<string, int> Dict)
        {
            var ListTemp = Dict.ToList();

            ListTemp.Sort((i1, i2) => -i1.Value.CompareTo(i2.Value));

            Dict.Clear();

            foreach (var item in ListTemp)
            {
                Dict.Add(item.Key, item.Value);
            }
        }

        public void SetClasseBox(bool mois,DateTime debut, int nbJour)
        {
            ClasseEvent_item temp;

            BoxClasseList.Clear();

            if (mois)
            {
                for (int i = 0; i < 4; ++i)
                {
                    RemplirNbClasseYear();
                    temp = _prim.FindClasse(NbParClasse_Year.ElementAt(i).Key);
                    temp.Pourcentage = Pourcentage(NbParClasse_Year.ElementAt(i).Value, NbParClasse_Year.Values.Sum());
                    BoxClasseList.Add(temp);
                }
            }

            else
            {
                FindPlusGrandParClasse(debut, nbJour);

                for(int i = 0; i<4; ++i)
                {
                    temp = _prim.FindClasse(NbParClasse_Day.ElementAt(i).Key);
                    temp.Pourcentage = Pourcentage(NbParClasse_Day.ElementAt(i).Value, NbParClasse_Day.Values.Sum());
                    BoxClasseList.Add(temp);
                }
            }
            BoxClasse.ItemsSource = BoxClasseList;

            CreateLine(mois, debut, nbJour);
            SetTextStats(yearAct, nbJour);
        }

        private void InitNbClasseJour()
        {            
            for(int i = 0; i<_dict.DictClasse.Count; ++i)
            {
                NbParClasse_Day.Add(_dict.DictClasse.Values.ElementAt(i).Titre, 0);
            }
        }

        private void FindPlusGrandParClasse(DateTime debut, int nbJour)
        {
            NbParClasse_Day.Clear();
            InitNbClasseJour();

            for (int i = 0; i < nbJour; ++i)
            {
                foreach (EventDay item in _global.A[yearAct].M[debut.Month - 1].J[debut.Day - 1].ListeEvent)
                {
                    NbParClasse_Day[item.Classe] += 1;
                }
                debut = debut.AddDays(1);

            }

            TriDict(NbParClasse_Day);
        }
        
        
        public int Pourcentage(int nbre, int total)
        {
            if (total != 0)
                return (nbre * 100 / total);
            else return 0;
        }


        public void CreateColumnMonth(int index)
        {
            int numY = _global._currentDay.Date.Year - DateTime.Today.Year + 1;

            string titre = _global.A[numY].M[0].NbParClasse.ElementAt(index).Key;
            string couleur = _dict.DictClasse[titre].Couleur;

            UpdateMonthValue(index);

            var Column = new ColumnSeries<double>
            {
                Name = titre,
                Rx = 6,
                Ry = 6,
                Fill = new SolidColorPaint(SKColor.Parse(couleur)),
                Values = ValeurMois,
                IgnoresBarPosition = true
            };

            SeriesMonth[0] = Column;
            XAxes[0].Labels = NomMois;
            DataContext = this;
        }

        public void UpdateMonthValue(int index)
        {
            for (int i = 0; i < 12; i++)
            {
                ValeurMois[i] = _global.A[yearAct].M[i].NbParClasse.ElementAt(index).Value;
            }
        }





        public void CreateLine(bool mois, DateTime date, int nbJour)
        {
            var strokeThickness = 4;
            var strokeDashArray = new float[] { 3 * strokeThickness, 2 * strokeThickness };
            var effect = new DashEffect(strokeDashArray);

            string titre = _global.A[yearAct].M[0].NbParClasse.ElementAt(_currentClasseIndex).Key;
            string couleur = _dict.DictClasse[titre].Couleur;

            if (mois)
            {
                UpdateValueLine(_currentClasseIndex, yearAct, titre);
                for (int i = 0; i < 12; ++i)
                {
                    Label[i] = NomMois[i];
                }
                //Label = NomMois;
            }
            else
            {
                UpdateValueLineDays(date, nbJour);
                UpdateNomJours(nbJour);

                for(int i = 0; i<nbJour; ++i)
                {
                    Label[i] = NomJours[i];
                }

                //Label = NomJours;
            }

            var Line = new LineSeries<double>
            {
                Name = titre,
                Values = ValeurMoisLine,
                LineSmoothness = 1,
                GeometrySize = 11,
                GeometryStroke = new SolidColorPaint(SKColor.Parse(couleur), 4),
                Stroke = new SolidColorPaint
                {
                    Color = SKColor.Parse(couleur),
                    StrokeCap = SKStrokeCap.Round,

                    StrokeThickness = strokeThickness,
                    PathEffect = effect

                },

                Fill = null,
               
            };

            SeriesDashed[0] = Line;
            XAxesLine[0].Labels = Label;

            DataContext = this;
        }


        public void UpdateValueLine(int index, int numY, string classe)
        {
            ValeurMoisLine.Clear();
            for (int i = 0; i < 12; i++)
            {
                ValeurMoisLine.Add(_global.A[numY].M[i].NbParClasse[classe]);
            }

        }

        public void UpdateValueLineDays(DateTime debut, int nbJour)
        {
            string classe = _global.A[yearAct].M[0].NbParClasse.ElementAt(_currentClasseIndex).Key;


            ValeurMoisLine.Clear();
            for (int i = 0; i < nbJour; i++)
            {
                ValeurMoisLine.Add(NbreClasseEvent(classe, debut.AddDays(i)));
            }

        }

        private int NbreClasseEvent(string classe, DateTime date)
        {
            int compteur = 0;
            //int index = _mainView.A[yearAct].M[date.Month - 1].J[date.Day - 1].ListeEvent;
            foreach (EventDay item in _global.A[yearAct].M[date.Month - 1].J[date.Day - 1].ListeEvent)
            {
                if (item.Classe == classe) ++compteur;
            }

            return compteur;
        }

        private void Values_jours(object sender, RoutedEventArgs e)
        {
            actualGraph = "jour";
            SetClasseBox(false, DateActuelle_Graph, 3);

        }

        private void Values_Mois(object sender, RoutedEventArgs e)
        {
            actualGraph = "mois";
            SetClasseBox(true, DateActuelle_Graph, -1);
        }

        private void Values_Sem(object sender, RoutedEventArgs e)
        {
            actualGraph = "semaine";
            SetClasseBox(false, DateActuelle_Graph, 7);
        }

        private void Moins(object sender, RoutedEventArgs e)
        {
            if (actualGraph == "mois")
            {
                if (yearAct - 1 >= 0)
                {
                    --yearAct;
                    SetClasseBox(true, DateActuelle_Graph, -1);
                }
            }

            else if (actualGraph == "semaine")
            {
                if (DateActuelle_Graph.AddDays(-7) > new DateTime(2021, 12, 12))
                {
                    DateActuelle_Graph = DateActuelle_Graph.AddDays(-7);
                    SetClasseBox(false, DateActuelle_Graph, 7);
                }
            }

            else
            {
                if (DateActuelle_Graph.AddDays(-3) > new DateTime(2021, 12, 31))
                {
                    DateActuelle_Graph = DateActuelle_Graph.AddDays(-3);
                    SetClasseBox(false, DateActuelle_Graph, 3);
                }
            }
        }

        private void Plus(object sender, RoutedEventArgs e)
        {
            if (actualGraph == "mois")
            {
                if (yearAct + 1 < 4)
                {
                    ++yearAct;
                    SetClasseBox(true, DateActuelle_Graph, -1);
                }
            }

            else if (actualGraph == "semaine")
            {
                if (DateActuelle_Graph.AddDays(7) < new DateTime(2026, 01, 01))
                {
                    DateActuelle_Graph = DateActuelle_Graph.AddDays(7);
                    SetClasseBox(false, DateActuelle_Graph, 7);
                }
            }

            else
            {
                if (DateActuelle_Graph.AddDays(3) < new DateTime(2026, 01, 01))
                {
                    DateActuelle_Graph = DateActuelle_Graph.AddDays(3);
                    SetClasseBox(false, DateActuelle_Graph, 3);
                }
            }
        }

        private void SetTextStats(int numY, int nbreJour)
        {
            if (nbreJour == -1)
            {
                TextStats.Text = "(1 an - " + (numY + 2022) + ")";
                TXTboxInterval.Text = "1 an";
            }

            else
            {
                string debut = DateActuelle_Graph.ToString("ddd", new System.Globalization.CultureInfo("fr-FR")) + DateActuelle_Graph.Day;
                string fin = DateActuelle_Graph.AddDays(nbreJour - 1).ToString("ddd", new System.Globalization.CultureInfo("fr-FR")) + DateActuelle_Graph.AddDays(nbreJour - 1).Day;
                TextStats.Text = "(" + debut + " - " + fin + ")";

                if (nbreJour == 7)
                    TXTboxInterval.Text = "1 semaine";
                else TXTboxInterval.Text = "3 jours";
            }
        }

        private void UpdateNomJours(int nbJour)
        {
            string numJour;
            string nomMois;

            NomJours.Clear();

            for (int i = 0; i<nbJour; ++i)
            {
                numJour = DateActuelle_Graph.AddDays(i).Day.ToString();
                nomMois = DateActuelle_Graph.AddDays(i).ToString("MMM", new System.Globalization.CultureInfo("fr-FR"));

                NomJours.Add(numJour + " " + nomMois);
            }
        }
    }
}


