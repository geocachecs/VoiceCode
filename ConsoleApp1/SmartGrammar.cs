using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Speech.Recognition;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.Text.RegularExpressions;



// issues: 
//  recognition is spotty -- don't know why...
//  occasional 'RaceOnRCWCleanup' error - appears to occur when too many grammars are unloaded at once!


namespace ConsoleApp2
{
    class SmartGrammars : GrammarClass
    {
        private static string[] keyWords = { "camel", "snake", "allcaps", "nocaps", "low-camel", "low-camel-snake", "camel-snake" };

        private static char[] delim = { '\x1', '\x2', '\x3', '\x4', '\x5', '\x6', '\x7', '\x8', '\x9', '\xa', '\xb', '\xc', '\xd', '\xe', '\xf',
                    '\x10', '\x11', '\x12', '\x13', '\x14', '\x15', '\x16', '\x17', '\x18', '\x19', '\x1a', '\x1b', '\x1c', '\x1d', '\x1e',
                    '\x1f', '\x20', '\x21', '\x22', '\x23', '\x24', '\x25', '\x26', '\x27', '\x28', '\x29', '\x2a', '\x2b', '\x2c', '\x2d',
                    '\x2e', '\x2f', '\x3a', '\x3b', '\x3c', '\x3d', '\x3e', '\x3f', '\x40', '\x5b', '\x5c', '\x5d', '\x5f', '\x5e', '\x60', '\x7b',
                    '\x7c', '\x7d', '\x7e', '\x7f' };

        private static string[] phonetic_alphabet = { "a-b-c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n",
                    "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" };

        private bool active;
        private long lastGrammarUpdate;
        private long MAX_MILLISECONDS_BETWEEN_PURGE = 20000;
        private long MAX_MILLISECOND_TIMEOUT = 40000;
        private int MAX_SMARTWORD_LIST_LENGTH = 1000;
        private string directorypath;
        private string smartGrammarName;
        private List<string> smartWords;
        public Grammar smartGrammar;


        //private static readonly object grammarLock = new object();

        //public SmartGrammars(string filename)
        //{
        //    this.filename = filename;
        //    this.smartWords = new List<string>();
        //    this.smartWords.Add("supercalifragilisticexpialidocious"); // to prevent exceptions when the log file is not available
        //    this.smartGrammar = null;
        //    this.active = false;
        //
        //    string[] filename_tmp = this.filename.Split('\\');
        //    this.smartGrammarName = "SmartGrammar_" + filename_tmp[filename_tmp.Length-1];
        //
        //    this.lastGrammarUpdate = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
        //}

        public SmartGrammars(string directorypath)
        {
            this.directorypath = directorypath;
            this.smartWords = new List<string>();
            this.smartWords.Add("supercalifragilisticexpialidocious"); // to prevent exceptions when the log file is not available
            this.smartGrammar = null;
            this.active = false;

            string[] directorypath_tmp = this.directorypath.Split('\\');
            this.smartGrammarName = "SmartGrammar_" + directorypath[directorypath.Length - 1];

            this.lastGrammarUpdate = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
        }

        private void setSmartWords()
        {
            string screenBufText;
            string[] strarray;
            List<string> tmpwords_A = new List<string>();
            List<string> tmpwords_B = new List<string>();
            List<string> allwords = new List<string>();


            string[] allScreenBufferFilenames = Directory.GetFiles(this.directorypath);
            foreach (string filename in allScreenBufferFilenames)
            {
                try { 
                    screenBufText = System.IO.File.ReadAllText(filename);

                    //File.Delete(filename); // remove when debugging is done

                    // split by delim characters
                    strarray = screenBufText.Split(SmartGrammars.delim, StringSplitOptions.RemoveEmptyEntries);

                    foreach (string word in strarray)
                    {
                        // split by Boundaries from lowercase to uppercase
                        tmpwords_A.AddRange(Regex.Split(word, @"(?=[A-Z])(\B)(?<=[a-z])"));

                    }
                    foreach (string word in tmpwords_A)
                    {
                        // split by letter/number boundaries
                        tmpwords_B.AddRange(Regex.Split(word, @"(?=[A-Za-z])(\B)(?<=[0-9])|(?=[0-9])(\B)(?<=[A-Za-z])"));

                    }

                    allwords.AddRange(tmpwords_B);
                }
                catch
                {
                    continue;
                }
            }

            foreach (string word in allwords.ToArray())
            {
                if(word.Length > 2 || int.TryParse(word, out _))
                {
                    this.smartWords.Remove(word.ToLower());
                    this.smartWords.Add(word.ToLower());

                    if (this.smartWords.Count() == this.MAX_SMARTWORD_LIST_LENGTH + 1)
                    {
                        this.smartWords.RemoveAt(0);
                    }
                    else if (this.smartWords.Count() > this.MAX_SMARTWORD_LIST_LENGTH + 1)
                    {
                        throw new Exception("There are more smartWords than should be allowed");
                    }
                }
            }



        }

        private void setSmartGrammar() {  

            if(this.smartWords.Count == 0)
            {
                return;
            }

            Choices c_keyWords = new Choices();
            Choices c_smartWords = new Choices();
            GrammarBuilder gb = new GrammarBuilder();
            c_keyWords.Add(SmartGrammars.keyWords);
            c_smartWords.Add(this.smartWords.ToArray());

            gb.Append(c_keyWords);
            gb.Append(c_smartWords, 1,10);

            this.smartGrammar = new Grammar(gb);
            this.smartGrammar.Priority = 1;
            this.smartGrammar.Weight = 1;
            this.smartGrammar.Name = this.smartGrammarName; // "NEW_" + this.smartGrammarName;


        }

        public override void HandleSpeechRecognized(object sender, SpeechRecognizedEventArgs e) {
            Console.WriteLine(e.Result.Grammar.Name);
            Console.WriteLine(e.Result.Text);

            if (e.Result.Grammar.Name == "OLD_" + this.smartGrammarName || e.Result.Grammar.Name == this.smartGrammarName || e.Result.Grammar.Name == "NEW_" + this.smartGrammarName)
            {
                System.Console.WriteLine(e.Result.Grammar.Name);
                String output = "";
                string thisword;
                //{ "camel", "score", "allcaps", "nocaps" }
                string keyword = e.Result.Words[0].Text;
                for (int i = 1; i < e.Result.Words.Count; i++)
                {
                    thisword = e.Result.Words[i].Text;
                    switch (keyword)
                    {
                        case "camel":
                            output += Char.ToUpper(thisword[0]) + thisword.Substring(1);
                            break;
                        case "snake":
                            output += thisword + "_";
                            break;
                        case "low-camel":
                            if (i != 1)
                            {
                                output += Char.ToUpper(thisword[0]) + thisword.Substring(1);
                            }
                            else
                            {
                                output += thisword;
                            }
                            break;
                        case "camel-snake":
                            output += Char.ToUpper(thisword[0]) + thisword.Substring(1) + "_";
                            break;
                        case "low-camel-snake":
                            if (i != 1)
                            {
                                output += Char.ToUpper(thisword[0]) + thisword.Substring(1) + "_";
                            }
                            else
                            {
                                output += thisword + "_";
                            }
                            break;

                        case "allcaps":
                            output += thisword.ToUpper();
                            break;
                        case "nocaps":
                            output += thisword.ToLower();
                            break;
                        default:
                            break;
                    }

                }
                if (keyword == "snake" || keyword == "camel-snake" || keyword == "low-camel-snake")
                {
                    output = output.TrimEnd('_');

                }

                System.Console.WriteLine(output);
                SendKeys.SendWait(output);
            }

            // Surreptitiously remove the old grammars
            SpeechRecognitionEngine sender_sre = (SpeechRecognitionEngine)sender;
            updateGrammars(ref sender_sre);
            //this.setSmartWords();
            //this.setSmartGrammar();
            //SpeechRecognitionEngine sre = (SpeechRecognitionEngine)sender;
            //RemoveOldGrammars(ref sre);
            //sre.LoadGrammar(this.smartGrammar);

        }

        
        public override void ConfigureSRE(ref SpeechRecognitionEngine sre)
        {
            if (sre == null)
            {
                sre = new SpeechRecognitionEngine();
            }

            this.updateGrammars(ref sre);
            sre.SpeechRecognized += this.HandleSpeechRecognized;
            this.active = true;
        }

        public override void UpdateSRE(ref SpeechRecognitionEngine sre) {
            if (this.lastGrammarUpdate - DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond > this.MAX_MILLISECONDS_BETWEEN_PURGE)
            {

            }
            if (this.lastGrammarUpdate - DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond > this.MAX_MILLISECONDS_BETWEEN_PURGE)
            {
                updateGrammars(ref sre);
            }

        }


        private bool updateGrammars(ref SpeechRecognitionEngine sre)
        {
            List<Grammar> oldGrammars = new List<Grammar>();
            this.setSmartWords();
            this.setSmartGrammar();

            lock (GrammarClass.grammarLock)
            {

                foreach (Grammar g in sre.Grammars)
                {
                    if (g.Name == this.smartGrammarName)
                    {
                        oldGrammars.Add(g);
                    }
                }

                foreach (Grammar g in oldGrammars)
                {
                    try
                    {
                        sre.UnloadGrammar(g);
                    }
                    catch (Exception e)
                    {
                        continue;
                    }
                }
                sre.LoadGrammar(this.smartGrammar);
                
            }
            this.lastGrammarUpdate = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            return true;
        }


        private void deactivate(ref SpeechRecognitionEngine sre)
        {
            List<Grammar> oldGrammars = new List<Grammar>();
            

            lock (GrammarClass.grammarLock)
            {

                foreach (Grammar g in sre.Grammars)
                {
                    if (g.Name == this.smartGrammarName)
                    {
                        oldGrammars.Add(g);
                    }
                }

                foreach (Grammar g in oldGrammars)
                {
                    try
                    {
                        sre.UnloadGrammar(g);
                    }
                    catch (Exception e)
                    {
                        continue;
                    }
                }
               

            }
        }


    }
}
