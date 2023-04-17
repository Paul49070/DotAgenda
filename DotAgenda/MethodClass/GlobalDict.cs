using DotAgenda.MethodClass.DataBaseMethods;
using DotAgenda.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolTip;

namespace DotAgenda.MethodClass
{
    public class GlobalDict
    {
        private static readonly GlobalDict dict = new GlobalDict();

        public static GlobalDict _dict => dict;

        public Dictionary<string, int> DictMois;
        public Dictionary<string, string[]> DossierTypeIconDict;
        public Dictionary<string, string> ExtensionDict;

        public Dictionary<string, ClasseEvent_item> DictClasse;
        public Dictionary<string, GroupEvent> DictGroupEvent;
        public Dictionary<string, Fichier> DictFile;
        public Dictionary<string, string> SmtpList;


        private string Audio = "Audio";
        private string Video = "Video";
        private string Compress = "Compressed";
        private string Data = "Data";
        private string Executable = "Executable";
        private string Image = "Image";
        private string Web = "Web";
        private string Presentation = "Presentation";
        private string Tableau = "Table";
        private string Systeme = "Systeme";
        private string Texte = "Texte";
        private readonly string Code = "Code";
        public string Autre { get; } = "Autre";
        
        private GlobalDict()
        {
            
        }

        public void InitAll()
        {
            DictClasse = new Dictionary<string, ClasseEvent_item>();
            DictGroupEvent = new Dictionary<string, GroupEvent>();
            DictFile = new Dictionary<string, Fichier>();

            DictMois =InitTailleMois();
            DossierTypeIconDict = InitTypeIcon();
            ExtensionDict = InitExtension();
        }

        public void InitSmtpList()
        {
            SmtpList = new Dictionary<string, string>
            {
                { "neuf", "smtp.neuf.fr" },
                { "aliceadsl", "smtp.aliceadsl.fr" },
                { "aol", "smtp.aol.com" },
                { "att", "outbound.att.net" },
                { "bluewin", "smtpauths.bluewin.ch" },
                { "bouygtel", "smtp.bouygtel.fr" },
                { "club-internet", "mail.club-internet.fr" },
                { "free", "smtp.free.fr" },
                { "gmail", "smtp.gmail.com" },
                { "ifrance", "smtp.ifrance.fr" },
                { "hotmail", "smtp.live.com" },
                { "laposte", "smtp.laposte.fr" },
                { "netcourrier", "smtp.netcourrier.com" },
                { "o2", "smtp.o2.com" },
                { "outlook", "smtp.live.com" },
                { "sympatico", "smtphm.sympatico.ca" },
                { "tiscali", "smtp.tiscali.fr" },
                { "verizon", "outgoing.verizon.net" },
                { "voila", "smtp.voila.fr" },
                { "wanadoo", "smtp.wanadoo.fr" },
                { "yahoo", "mail.yahoo.com" }
            };
        }

        public Dictionary<string, int> InitTailleMois()
        {
            Dictionary<string, int> DictMois = new Dictionary<string, int>
            {
                { "Janvier", 31 },
                { "Fevrier", 28 },
                { "Mars", 31 },
                { "Avril", 30 },
                { "Mai", 31 },
                { "Juin", 30 },
                { "Juillet", 31 },
                { "Aout", 31 },
                { "Septembre", 30 },
                { "Octobre", 31 },
                { "Novembre", 30 },
                { "Decembre", 31 }
            };

            return DictMois;
        }

        public Dictionary<string, string[]> InitTypeIcon()
        {
            Dictionary<string, string[]> DossierTypeIconDict = new Dictionary<string, string[]>
            {
                { Audio, new string[] { Audio, "filemusicoutline", "#f55d42" } },
                { Video, new string[] { Video, "filevideooutline", "#42f5ad" } },
                { Compress, new string[] { "Archive", "zipboxoutline", "#f5da42" } },
                { Data, new string[] { Data, "databaseoutline", "#4e42f5" } },
                { Executable, new string[] { Executable, "filesettingsoutline", "#b5b5b5" } },
                { Image, new string[] { Image, "fileimageoutline", "#4266f5" } },
                { Web, new string[] { Web, "webboxoutline", "#bc42f5" } },
                { Presentation, new string[] { Presentation, "FilePresentationBoxoutline", "#a1f542" } },
                { Tableau, new string[] { Tableau, "filetableoutline", "#FFFFFF" } },
                { Systeme, new string[] { Systeme, "filecogoutline", "#FFFFFF" } },
                { Texte, new string[] { Texte, "filedocumentoutline", "#FFFFFF" } },
                { Code, new string[] { Code, "filecodeoutline", "#4287f5" } },
                { Autre, new string[] { Autre, "fileoutline", "#FFFFFF" } }
            };

            return DossierTypeIconDict;
        }



        public Dictionary<string, string> InitExtension()
        {
            Dictionary<string, string> ExtensionDict = new Dictionary<string, string>
            {
                { ".zip", Compress },
                { ".rar", Compress },
                { ".tar", Compress },
                { ".pkg", Compress },
                { ".arj", Compress },
                { ".tar.gz", Compress },
                { ".sitx", Compress },
                { ".7z", Compress },
                { ".rpm", Compress },
                { ".gz", Compress },
                { ".z", Compress },

                { ".aif", Audio },
                { ".cda", Audio },
                { ".mid", Audio },
                { ".midi", Audio },
                { ".mp3", Audio },
                { ".mpa", Audio },
                { ".ogg", Audio },
                { ".wav", Audio },
                { ".wma", Audio },
                { ".wpl", Audio },

                { ".exe", Executable },
                { ".apk", Executable },
                { ".bat", Executable },
                { ".bin", Executable },
                { ".pl", Executable },
                { ".com", Executable },
                { ".gadget", Executable },
                { ".jar", Executable },
                { ".msi", Executable },
                { ".py", Executable },
                { ".wsf", Executable },

                { ".ai", Image },
                { ".bmp", Image },
                { ".gif", Image },
                { ".ico", Image },
                { ".jpg", Image },
                { ".jpeg", Image },
                { ".png", Image },
                { ".ps", Image },
                { ".psd", Image },
                { ".svg", Image },
                { ".tif", Image },
                { ".tiff", Image },
                { ".webp", Image },

                { ".asp", Code },
                { ".aspx", Code },
                { ".cer", Code },
                { ".cfm", Code },
                { ".cgi", Code },
                { ".css", Code },
                { ".html", Code },
                { ".htm", Code },
                { ".js", Code },
                { ".jsp", Code },
                { ".part", Code },
                { ".php", Code },
                { ".rss", Code },
                { ".xhtml", Code },

                { ".key", Presentation },
                { ".odp", Presentation },
                { ".pps", Presentation },
                { ".ppt", Presentation },
                { ".pptx", Presentation },

                { ".c", Code },
                { ".class", Code },
                { ".cpp", Code },
                { ".cs", Code },
                { ".h", Code },
                { ".java", Code },
                { ".sh", Code },
                { ".swift", Code },
                { ".vb", Code },

                { ".ods", Tableau },
                { ".xls", Tableau },
                { ".xlsm", Tableau },
                { ".xlsx", Tableau },

                { ".bak", Systeme },
                { ".cfg", Systeme },
                { ".cpl", Systeme },
                { ".cab", Systeme },
                { ".cur", Systeme },
                { ".dll", Systeme },
                { ".dmp", Systeme },
                { ".drv", Systeme },
                { ".icns", Systeme },
                { ".ini", Systeme },
                { ".lnk", Systeme },
                { ".sys", Systeme },
                { ".tmp", Systeme },

                { ".3g2", Video },
                { ".3gp", Video },
                { ".avi", Video },
                { ".flv", Video },
                { ".h264", Video },
                { ".m4v", Video },
                { ".mkv", Video },
                { ".mov", Video },
                { ".mp4", Video },
                { ".mpg", Video },
                { ".mpeg", Video },
                { ".rm", Video },
                { ".swf", Video },
                { ".vob", Video },
                { ".webm", Video },
                { ".wmv", Video },

                { ".doc", Texte },
                { ".docx", Texte },
                { ".odt", Texte },
                { ".pdf", Texte },
                { ".rtf", Texte },
                { ".tex", Texte },
                { ".txt", Texte },
                { ".wpd", Texte },

                { ".db", Data },
                { ".sql", Data },

                { "null", Autre }
            };

            return ExtensionDict;
        }
    }
}
