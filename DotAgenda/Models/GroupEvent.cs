using DotAgenda.MethodClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotAgenda.Models
{
    public class GroupEvent
    {
        private string _groupID;

        public string GroupID
        {
            get { return _groupID; }
        }

        private List<EventDay> _GroupEventListe;

        public List<EventDay> GroupEventListe
        {
            get { return _GroupEventListe; }
            set { _GroupEventListe = value; }
        }

        public GroupEvent(string groupID, EventDay firstEvent)
        {
            this.GroupEventListe = new List<EventDay>();
            this._groupID = groupID;
            AddToDictGroupEvent();
            AddEventToListe(firstEvent);
        }

        private void AddToDictGroupEvent()
        {
            if(GlobalDict._dict.DictGroupEvent.Keys.ToList().IndexOf(GroupID) == -1)
                GlobalDict._dict.DictGroupEvent.Add(GroupID, this);
        }

        public void AddEventToListe(EventDay e)
        {
            if (GroupEventListe.IndexOf(e) == -1)
                GroupEventListe.Add(e);
        }

        public void DeleteEventFromListe(EventDay e)
        {
            if (GroupEventListe.IndexOf(e) != -1)
                GroupEventListe.Remove(e);
        }
    }
}
