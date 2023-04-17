using DotAgenda.Core;
using DotAgenda.MethodClass;
using DotAgenda.Models;
using DotAgenda.ViewModels;
using static DotAgenda.Models.EventDay;
using System.Collections.Generic;
using System.Linq;

namespace DotAgenda.ViewModels
{
    class MoreEventPageAdvancedViewModel
    {
        private MoreEventPageViewModel _MoreEventViewModel;

        public MoreEventPageViewModel MoreEventViewModel
        {
            get { return _MoreEventViewModel; }
            set
            {
                _MoreEventViewModel = value;
            }
        }

        private EventDay _newEvent;

        public EventDay newEvent
        {
            get { return _newEvent; }
            set
            {
                if (_newEvent != value)
                    _newEvent = value;
            }
        }

        private List<string> _ReccurenceString;
        public List<string> ReccurenceString
        {
            get { return _ReccurenceString; }
            set
            {
                _ReccurenceString = value;
            }
        }

        Dictionary<Alerte.DelaiType, string> Dict = new Dictionary<Alerte.DelaiType, string>
            {
                { Alerte.DelaiType.Minute, "minutes avant" },
                { Alerte.DelaiType.Hour, "heures avant" },
                { Alerte.DelaiType.Day, "jours avant" },
                { Alerte.DelaiType.Week, "semaines avant" },
            };

        private List<string> _AlertesString = new List<string>();

        public List<string> AlertesString
        {
            get { return _AlertesString; }
            set
            {
                _AlertesString = value;
            }
        }

        private string _SelectedAlertes;
        public string SelectedAlertes
        {
            get { return _SelectedAlertes; }
            set
            {
                _SelectedAlertes = value;
            }
        }


        private string _SelectedReccurence;
        public string SelectedReccurence
        {
            get { return _SelectedReccurence; }
            set
            {
                _SelectedReccurence = value;
            }
        }

        private string _EveryXTime;
        public string EveryXTime
        {
            get { return _EveryXTime; }
            set
            {
                if (int.TryParse(value, out newEvent.Reccurence.EveryXtime))
                {
                    _EveryXTime = newEvent.Reccurence.EveryXtime.ToString();
                }
            }
        }

        public int AlertesTemp { get; } = 30;

        private string _AlertesDuree;
        public string AlertesDuree
        {
            get { return _AlertesDuree; }
            set
            {

                    _AlertesDuree = AlertesTemp.ToString();
            }
        }

        private Dictionary<RepeatType, string> DictRepeat { get; } = new Dictionary<RepeatType, string>
            {
                { RepeatType.Daily , "jours"},
                { RepeatType.Weekly, "semaines"},
                { RepeatType.Monthly, "mois"},
                { RepeatType.Yearly, "ans"}
            };

        public MoreEventPageAdvancedViewModel()
        {
        }

        public void Init(MoreEventPageViewModel MoreEventViewModel)
        {
            this.MoreEventViewModel = MoreEventViewModel;
            newEvent = MoreEventViewModel.newEvent;

            ReccurenceString = DictRepeat.Values.ToList();
            if (newEvent.Reccurence.Repeat != RepeatType.None)
            {
                SelectedReccurence = DictRepeat[newEvent.Reccurence.Repeat];
            }

            else SelectedReccurence = ReccurenceString[0];

            EveryXTime = newEvent.Reccurence.EveryXtime.ToString();
            AlertesString = Dict.Values.ToList();
            _AlertesDuree = "30";
        }
    }
}
